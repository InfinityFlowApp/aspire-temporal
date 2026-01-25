using Aspire.Hosting.ApplicationModel;

using InfinityFlow.Aspire.Temporal;

namespace Aspire.Hosting;

/// <summary>
/// Extension methods for adding Temporal server as a container resource to an Aspire application.
/// </summary>
public static class TemporalServerContainerBuilderExtensions
{
    internal const string TemporalServerImageName = "temporalio/admin-tools";
    internal const string TemporalServerImageTag = "1.28.2-tctl-1.18.1-cli-1.1.1";

    /// <summary>
    /// Adds a Temporal server container resource to the Aspire host with custom configuration.
    /// </summary>
    /// <param name="builder">The distributed application builder.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="callback">Callback to configure the Temporal server options using a fluent builder.</param>
    /// <returns>A resource builder for the Temporal server container.</returns>
    public static IResourceBuilder<TemporalServerContainerResource> AddTemporalServerContainer(this IDistributedApplicationBuilder builder, string name,
        Action<TemporalServerResourceBuilder> callback)
    {
        var rb = new TemporalServerResourceBuilder();
        callback(rb);
        var args = rb.Build();

        return builder.AddTemporalServerContainer(name, args);
    }

    /// <summary>
    /// Adds a Temporal server container resource to the Aspire host with default configuration.
    /// </summary>
    /// <param name="builder">The distributed application builder.</param>
    /// <param name="name">The name of the resource.</param>
    /// <returns>A resource builder for the Temporal server container.</returns>
    public static IResourceBuilder<TemporalServerContainerResource> AddTemporalServerContainer(this IDistributedApplicationBuilder builder, string name)
    {
        return builder.AddTemporalServerContainer(name, new TemporalServerResourceArguments());
    }
    private static IResourceBuilder<TemporalServerContainerResource> AddTemporalServerContainer(this IDistributedApplicationBuilder builder, string name, TemporalServerResourceArguments args)
    {
        var container = new TemporalServerContainerResource(name, args);

        var resourceBuilder = builder.AddResource(container)
                .WithImage(TemporalServerImageName, TemporalServerImageTag)
                .WithArgs(args.GetArgs())
                .WithEntrypoint("temporal")
                .WithHttpsEndpoint(name: "server", port: args.Port, targetPort: 7233).AsHttp2Service(); // Internal port is always 7233

        if (args.Headless is not true)
        {
            resourceBuilder.WithHttpEndpoint(name: "ui", port: args.UiPort, targetPort: 8233); // Internal port is always 8233
        }

        if (args.MetricsPort is not null)
        {
            resourceBuilder.WithHttpEndpoint(name: "metrics", port: args.MetricsPort, targetPort: 7235); // Internal port is always 7235
        }

        if (args.HttpPort is not null)
        {
            resourceBuilder.WithHttpEndpoint(name: "http", port: args.HttpPort, targetPort: 7234); // Internal port is always 7234
        }

        return resourceBuilder;
    }
}
