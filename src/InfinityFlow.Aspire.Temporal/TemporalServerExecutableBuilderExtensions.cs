using Aspire.Hosting.ApplicationModel;

using InfinityFlow.Aspire.Temporal;

namespace Aspire.Hosting;

/// <summary>
/// Extension methods for adding Temporal server as an executable resource to an Aspire application.
/// </summary>
public static class TemporalServerExecutableBuilderExtensions
{
    /// <summary>
    /// Adds a Temporal server executable resource to the Aspire host with custom configuration.
    /// Requires the Temporal CLI executable to be in your PATH.
    /// </summary>
    /// <param name="builder">The distributed application builder.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="callback">Callback to configure the Temporal server options using a fluent builder.</param>
    /// <returns>A resource builder for the Temporal server executable.</returns>
    public static IResourceBuilder<TemporalServerExecutableResource> AddTemporalServerExecutable(this IDistributedApplicationBuilder builder, string name,
        Action<TemporalServerResourceBuilder> callback)
    {
        var rb = new TemporalServerResourceBuilder();
        callback(rb);
        var args = rb.Build();

        return builder.AddTemporalServerExecutable(name, args);
    }

    /// <summary>
    /// Adds a Temporal server executable resource to the Aspire host with default configuration.
    /// Requires the Temporal CLI executable to be in your PATH.
    /// </summary>
    /// <param name="builder">The distributed application builder.</param>
    /// <param name="name">The name of the resource.</param>
    /// <returns>A resource builder for the Temporal server executable.</returns>
    public static IResourceBuilder<TemporalServerExecutableResource> AddTemporalServerExecutable(this IDistributedApplicationBuilder builder, string name)
    {
        return builder.AddTemporalServerExecutable(name, new TemporalServerResourceArguments());
    }

    private static IResourceBuilder<TemporalServerExecutableResource> AddTemporalServerExecutable(this IDistributedApplicationBuilder builder, string name,
        TemporalServerResourceArguments args)
    {
        var resourceBuilder = builder.AddResource(new TemporalServerExecutableResource(name, args))
            .WithArgs(args.GetArgs());

        resourceBuilder.WithHttpEndpoint(port: args.Port, name: "server").AsHttp2Service();

        if (args.Headless is not true)
        {
            resourceBuilder.WithHttpEndpoint(port: args.UiPort, name: "ui");
        }

        if (args.MetricsPort is not null)
        {
            resourceBuilder.WithHttpEndpoint(port: args.MetricsPort, name: "metrics");
        }

        if (args.HttpPort is not null)
        {
            resourceBuilder.WithHttpEndpoint(port: args.HttpPort, name: "http");
        }

        return resourceBuilder;
    }
}
