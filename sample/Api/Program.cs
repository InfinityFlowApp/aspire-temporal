using System.Diagnostics.Metrics;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using Temporalio.Client;
using Temporalio.Extensions.DiagnosticSource;
using Temporalio.Extensions.OpenTelemetry;
using Temporalio.Runtime;

var builder = WebApplication.CreateBuilder(args);

using var meter = new Meter("Temporal.Client");

var runtime = new TemporalRuntime(new()
{
    Telemetry = new()
    {
        Metrics = new() { CustomMetricMeter = new CustomMetricMeter(meter) },
    },
});

builder.AddServiceDefaults();

builder.Services.AddTemporalClient(opts =>
{
    opts.TargetHost = builder.Configuration["ConnectionStrings:temporal"];
    opts.Namespace = Constants.Namespace;
    opts.Interceptors = [new TracingInterceptor()];
    opts.Runtime = runtime;
});

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseHttpsRedirection();

app.MapGet("/say-hello", async Task<Results<Ok<string>, BadRequest<string>>> (
    HttpContext ctx,
    [FromQuery] string? name,
    [FromServices] ITemporalClient client) =>
{
    if (name is null or { Length: > 1000 })
    {
        return TypedResults.BadRequest($"Pass a name using query param {nameof(name)}");
    }

    var result = await client.ExecuteWorkflowAsync<HelloWorkflow, string>(x => x.RunAsync(name),
        new WorkflowOptions
        {
            Id = name,
            TaskQueue = Constants.TaskQueueName
        });

    return TypedResults.Ok(result);
});

app.Run();