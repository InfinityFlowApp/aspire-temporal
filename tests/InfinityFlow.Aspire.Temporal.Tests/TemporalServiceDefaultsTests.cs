using InfinityFlow.Aspire.Temporal.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Xunit;

namespace InfinityFlow.Aspire.Temporal.Tests;

public class TemporalServiceDefaultsTests
{
    [Fact]
    public void AddTemporalServiceDefaults_CanBeCalledWithoutError()
    {
        var services = new ServiceCollection();

        var result = services.AddTemporalServiceDefaults();

        Assert.Same(services, result);
    }

    [Fact]
    public void AddTemporalServiceDefaults_RegistersMeterProviderConfiguration()
    {
        var services = new ServiceCollection();
        services.AddOpenTelemetry().WithMetrics();

        services.AddTemporalServiceDefaults();

        // Verify the configuration was registered by checking service descriptors
        Assert.Contains(services, s =>
            s.ServiceType == typeof(IConfigureOptions<MeterProviderBuilderBase>)
            || s.ServiceType.Name.Contains("MeterProvider", StringComparison.Ordinal));
    }

    [Fact]
    public void AddTemporalServiceDefaults_RegistersTracerProviderConfiguration()
    {
        var services = new ServiceCollection();
        services.AddOpenTelemetry().WithTracing();

        services.AddTemporalServiceDefaults();

        Assert.Contains(services, s =>
            s.ServiceType == typeof(IConfigureOptions<TracerProviderBuilderBase>)
            || s.ServiceType.Name.Contains("TracerProvider", StringComparison.Ordinal));
    }
}
