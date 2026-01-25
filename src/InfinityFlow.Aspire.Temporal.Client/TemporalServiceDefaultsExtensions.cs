using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Aspire.Hosting;

/// <summary>
/// Provides extension methods for configuring Temporal OpenTelemetry in Aspire service defaults.
/// </summary>
public static class TemporalServiceDefaultsExtensions
{
    /// <summary>
    /// Adds Temporal tracing sources to OpenTelemetry tracing configuration.
    /// Call this in your ServiceDefaults Extensions.cs file.
    /// </summary>
    /// <param name="builder">The tracing builder.</param>
    /// <returns>The tracing builder for method chaining.</returns>
    public static TracerProviderBuilder AddTemporalTracing(this TracerProviderBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Add the Temporal tracing activity sources
        // These correspond to TracingInterceptor.ClientSource, WorkflowsSource, and ActivitiesSource
        return builder.AddSource(
            "Temporalio.Client",
            "Temporalio.Workflows",
            "Temporalio.Activities");
    }

    /// <summary>
    /// Adds Temporal metrics meter to OpenTelemetry metrics configuration.
    /// Call this in your ServiceDefaults Extensions.cs file.
    /// </summary>
    /// <param name="builder">The metrics builder.</param>
    /// <returns>The metrics builder for method chaining.</returns>
    public static MeterProviderBuilder AddTemporalMetrics(this MeterProviderBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddMeter("Temporal.Client");
    }

    /// <summary>
    /// Configures OpenTelemetry for Temporal in Aspire service defaults.
    /// This is a convenience method that adds both tracing and metrics.
    /// </summary>
    /// <param name="builder">The logging builder from AddServiceDefaults().</param>
    /// <returns>The logging builder for method chaining.</returns>
    public static ILoggingBuilder AddTemporalObservability(this ILoggingBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.ConfigureOpenTelemetryTracerProvider(tracing =>
            tracing.AddTemporalTracing());

        builder.Services.ConfigureOpenTelemetryMeterProvider(metrics =>
            metrics.AddTemporalMetrics());

        return builder;
    }
}
