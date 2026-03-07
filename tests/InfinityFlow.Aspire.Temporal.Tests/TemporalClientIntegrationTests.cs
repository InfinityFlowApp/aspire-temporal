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

[Collection("Integration")]
[Trait("Category", "Integration")]
public class TemporalClientIntegrationTests
{
    [Fact]
    public async Task AddTemporalClient_ResolvesConnectionAndConnects()
    {
        var ct = TestContext.Current.CancellationToken;
        var (targetHost, aspireApp) = await StartTemporalServer(cancellationToken: ct);
        await using var _ = aspireApp;

        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Configuration["ConnectionStrings:temporal"] = targetHost;

        hostBuilder.AddTemporalClient("temporal");

        using var host = hostBuilder.Build();
        await host.StartAsync(ct);

        var client = host.Services.GetRequiredService<ITemporalClient>();
        Assert.NotNull(client);
        Assert.Equal(targetHost, client.Connection.Options.TargetHost);

        await host.StopAsync(ct);
    }

    [Fact]
    public async Task AddTemporalClient_ConfigureClientSetsNamespace()
    {
        var ct = TestContext.Current.CancellationToken;
        var (targetHost, aspireApp) = await StartTemporalServer(cancellationToken: ct);
        await using var _ = aspireApp;

        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Configuration["ConnectionStrings:temporal"] = targetHost;

        hostBuilder.AddTemporalClient("temporal", opts =>
        {
            opts.Namespace = "custom-ns";
        });

        using var host = hostBuilder.Build();
        await host.StartAsync(ct);

        var client = host.Services.GetRequiredService<ITemporalClient>();
        Assert.Equal("custom-ns", client.Options.Namespace);

        await host.StopAsync(ct);
    }

    [Fact]
    public async Task AddTemporalClient_ConfigureOptionsApplies()
    {
        var ct = TestContext.Current.CancellationToken;
        var (targetHost, aspireApp) = await StartTemporalServer(cancellationToken: ct);
        await using var _ = aspireApp;

        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Configuration["ConnectionStrings:temporal"] = targetHost;

        hostBuilder.AddTemporalClient("temporal")
            .ConfigureOptions(opts => opts.Namespace = "configured-ns");

        using var host = hostBuilder.Build();
        await host.StartAsync(ct);

        var client = host.Services.GetRequiredService<ITemporalClient>();
        Assert.Equal("configured-ns", client.Options.Namespace);

        await host.StopAsync(ct);
    }

    [Fact]
    public async Task HealthCheck_ReturnsHealthy_WhenConnected()
    {
        var ct = TestContext.Current.CancellationToken;
        var (targetHost, aspireApp) = await StartTemporalServer(cancellationToken: ct);
        await using var _ = aspireApp;

        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Configuration["ConnectionStrings:temporal"] = targetHost;

        hostBuilder.AddTemporalClient("temporal");

        using var host = hostBuilder.Build();
        await host.StartAsync(ct);

        var healthCheckService = host.Services.GetRequiredService<HealthCheckService>();
        var result = await healthCheckService.CheckHealthAsync(ct);

        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Contains(result.Entries, e => e.Key == "temporal-temporal");

        await host.StopAsync(ct);
    }

    [Fact]
    public async Task AddTemporalWorker_RegistersWorkflowAndActivities()
    {
        var ct = TestContext.Current.CancellationToken;
        var (targetHost, aspireApp) = await StartTemporalServer(cancellationToken: ct);
        await using var _ = aspireApp;

        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Configuration["ConnectionStrings:temporal"] = targetHost;

        hostBuilder.AddTemporalWorker("temporal", "test-queue")
            .AddWorkflow<TestWorkflow>()
            .AddScopedActivities<TestActivities>();

        using var host = hostBuilder.Build();
        await host.StartAsync(ct);

        var client = host.Services.GetRequiredService<ITemporalClient>();
        Assert.NotNull(client);

        await host.StopAsync(ct);
    }

    [Fact]
    public async Task AddTemporalWorker_ExecutesWorkflow()
    {
        var ct = TestContext.Current.CancellationToken;
        var (targetHost, aspireApp) = await StartTemporalServer(cancellationToken: ct);
        await using var _ = aspireApp;

        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Configuration["ConnectionStrings:temporal"] = targetHost;

        hostBuilder.AddTemporalWorker("temporal", "e2e-queue")
            .AddWorkflow<TestWorkflow>()
            .AddScopedActivities<TestActivities>();

        using var host = hostBuilder.Build();
        await host.StartAsync(ct);

        var client = host.Services.GetRequiredService<ITemporalClient>();

        var result = await client.ExecuteWorkflowAsync(
            (TestWorkflow wf) => wf.RunAsync("world"),
            new WorkflowOptions($"test-{Guid.NewGuid()}", "e2e-queue"));

        Assert.Equal("Hello world", result);

        await host.StopAsync(ct);
    }

