using InfinityFlow.Aspire.Temporal.Client;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Temporalio.Client;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddTemporalClient("temporal", opts =>
{
    opts.Namespace = Constants.Namespace;
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
