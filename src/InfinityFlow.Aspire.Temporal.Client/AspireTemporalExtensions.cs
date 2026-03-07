using System.Diagnostics.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Temporalio.Client;
using Temporalio.Extensions.DiagnosticSource;
using Temporalio.Extensions.Hosting;
using Temporalio.Extensions.OpenTelemetry;
using Temporalio.Runtime;

namespace InfinityFlow.Aspire.Temporal.Client;

/// <summary>
/// Extension methods for adding Temporal client and worker registrations with automatic
/// connection string resolution and OpenTelemetry integration.
/// </summary>
public static class AspireTemporalExtensions
{
    internal const string MeterName = "Temporal.Client";

    /// <summary>
    /// Registers a Temporal client with automatic connection string resolution and OpenTelemetry support.
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    /// <param name="connectionName">The connection string name. Defaults to "temporal".</param>
    /// <param name="configureClient">Optional additional configuration for the client connect options.</param>
    /// <returns>A <see cref="TemporalClientBuilder"/> for further configuration.</returns>
    public static TemporalClientBuilder AddTemporalClient(
        this IHostApplicationBuilder builder,
        string connectionName = "temporal",
        Action<TemporalClientConnectOptions>? configureClient = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var connectionString = builder.Configuration.GetConnectionString(connectionName);
        if (string.IsNullOrEmpty(connectionString))
        {
            var available = builder.Configuration.GetSection("ConnectionStrings").GetChildren()
                .Select(c => c.Key);
            throw new InvalidOperationException(
                $"Temporal connection string '{connectionName}' not found. " +
                $"Available connection strings: [{string.Join(", ", available)}]. " +
                "Ensure the Temporal resource is referenced in AppHost using .WithReference(temporal).");
        }

        var runtime = RegisterTemporalRuntime(builder.Services);

        builder.Services.AddTemporalClient(opts =>
        {
            opts.TargetHost = connectionString;
            opts.Runtime = runtime;
            opts.Interceptors = [new TracingInterceptor()];
            configureClient?.Invoke(opts);
        });

        RegisterHealthCheck(builder.Services, connectionName);

        return new TemporalClientBuilder(builder.Services);
    }

    /// <summary>
    /// Registers a hosted Temporal worker with automatic connection string resolution and OpenTelemetry support.
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    /// <param name="connectionName">The connection string name.</param>
    /// <param name="taskQueue">The task queue for the worker.</param>
    /// <param name="configureClient">Optional additional configuration for the client connect options.</param>
    /// <returns>A <see cref="TemporalWorkerBuilder"/> for further configuration.</returns>
    public static TemporalWorkerBuilder AddTemporalWorker(
        this IHostApplicationBuilder builder,
        string connectionName,
        string taskQueue,
        Action<TemporalClientConnectOptions>? configureClient = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(taskQueue);

        var connectionString = builder.Configuration.GetConnectionString(connectionName);
        if (string.IsNullOrEmpty(connectionString))
        {
            var available = builder.Configuration.GetSection("ConnectionStrings").GetChildren()
                .Select(c => c.Key);
            throw new InvalidOperationException(
                $"Temporal connection string '{connectionName}' not found. " +
                $"Available connection strings: [{string.Join(", ", available)}]. " +
                "Ensure the Temporal resource is referenced in AppHost using .WithReference(temporal).");
        }

        var runtime = RegisterTemporalRuntime(builder.Services);

        var workerOptionsBuilder = builder.Services
            .AddTemporalClient(opts =>
            {
                opts.TargetHost = connectionString;
                opts.Runtime = runtime;
                opts.Interceptors = [new TracingInterceptor()];
                configureClient?.Invoke(opts);
            })
            .AddHostedTemporalWorker(taskQueue);

        RegisterHealthCheck(builder.Services, connectionName);

        return new TemporalWorkerBuilder(workerOptionsBuilder, builder.Services);
    }

    private static TemporalRuntime RegisterTemporalRuntime(IServiceCollection services)
    {
        var existing = services.FirstOrDefault(d => d.ServiceType == typeof(TemporalRuntimeRegistration));
        if (existing?.ImplementationInstance is TemporalRuntimeRegistration reg)
            return reg.Runtime;

        var meter = new Meter(MeterName);
        var runtime = new TemporalRuntime(new TemporalRuntimeOptions
        {
            Telemetry = new TelemetryOptions
            {
                Metrics = new MetricsOptions { CustomMetricMeter = new CustomMetricMeter(meter) },
            },
        });

        services.AddSingleton(new TemporalRuntimeRegistration(meter, runtime));
        services.AddSingleton(meter);
        services.AddSingleton(runtime);
        return runtime;
    }

    private static void RegisterHealthCheck(IServiceCollection services, string connectionName)
    {
        var healthCheckName = $"temporal-{connectionName}";
        // Avoid duplicate health check registration when both AddTemporalClient and AddTemporalWorker are used
        if (services.Any(d => d.ServiceType == typeof(HealthCheckRegistrationMarker)
            && d.ImplementationInstance is HealthCheckRegistrationMarker m && m.Name == healthCheckName))
            return;

        services.AddSingleton(new HealthCheckRegistrationMarker(healthCheckName));
        services.AddHealthChecks()
            .AddCheck<TemporalHealthCheck>(healthCheckName);
    }

    private sealed class TemporalRuntimeRegistration(Meter meter, TemporalRuntime runtime)
    {
        public Meter Meter { get; } = meter;
        public TemporalRuntime Runtime { get; } = runtime;
    }

    private sealed record HealthCheckRegistrationMarker(string Name);
}
