using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Temporalio.Client;
using Temporalio.Extensions.Hosting;

namespace Aspire.Hosting;

/// <summary>
/// Provides extension methods for adding Temporal client to an Aspire application.
/// </summary>
public static class AspireTemporalExtensions
{
    /// <summary>
    /// Adds Temporal client to the service collection using the specified connection name.
    /// The connection string is automatically retrieved from Aspire configuration.
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    /// <param name="connectionName">The name of the Temporal connection (default: "temporal").</param>
    /// <param name="configureClient">Optional callback to configure the Temporal client options.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddTemporalClient(
        this IHostApplicationBuilder builder,
        string connectionName = "temporal",
        Action<TemporalClientConnectOptions>? configureClient = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var connectionString = builder.Configuration.GetConnectionString(connectionName);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                $"Temporal connection string '{connectionName}' not found. " +
                $"Ensure the Temporal resource is referenced in AppHost using .WithReference(temporal).");
        }

        return builder.Services.AddTemporalClient(opts =>
        {
            opts.TargetHost = connectionString;
            configureClient?.Invoke(opts);
        });
    }

    /// <summary>
    /// Adds a Temporal worker to the service collection using the specified connection name.
    /// The connection string is automatically retrieved from Aspire configuration.
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    /// <param name="taskQueue">The task queue name for this worker.</param>
    /// <param name="connectionName">The name of the Temporal connection (default: "temporal").</param>
    /// <param name="configureClient">Optional callback to configure the Temporal client options.</param>
    /// <returns>The service collection for adding activities and workflows.</returns>
    public static IServiceCollection AddTemporalWorker(
        this IHostApplicationBuilder builder,
        string taskQueue,
        string connectionName = "temporal",
        Action<TemporalClientConnectOptions>? configureClient = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(taskQueue);

        var connectionString = builder.Configuration.GetConnectionString(connectionName);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                $"Temporal connection string '{connectionName}' not found. " +
                $"Ensure the Temporal resource is referenced in AppHost using .WithReference(temporal).");
        }

        builder.Services
            .AddTemporalClient(opts =>
            {
                opts.TargetHost = connectionString;
                configureClient?.Invoke(opts);
            })
            .AddHostedTemporalWorker(taskQueue);

        return builder.Services;
    }
}