    [Fact]
    public async Task AddTemporalWorker_TransientActivities()
    {
        var ct = TestContext.Current.CancellationToken;
        var (targetHost, aspireApp) = await StartTemporalServer(cancellationToken: ct);
        await using var _ = aspireApp;

        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Configuration["ConnectionStrings:temporal"] = targetHost;

        hostBuilder.AddTemporalWorker("temporal", "transient-queue")
            .AddWorkflow<TestWorkflow>()
            .AddTransientActivities<TestActivities>();

        using var host = hostBuilder.Build();
        await host.StartAsync(ct);

        var client = host.Services.GetRequiredService<ITemporalClient>();

        var result = await client.ExecuteWorkflowAsync(
            (TestWorkflow wf) => wf.RunAsync("transient"),
            new WorkflowOptions($"test-{Guid.NewGuid()}", "transient-queue"));

        Assert.Equal("Hello transient", result);

        await host.StopAsync(ct);
    }

    [Fact]
    public async Task AddTemporalWorker_SingletonActivities()
    {
        var ct = TestContext.Current.CancellationToken;
        var (targetHost, aspireApp) = await StartTemporalServer(cancellationToken: ct);
        await using var _ = aspireApp;

        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Configuration["ConnectionStrings:temporal"] = targetHost;

        hostBuilder.AddTemporalWorker("temporal", "singleton-queue")
            .AddWorkflow<TestWorkflow>()
            .AddSingletonActivities<TestActivities>();

        using var host = hostBuilder.Build();
        await host.StartAsync(ct);

        var client = host.Services.GetRequiredService<ITemporalClient>();

        var result = await client.ExecuteWorkflowAsync(
            (TestWorkflow wf) => wf.RunAsync("singleton"),
            new WorkflowOptions($"test-{Guid.NewGuid()}", "singleton-queue"));

        Assert.Equal("Hello singleton", result);

        await host.StopAsync(ct);
    }

    [Fact]
    public async Task AddTemporalWorker_ActivitiesInstance()
    {
        var ct = TestContext.Current.CancellationToken;
        var (targetHost, aspireApp) = await StartTemporalServer(cancellationToken: ct);
        await using var _ = aspireApp;

        var hostBuilder = Host.CreateApplicationBuilder();
        hostBuilder.Configuration["ConnectionStrings:temporal"] = targetHost;

        hostBuilder.AddTemporalWorker("temporal", "instance-queue")
            .AddWorkflow<TestWorkflow>()
            .AddActivitiesInstance(new TestActivities());

        using var host = hostBuilder.Build();
        await host.StartAsync(ct);

        var client = host.Services.GetRequiredService<ITemporalClient>();

        var result = await client.ExecuteWorkflowAsync(
            (TestWorkflow wf) => wf.RunAsync("instance"),
            new WorkflowOptions($"test-{Guid.NewGuid()}", "instance-queue"));

        Assert.Equal("Hello instance", result);

        await host.StopAsync(ct);
    }

    /// <summary>
    /// Starts a Temporal dev server via Aspire and returns (targetHost, app).
    /// Each test gets a uniquely named resource to avoid container conflicts.
    /// </summary>
    private static async Task<(string TargetHost, DistributedApplication App)> StartTemporalServer(
        [System.Runtime.CompilerServices.CallerMemberName] string callerName = "",
        CancellationToken cancellationToken = default)
    {
        var resourceName = $"t-{callerName}".ToLowerInvariant().Replace("_", "-");
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.TestAppHost>(cancellationToken);

        var temporal = builder.AddTemporalServerContainer(resourceName);

        // Make the server endpoint non-proxied so the Temporal gRPC client can connect directly.
        temporal.WithEndpoint("server", e => { e.IsProxied = false; e.UriScheme = "http"; });

        var app = await builder.BuildAsync(cancellationToken);

        var rns = app.Services.GetRequiredService<ResourceNotificationService>();
        await app.StartAsync(cancellationToken);

        await rns.WaitForResourceAsync(resourceName, KnownResourceStates.Running, cancellationToken)
            .WaitAsync(TimeSpan.FromSeconds(120), cancellationToken);

        var serverEndpoint = temporal.GetEndpoint("server");
        var uri = new Uri(serverEndpoint.Url);
        var targetHost = $"{uri.Host}:{uri.Port}";

        // Allow server to fully initialize
        await Task.Delay(3000, cancellationToken);

        return (targetHost, app);
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
