using Microsoft.Extensions.DependencyInjection;
using Temporalio.Client;

namespace InfinityFlow.Aspire.Temporal.Client;

/// <summary>
/// Fluent builder for configuring the Temporal client registration.
/// </summary>
public sealed class TemporalClientBuilder
{
    internal TemporalClientBuilder(IServiceCollection services)
    {
        Services = services;
    }

    /// <summary>
    /// Gets the service collection being configured.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Applies additional configuration to the Temporal client connect options.
    /// </summary>
    /// <param name="configure">Action to configure client connect options.</param>
    /// <returns>This builder for chaining.</returns>
    public TemporalClientBuilder ConfigureOptions(Action<TemporalClientConnectOptions> configure)
    {
        Services.AddTemporalClient(configure);
        return this;
    }
}
