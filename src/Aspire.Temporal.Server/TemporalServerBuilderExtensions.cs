using System.Net.Sockets;

namespace Aspire.Temporal.Server;

public static class TemporalServerBuilderExtensions
{
    /// <summary>
    /// Adds a temporal server resource instance to the Aspire host. Requires the Temporal executable location to be in your path.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static IResourceBuilder<TemporalServerExecutableResource> AddTemporalServerExecutable(this IDistributedApplicationBuilder builder, string name,
        Action<TemporalServerExecutableResourceBuilder> callback)
    {
        var rb = new TemporalServerExecutableResourceBuilder();
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
        return builder.AddTemporalServerExecutable(name, new TemporalServerExecutableResourceArguments());
    }

    private static IResourceBuilder<TemporalServerExecutableResource> AddTemporalServerExecutable(this IDistributedApplicationBuilder builder, string name,
        TemporalServerExecutableResourceArguments args)
    {
        const int DefaultPort = 7233;

        var port = args.Port ?? DefaultPort;
        var uiPort = args.UiPort ?? port + 1000;

        var resourceBuilder = builder.AddResource(new TemporalServerExecutableResource(name, args))
            .WithAnnotation(new ServiceBindingAnnotation(protocol: ProtocolType.Tcp, name: "server", uriScheme: "http", port: port, isExternal: true));


        if (args.Headless is not true)
        {
            resourceBuilder.WithAnnotation(new ServiceBindingAnnotation(protocol: ProtocolType.Tcp, name: "ui", uriScheme: "http", port: uiPort, isExternal: true));
        }

        if (args.MetricsPort is not null)
        {
            resourceBuilder.WithAnnotation(new ServiceBindingAnnotation(protocol: ProtocolType.Tcp, name: "metrics", uriScheme: "http", port: args.MetricsPort, isExternal: true));
        }

        if (args.HttpPort is not null)
        {
            resourceBuilder.WithAnnotation(new ServiceBindingAnnotation(protocol: ProtocolType.Tcp, name: "http", uriScheme: "http", port: args.HttpPort, isExternal: true));
        }

        return resourceBuilder;
    }
}