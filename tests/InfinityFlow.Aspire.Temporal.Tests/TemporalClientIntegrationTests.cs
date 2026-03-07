using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using InfinityFlow.Aspire.Temporal;
using InfinityFlow.Aspire.Temporal.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Temporalio.Activities;
using Temporalio.Client;
using Temporalio.Workflows;
using Xunit;

namespace InfinityFlow.Aspire.Temporal.Tests;

[Trait("Category", "Integration")]
public class TemporalClientIntegrationTests
{
    [Fact]
    public async Task AddTemporalClient_ResolvesConnectionAndConnects()
    {
        var (address, port, aspireApp) = await StartTemporalServer();
        await using var _ = aspireApp;

        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Configuration["ConnectionStrings:temporal"] = $"{address}:{port}";

        hostBuilder.AddTemporalClient("temporal");

        using var host = hostBuilder.Build();
        await host.StartAsync();

        var client = host.Services.GetRequiredService<ITemporalClient>();
        Assert.NotNull(client);
        Assert.Equal($"{address}:{port}", client.Connection.Options.TargetHost);

        await host.StopAsync();
    }

    [Fact]
    public async Task AddTemporalClient_ConfigureClientSetsNamespace()
    {
        var (address, port, aspireApp) = await StartTemporalServer();
        await using var _ = aspireApp;

        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Configuration["ConnectionStrings:temporal"] = $"{address}:{port}";

        hostBuilder.AddTemporalClient("temporal", opts =>
        {
            opts.Namespace = "custom-ns";
        });

        using var host = hostBuilder.Build();
        await host.StartAsync();

        var client = host.Services.GetRequiredService<ITemporalClient>();
        Assert.Equal("custom-ns", client.Options.Namespace);

        await host.StopAsync();
    }

    [Fact]
    public async Task AddTemporalClient_ConfigureOptionsApplies()
    {
        var (address, port, aspireApp) = await StartTemporalServer();
        await using var _ = aspireApp;

        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Configuration["ConnectionStrings:temporal"] = $"{address}:{port}";

        hostBuilder.AddTemporalClient("temporal")
            .ConfigureOptions(opts => opts.Namespace = "configured-ns");

        using var host = hostBuilder.Build();
        await host.StartAsync();

        var client = host.Services.GetRequiredService<ITemporalClient>();
        Assert.Equal("configured-ns", client.Options.Namespace);

        await host.StopAsync();
    }

    [Fact]
    public async Task HealthCheck_ReturnsHealthy_WhenConnected()
    {
        var (address, port, aspireApp) = await StartTemporalServer();
        await using var _ = aspireApp;

        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Configuration["ConnectionStrings:temporal"] = $"{address}:{port}";

        hostBuilder.AddTemporalClient("temporal");

        using var host = hostBuilder.Build();
        await host.StartAsync();

        var healthCheckService = host.Services.GetRequiredService<HealthCheckService>();
        var result = await healthCheckService.CheckHealthAsync();

        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Contains(result.Entries, e => e.Key == "temporal-temporal");

        await host.StopAsync();
    }

    [Fact]
    public async Task AddTemporalWorker_RegistersWorkflowAndActivities()
    {
        var (address, port, aspireApp) = await StartTemporalServer();
        await using var _ = aspireApp;

        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Configuration["ConnectionStrings:temporal"] = $"{address}:{port}";

        hostBuilder.AddTemporalWorker("temporal", "test-queue")
            .AddWorkflow<TestWorkflow>()
            .AddScopedActivities<TestActivities>();

        using var host = hostBuilder.Build();
        await host.StartAsync();

        var client = host.Services.GetRequiredService<ITemporalClient>();
        Assert.NotNull(client);

        await host.StopAsync();
    }

    [Fact]
    public async Task AddTemporalWorker_ExecutesWorkflow()
    {
        var (address, port, aspireApp) = await StartTemporalServer();
        await using var _ = aspireApp;

        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Configuration["ConnectionStrings:temporal"] = $"{address}:{port}";

        hostBuilder.AddTemporalWorker("temporal", "e2e-queue")
            .AddWorkflow<TestWorkflow>()
            .AddScopedActivities<TestActivities>();

        using var host = hostBuilder.Build();
        await host.StartAsync();

        var client = host.Services.GetRequiredService<ITemporalClient>();

        var result = await client.ExecuteWorkflowAsync(
            (TestWorkflow wf) => wf.RunAsync("world"),
            new WorkflowOptions($"test-{Guid.NewGuid()}", "e2e-queue"));

        Assert.Equal("Hello world", result);

        await host.StopAsync();
    }

