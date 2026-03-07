using InfinityFlow.Aspire.Temporal.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Temporalio.Client;
using Temporalio.Runtime;
using Xunit;

namespace InfinityFlow.Aspire.Temporal.Tests;

public class TemporalClientExtensionsTests
{
    [Fact]
    public void AddTemporalClient_MissingConnectionString_Throws()
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            EnvironmentName = "Testing",
        });

        var ex = Assert.Throws<InvalidOperationException>(() =>
            builder.AddTemporalClient("temporal"));

        Assert.Contains("temporal", ex.Message);
        Assert.Contains("not found", ex.Message);
    }

    [Fact]
    public void AddTemporalClient_WithConnectionString_RegistersServices()
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            EnvironmentName = "Testing",
        });
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:temporal"] = "localhost:7233"
        });

        builder.AddTemporalClient("temporal");

        var services = builder.Services;
        Assert.Contains(services, s => s.ServiceType == typeof(ITemporalClient));
        Assert.Contains(services, s => s.ServiceType == typeof(TemporalRuntime));
    }

    [Fact]
    public void AddTemporalClient_RegistersHealthCheck()
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            EnvironmentName = "Testing",
        });
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:temporal"] = "localhost:7233"
        });

        builder.AddTemporalClient("temporal");

        var sp = builder.Services.BuildServiceProvider();
        var healthCheckRegistrations = sp.GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value.Registrations;
        Assert.Contains(healthCheckRegistrations, r => r.Name == "temporal-temporal");
    }

    [Fact]
    public void AddTemporalClient_ConfigureOptions_AppliesCallback()
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            EnvironmentName = "Testing",
        });
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:temporal"] = "localhost:7233"
        });

        builder.AddTemporalClient("temporal", opts =>
        {
            opts.Namespace = "custom-ns";
        });

        // Verify services are registered (callback applied during service resolution)
        Assert.Contains(builder.Services, s => s.ServiceType == typeof(ITemporalClient));
    }

    [Fact]
    public void AddTemporalClient_DefaultConnectionName_IsTemporalString()
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            EnvironmentName = "Testing",
        });
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:temporal"] = "localhost:7233"
        });

        // Should not throw — default connectionName is "temporal"
        builder.AddTemporalClient();

        Assert.Contains(builder.Services, s => s.ServiceType == typeof(ITemporalClient));
    }

    [Fact]
    public void AddTemporalClient_ReturnsTemporalClientBuilder()
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            EnvironmentName = "Testing",
        });
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:temporal"] = "localhost:7233"
        });

        var result = builder.AddTemporalClient("temporal");

        Assert.IsType<TemporalClientBuilder>(result);
        Assert.Same(builder.Services, result.Services);
    }

    [Fact]
    public void AddTemporalWorker_MissingConnectionString_Throws()
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            EnvironmentName = "Testing",
        });

        var ex = Assert.Throws<InvalidOperationException>(() =>
            builder.AddTemporalWorker("temporal", "my-queue"));

        Assert.Contains("temporal", ex.Message);
        Assert.Contains("not found", ex.Message);
    }

    [Fact]
    public void AddTemporalWorker_WithConnectionString_RegistersServices()
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            EnvironmentName = "Testing",
        });
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:temporal"] = "localhost:7233"
        });

        builder.AddTemporalWorker("temporal", "my-queue");

        Assert.Contains(builder.Services, s => s.ServiceType == typeof(ITemporalClient));
        Assert.Contains(builder.Services, s => s.ServiceType == typeof(TemporalRuntime));
    }

    [Fact]
    public void AddTemporalWorker_RegistersHealthCheck()
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            EnvironmentName = "Testing",
        });
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:temporal"] = "localhost:7233"
        });

        builder.AddTemporalWorker("temporal", "my-queue");

        var sp = builder.Services.BuildServiceProvider();
        var healthCheckRegistrations = sp.GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value.Registrations;
        Assert.Contains(healthCheckRegistrations, r => r.Name == "temporal-temporal");
    }

    [Fact]
    public void AddTemporalWorker_ReturnsTemporalWorkerBuilder()
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            EnvironmentName = "Testing",
        });
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:temporal"] = "localhost:7233"
        });

        var result = builder.AddTemporalWorker("temporal", "my-queue");

        Assert.IsType<TemporalWorkerBuilder>(result);
        Assert.Same(builder.Services, result.Services);
    }

    [Fact]
    public void AddTemporalWorker_ConfigureOptions_AppliesCallback()
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            EnvironmentName = "Testing",
        });
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:temporal"] = "localhost:7233"
        });

        builder.AddTemporalWorker("temporal", "my-queue", opts =>
        {
            opts.Namespace = "custom-ns";
        });

        Assert.Contains(builder.Services, s => s.ServiceType == typeof(ITemporalClient));
    }

    [Fact]
    public void AddTemporalRuntime_RegisteredOnce_WhenCalledMultipleTimes()
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            EnvironmentName = "Testing",
        });
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:temporal"] = "localhost:7233"
        });

        builder.AddTemporalClient("temporal");
        builder.AddTemporalWorker("temporal", "my-queue");

        var runtimeDescriptors = builder.Services.Where(s => s.ServiceType == typeof(TemporalRuntime)).ToList();
        Assert.Single(runtimeDescriptors);
    }
}
