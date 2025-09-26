using Aspire.Hosting.ApplicationModel;

using InfinityFlow.Aspire.Temporal;

namespace Aspire.Hosting;
public static class TemporalServerContainerBuilderExtensions
{
    /// <summary>
    /// Adds a temporal server resource instance to the Aspire host. Requires the Temporal Container location to be in your path.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static Task<IResourceBuilder<TemporalServerContainerResource>> AddTemporalServerContainer(this IDistributedApplicationBuilder builder, string name,
        Action<TemporalServerResourceBuilder> callback)
    {
        var rb = new TemporalServerResourceBuilder();
        callback(rb);
        var args = rb.Build();

        return builder.AddTemporalServerContainer(name, args);
    }

    /// <summary>
    /// Adds a temporal server resource instance to the Aspire host. Requires the Temporal Container location to be in your path.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Task<IResourceBuilder<TemporalServerContainerResource>> AddTemporalServerContainer(this IDistributedApplicationBuilder builder, string name)
    {
        return builder.AddTemporalServerContainer(name, new TemporalServerResourceArguments());
    }
    private static async Task<IResourceBuilder<TemporalServerContainerResource>> AddTemporalServerContainer(this IDistributedApplicationBuilder builder, string name, TemporalServerResourceArguments args)
    {
        var container = new TemporalServerContainerResource(name, args);

        var resourceBuilder = builder.AddResource(container)
                .WithAnnotation(new ContainerImageAnnotation() { Image = "temporalio/admin-tools", Tag = "latest" })
                .WithArgs(args.GetArgs())
                .WithEntrypoint("temporal")
                .WithHttpsEndpoint(name: "server", port: args.Port, targetPort: args.Port).AsHttp2Service(); // Internal port is always 7233

        if (args.Headless is not true)
        {
            resourceBuilder.WithHttpEndpoint(name: "ui", port: args.UiPort, targetPort: args.UiPort); // Internal port is always 8233
        }

        if (args.MetricsPort is not null)
        {
            resourceBuilder.WithHttpEndpoint(name: "metrics", port: args.MetricsPort, targetPort: args.MetricsPort); // Internal port is always 7235
        }

        if (args.HttpPort is not null)
        {
            resourceBuilder.WithHttpEndpoint(name: "http", port: args.HttpPort, targetPort: args.HttpPort); // Internal port is always 7234
        }

        return resourceBuilder;
    }
}
