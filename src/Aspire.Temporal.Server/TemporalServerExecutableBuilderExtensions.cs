namespace Aspire.Temporal.Server;

public static class TemporalServerExecutableBuilderExtensions
{
    /// <summary>
    /// Adds a temporal server resource instance to the Aspire host. Requires the Temporal executable location to be in your path.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static IResourceBuilder<TemporalServerExecutableResource> AddTemporalServerExecutable(this IDistributedApplicationBuilder builder, string name,
        Action<TemporalServerResourceBuilder> callback)
    {
        var rb = new TemporalServerResourceBuilder();
        callback(rb);
        var args = rb.Build();

        return builder.AddTemporalServerExecutable(name, args);
    }

    /// <summary>
    /// Adds a temporal server resource instance to the Aspire host. Requires the Temporal executable location to be in your path.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IResourceBuilder<TemporalServerExecutableResource> AddTemporalServerExecutable(this IDistributedApplicationBuilder builder, string name)
    {
        return builder.AddTemporalServerExecutable(name, new TemporalServerResourceArguments());
    }

    private static IResourceBuilder<TemporalServerExecutableResource> AddTemporalServerExecutable(this IDistributedApplicationBuilder builder, string name,
        TemporalServerResourceArguments args)
    {
        var resourceBuilder = builder.AddResource(new TemporalServerExecutableResource(name, args));

        resourceBuilder.WithHttpEndpoint(hostPort: args.Port, name: "server").AsHttp2Service();

        if (args.Headless is not true)
        {
            resourceBuilder.WithHttpEndpoint(hostPort: args.UiPort ?? args.Port + 1000, name: "ui");
        }

        if (args.MetricsPort is not null)
        {
            resourceBuilder.WithHttpEndpoint(hostPort: args.MetricsPort?? 9000, name: "metrics");
        }

        if (args.HttpPort is not null)
        {
            resourceBuilder.WithHttpEndpoint(hostPort: args.HttpPort ?? 8080, name: "http");
        }

        return resourceBuilder;
    }
}