    [Fact]
    public async Task AddTemporalWorker_TransientActivities()
    {
        var (address, port, aspireApp) = await StartTemporalServer();
        await using var _ = aspireApp;

        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Configuration["ConnectionStrings:temporal"] = $"{address}:{port}";

        hostBuilder.AddTemporalWorker("temporal", "transient-queue")
            .AddWorkflow<TestWorkflow>()
            .AddTransientActivities<TestActivities>();

        using var host = hostBuilder.Build();
        await host.StartAsync();

        var client = host.Services.GetRequiredService<ITemporalClient>();

        var result = await client.ExecuteWorkflowAsync(
            (TestWorkflow wf) => wf.RunAsync("transient"),
            new WorkflowOptions($"test-{Guid.NewGuid()}", "transient-queue"));

        Assert.Equal("Hello transient", result);

        await host.StopAsync();
    }

    [Fact]
    public async Task AddTemporalWorker_SingletonActivities()
    {
        var (address, port, aspireApp) = await StartTemporalServer();
        await using var _ = aspireApp;

        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Configuration["ConnectionStrings:temporal"] = $"{address}:{port}";

        hostBuilder.AddTemporalWorker("temporal", "singleton-queue")
            .AddWorkflow<TestWorkflow>()
            .AddSingletonActivities<TestActivities>();

        using var host = hostBuilder.Build();
        await host.StartAsync();

        var client = host.Services.GetRequiredService<ITemporalClient>();

        var result = await client.ExecuteWorkflowAsync(
            (TestWorkflow wf) => wf.RunAsync("singleton"),
            new WorkflowOptions($"test-{Guid.NewGuid()}", "singleton-queue"));

        Assert.Equal("Hello singleton", result);

        await host.StopAsync();
    }

    [Fact]
    public async Task AddTemporalWorker_ActivitiesInstance()
    {
        var (address, port, aspireApp) = await StartTemporalServer();
        await using var _ = aspireApp;

        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Configuration["ConnectionStrings:temporal"] = $"{address}:{port}";

        hostBuilder.AddTemporalWorker("temporal", "instance-queue")
            .AddWorkflow<TestWorkflow>()
            .AddActivitiesInstance(new TestActivities());

        using var host = hostBuilder.Build();
        await host.StartAsync();

        var client = host.Services.GetRequiredService<ITemporalClient>();

        var result = await client.ExecuteWorkflowAsync(
            (TestWorkflow wf) => wf.RunAsync("instance"),
            new WorkflowOptions($"test-{Guid.NewGuid()}", "instance-queue"));

        Assert.Equal("Hello instance", result);

        await host.StopAsync();
    }

    /// <summary>
    /// Starts a Temporal dev server via Aspire and returns (address, port, app).
    /// Each test gets a uniquely named resource to avoid container conflicts.
    /// </summary>
    private static async Task<(string Address, int Port, DistributedApplication App)> StartTemporalServer(
        [System.Runtime.CompilerServices.CallerMemberName] string callerName = "")
    {
        var resourceName = $"t-{callerName}".ToLowerInvariant().Replace("_", "-");
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.TestAppHost>();

        var temporal = builder.AddTemporalServerContainer(resourceName);
        temporal.WithEndpoint(scheme: "http", targetPort: 7233, name: "grpc-direct", isProxied: false);

        var app = await builder.BuildAsync();

        var rns = app.Services.GetRequiredService<ResourceNotificationService>();
        await app.StartAsync();

        await rns.WaitForResourceAsync(resourceName, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromSeconds(120));

        var directEndpoint = temporal.Resource.Annotations
            .OfType<EndpointAnnotation>()
            .Single(e => e.Name == "grpc-direct");

        var address = directEndpoint.AllocatedEndpoint!.Address;
        var port = directEndpoint.AllocatedEndpoint!.Port;

        // Allow server to fully initialize
        await Task.Delay(3000);

        return (address, port, app);
    }

    [Workflow]
    public class TestWorkflow
    {
        [WorkflowRun]
        public async Task<string> RunAsync(string name)
        {
            return await Workflow.ExecuteActivityAsync(
                (TestActivities act) => act.Greet(name),
                new ActivityOptions { ScheduleToCloseTimeout = TimeSpan.FromMinutes(1) });
        }
    }

    public class TestActivities
    {
        [Activity]
        public Task<string> Greet(string name) => Task.FromResult($"Hello {name}");
    }
}
