using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Temporalio.Extensions.OpenTelemetry;

namespace InfinityFlow.Aspire.Temporal.Client;

/// <summary>
/// Extension methods for configuring OpenTelemetry meters and tracing sources for Temporal.
/// </summary>
public static class TemporalServiceDefaultsExtensions
{
    /// <summary>
    /// Wires up OpenTelemetry meter and tracing sources for Temporal client and worker telemetry.
    /// Call this from your service defaults configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddTemporalServiceDefaults(this IServiceCollection services)
    {
        services.ConfigureOpenTelemetryMeterProvider(metrics =>
            metrics.AddMeter(AspireTemporalExtensions.MeterName));

        services.ConfigureOpenTelemetryTracerProvider(tracing =>
            tracing.AddSource(
                TracingInterceptor.ClientSource.Name,
                TracingInterceptor.WorkflowsSource.Name,
                TracingInterceptor.ActivitiesSource.Name));

        return services;
    }
}
