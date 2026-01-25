using Aspire.Hosting;
using Temporalio.Activities;
using Temporalio.Extensions.Hosting;
using Temporalio.Workflows;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// Use the new fluent API for Temporal worker configuration
// This automatically sets up OpenTelemetry tracing and metrics for the Aspire dashboard
builder.AddTemporalWorker(Constants.TaskQueueName)
    .ConfigureOptions(opts =>
    {
        opts.Namespace = Constants.Namespace;
    })
    .Worker?
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
