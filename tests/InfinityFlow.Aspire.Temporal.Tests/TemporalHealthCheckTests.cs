using InfinityFlow.Aspire.Temporal.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit;

namespace InfinityFlow.Aspire.Temporal.Tests;

public class TemporalHealthCheckTests
{
    [Fact]
    public void HealthCheck_IsRegisteredWithCorrectName()
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            EnvironmentName = "Testing",
        });
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:myconn"] = "localhost:7233"
        });

        builder.AddTemporalClient("myconn");

        var sp = builder.Services.BuildServiceProvider();
        var options = sp.GetRequiredService<IOptions<HealthCheckServiceOptions>>();
        Assert.Contains(options.Value.Registrations, r => r.Name == "temporal-myconn");
    }

    [Fact]
    public void HealthCheck_WorkerRegistration_UsesConnectionName()
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            EnvironmentName = "Testing",
        });
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:worker-conn"] = "localhost:7233"
        });

        builder.AddTemporalWorker("worker-conn", "my-queue");

        var sp = builder.Services.BuildServiceProvider();
        var options = sp.GetRequiredService<IOptions<HealthCheckServiceOptions>>();
        Assert.Contains(options.Value.Registrations, r => r.Name == "temporal-worker-conn");
    }
}
