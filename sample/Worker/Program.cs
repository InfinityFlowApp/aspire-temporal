using InfinityFlow.Aspire.Temporal.Client;
using Temporalio.Activities;
using Temporalio.Workflows;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddTemporalWorker("temporal", Constants.TaskQueueName, opts =>
{
    opts.Namespace = Constants.Namespace;
})
.AddWorkflow<HelloWorkflow>()
.AddScopedActivities<HelloActivities>();

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
