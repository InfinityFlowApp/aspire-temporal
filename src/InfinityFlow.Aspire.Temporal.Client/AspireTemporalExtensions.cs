using System.Diagnostics.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;
using Temporalio.Client;
using Temporalio.Client.Interceptors;
using Temporalio.Extensions.DiagnosticSource;
using Temporalio.Extensions.Hosting;
using Temporalio.Extensions.OpenTelemetry;
using Temporalio.Runtime;

namespace Aspire.Hosting;

/// <summary>
/// Provides extension methods for adding Temporal client to an Aspire application with automatic OpenTelemetry configuration.
/// </summary>
public static class AspireTemporalExtensions
{
    /// <summary>
    /// Adds Temporal client to the service collection using the specified connection name.
    /// The connection string is automatically retrieved from Aspire configuration.
    /// Automatically configures OpenTelemetry tracing and metrics for the Aspire dashboard.
    /// Returns a builder that allows fluent configuration of interceptors and tracing sources.
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    /// <param name="connectionName">The name of the Temporal connection (default: "temporal").</param>
    /// <returns>A builder for fluent configuration of the Temporal client.</returns>
    public static TemporalClientBuilder AddTemporalClient(
        this IHostApplicationBuilder builder,
        string connectionName = "temporal")
    {
        ArgumentNullException.ThrowIfNull(builder);
        return new TemporalClientBuilder(builder, connectionName);
    }

    /// <summary>
    /// Adds a Temporal worker to the service collection using the specified connection name.
    /// The connection string is automatically retrieved from Aspire configuration.
    /// Automatically configures OpenTelemetry tracing and metrics for the Aspire dashboard.
    /// Returns a builder that allows fluent configuration of interceptors and tracing sources.
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    /// <param name="taskQueue">The task queue name for this worker.</param>
    /// <param name="connectionName">The name of the Temporal connection (default: "temporal").</param>
    /// <returns>A builder for fluent configuration of the Temporal worker.</returns>
    public static TemporalClientBuilder AddTemporalWorker(
        this IHostApplicationBuilder builder,
        string taskQueue,
        string connectionName = "temporal")
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(taskQueue);
        return new TemporalClientBuilder(builder, connectionName, taskQueue);
    }
}

/// <summary>
/// Builder for configuring Temporal client or worker with fluent API.
/// </summary>
public class TemporalClientBuilder
{
    private readonly IHostApplicationBuilder _builder;
    private readonly string _connectionName;
    private readonly string? _taskQueue;
    private readonly List<IClientInterceptor> _additionalInterceptors = new();
    private readonly List<string> _tracingSources = new() { "Temporalio.Client", "Temporalio.Workflows", "Temporalio.Activities" };
    private readonly List<Action<TemporalClientConnectOptions>> _configureActions = new();
    private bool _disableTracing;
    private bool _disableMetrics;
    private ITemporalWorkerServiceOptionsBuilder? _workerOptionsBuilder;
    private bool _registered;

    /// <summary>
    /// Gets the service collection for additional DI registration.
    /// Accessing this property triggers client registration if not already registered.
    /// </summary>
    public IServiceCollection Services
    {
        get
        {
            EnsureRegistered();
            return _builder.Services;
        }
    }

    /// <summary>
    /// Gets the worker options builder for adding activities and workflows.
    /// Only available when using AddTemporalWorker().
    /// Accessing this property triggers client registration if not already registered.
    /// </summary>
    public ITemporalWorkerServiceOptionsBuilder? Worker
    {
        get
        {
            EnsureRegistered();
            return _workerOptionsBuilder;
        }
    }

    internal TemporalClientBuilder(IHostApplicationBuilder builder, string connectionName, string? taskQueue = null)
    {
        _builder = builder;
        _connectionName = connectionName;
        _taskQueue = taskQueue;
    }

    private void EnsureRegistered()
    {
        if (!_registered)
        {
            RegisterClient();
            _registered = true;
        }
    }

    /// <summary>
    /// Configures additional interceptors for the Temporal client.
    /// The TracingInterceptor is added automatically unless WithoutTracing() is called.
    /// </summary>
    /// <param name="configure">Action to configure the interceptor list.</param>
    /// <returns>The builder for method chaining.</returns>
    public TemporalClientBuilder WithInterceptors(Action<IList<IClientInterceptor>> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        try
        {
            configure(_additionalInterceptors);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "An error occurred while configuring Temporal client interceptors. " +
                "Check your WithInterceptors callback for errors.",
                ex);
        }

