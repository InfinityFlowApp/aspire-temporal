using Microsoft.Extensions.DependencyInjection;
using Temporalio.Extensions.Hosting;

namespace InfinityFlow.Aspire.Temporal.Client;

/// <summary>
/// Fluent builder for configuring a hosted Temporal worker.
/// </summary>
public sealed class TemporalWorkerBuilder
{
    internal TemporalWorkerBuilder(ITemporalWorkerServiceOptionsBuilder workerOptionsBuilder, IServiceCollection services)
    {
        WorkerOptionsBuilder = workerOptionsBuilder;
        Services = services;
    }

    internal ITemporalWorkerServiceOptionsBuilder WorkerOptionsBuilder { get; }

    /// <summary>
    /// Gets the service collection being configured.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Registers a workflow type with the worker.
    /// </summary>
    /// <typeparam name="T">The workflow type.</typeparam>
    /// <returns>This builder for chaining.</returns>
    public TemporalWorkerBuilder AddWorkflow<T>()
    {
        WorkerOptionsBuilder.AddWorkflow<T>();
        return this;
    }

    /// <summary>
    /// Registers scoped activities with the worker.
    /// </summary>
    /// <typeparam name="T">The activities type.</typeparam>
    /// <returns>This builder for chaining.</returns>
    public TemporalWorkerBuilder AddScopedActivities<T>() where T : class
    {
        WorkerOptionsBuilder.AddScopedActivities<T>();
        return this;
    }

    /// <summary>
    /// Registers transient activities with the worker.
    /// </summary>
    /// <typeparam name="T">The activities type.</typeparam>
    /// <returns>This builder for chaining.</returns>
    public TemporalWorkerBuilder AddTransientActivities<T>() where T : class
    {
        WorkerOptionsBuilder.AddTransientActivities<T>();
        return this;
    }

    /// <summary>
    /// Registers singleton activities with the worker.
    /// </summary>
    /// <typeparam name="T">The activities type.</typeparam>
    /// <returns>This builder for chaining.</returns>
    public TemporalWorkerBuilder AddSingletonActivities<T>() where T : class
    {
        WorkerOptionsBuilder.AddSingletonActivities<T>();
        return this;
    }

    /// <summary>
    /// Registers activities with an existing instance.
    /// </summary>
    /// <typeparam name="T">The activities type.</typeparam>
    /// <param name="instance">The activity instance.</param>
    /// <returns>This builder for chaining.</returns>
    public TemporalWorkerBuilder AddActivitiesInstance<T>(T instance) where T : class
    {
        WorkerOptionsBuilder.AddActivitiesInstance(instance);
        return this;
    }
}
