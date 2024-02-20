using System.Diagnostics.Metrics;

using Temporalio.Activities;
using Temporalio.Extensions.DiagnosticSource;
using Temporalio.Extensions.Hosting;
using Temporalio.Extensions.OpenTelemetry;
using Temporalio.Runtime;
using Temporalio.Workflows;

var builder = Host.CreateApplicationBuilder(args);

using var meter = new Meter("Temporal.Client");

var runtime = new TemporalRuntime(new()
{
    Telemetry = new()
    {
        Metrics = new() { CustomMetricMeter = new CustomMetricMeter(meter) },
    },
});

builder.AddServiceDefaults();

builder.Services
    .AddTemporalClient(opts =>
    {
        opts.TargetHost = builder.Configuration["ConnectionStrings:temporal"];
        opts.Namespace = Constants.Namespace;
        opts.Interceptors = new[] { new TracingInterceptor() };
        opts.Runtime = runtime;
    })
    .AddHostedTemporalWorker(Constants.TaskQueueName)
    .AddScopedActivities<HelloActivities>()
    .AddWorkflow<HelloWorkflow>();

var host = builder.Build();
host.Run();

public static class Constants
{
    public const string TaskQueueName = "aspire-worker-task-queue";
    public const string Namespace = "test1";
}

[Workflow]
public class HelloWorkflow
{
    [WorkflowRun]
    public async Task<string> RunAsync(string name)
    {
        return await Workflow.ExecuteActivityAsync((HelloActivities act) => act.SayHello(name),
            new() { ScheduleToCloseTimeout = TimeSpan.FromMinutes(5) });
    }
}

public class HelloActivities
{
    [Activity]
    public Task<string> SayHello(string name)
    {
        return Task.FromResult("Hello " + name);
    }
}