        return this;
    }

    /// <summary>
    /// Configures the OpenTelemetry tracing sources for Temporal.
    /// By default includes: Temporalio.Client, Temporalio.Workflows, Temporalio.Activities.
    /// </summary>
    /// <param name="configure">Action to configure the tracing sources list.</param>
    /// <returns>The builder for method chaining.</returns>
    public TemporalClientBuilder WithTracingSources(Action<IList<string>> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        try
        {
            configure(_tracingSources);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "An error occurred while configuring Temporal tracing sources. " +
                "Check your WithTracingSources callback for errors.",
                ex);
        }

        return this;
    }

    /// <summary>
    /// Provides direct access to configure Temporal client options.
    /// </summary>
    /// <param name="configure">Action to configure the client options.</param>
    /// <returns>The builder for method chaining.</returns>
    public TemporalClientBuilder ConfigureOptions(Action<TemporalClientConnectOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        _configureActions.Add(configure);
        return this;
    }

    /// <summary>
    /// Disables automatic OpenTelemetry tracing configuration.
    /// </summary>
    /// <returns>The builder for method chaining.</returns>
    public TemporalClientBuilder WithoutTracing()
    {
        _disableTracing = true;
        return this;
    }

    /// <summary>
    /// Disables automatic OpenTelemetry metrics configuration.
    /// </summary>
    /// <returns>The builder for method chaining.</returns>
    public TemporalClientBuilder WithoutMetrics()
    {
        _disableMetrics = true;
        return this;
    }

    private void RegisterClient()
    {
        var connectionString = _builder.Configuration.GetConnectionString(_connectionName);

        if (string.IsNullOrEmpty(connectionString))
        {
            var availableConnections = _builder.Configuration.AsEnumerable()
                .Where(kvp => kvp.Key.StartsWith("ConnectionStrings:") && !string.IsNullOrEmpty(kvp.Value))
                .Select(kvp => kvp.Key.Replace("ConnectionStrings:", ""))
                .ToList();

            var availableInfo = availableConnections.Any()
                ? $" Available connection names: {string.Join(", ", availableConnections)}"
                : " No connection strings found in configuration.";

            throw new InvalidOperationException(
                $"Temporal connection string '{_connectionName}' not found.{availableInfo} " +
                $"Ensure the Temporal resource is referenced in AppHost using .WithReference(temporal).");
        }

        // Create and register a meter for Temporal metrics as a singleton for proper disposal
        var meter = new Meter("Temporal.Client");
        _builder.Services.AddSingleton(meter);

        // Create Temporal runtime with OpenTelemetry configuration
        var runtime = new TemporalRuntime(new()
        {
            Telemetry = new()
            {
                Metrics = _disableMetrics ? null : new() { CustomMetricMeter = new CustomMetricMeter(meter) }
            }
        });

        // Register OpenTelemetry tracing sources
        _builder.Services.ConfigureOpenTelemetryTracerProvider(tracing =>
        {
            // Add sources from the mutable list (which can be modified via WithTracingSources)
            foreach (var source in _tracingSources)
            {
                if (string.IsNullOrWhiteSpace(source))
                {
                    throw new InvalidOperationException(
                        "Tracing source name cannot be null or whitespace. " +
                        "Check your WithTracingSources configuration.");
                }
                tracing.AddSource(source);
            }
        });

        // Register the client
        var serviceCollection = _builder.Services.AddTemporalClient(opts =>
        {
            opts.TargetHost = connectionString;
            opts.Runtime = runtime;

            // Build interceptor list
            var interceptors = new List<IClientInterceptor>();

            // Add tracing interceptor if not disabled
            if (!_disableTracing)
            {
                interceptors.Add(new TracingInterceptor());
            }

            // Add any additional interceptors from the mutable list
            interceptors.AddRange(_additionalInterceptors);

            opts.Interceptors = interceptors;

            // Apply all configuration actions
            foreach (var configure in _configureActions)
            {
                try
                {
                    configure(opts);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        "An error occurred while configuring Temporal client options. " +
                        "Check your ConfigureOptions callback for errors.",
                        ex);
                }
            }
        });

        // Add worker if task queue is specified
        if (_taskQueue is not null)
        {
            _workerOptionsBuilder = serviceCollection.AddHostedTemporalWorker(_taskQueue);
        }
    }
}
