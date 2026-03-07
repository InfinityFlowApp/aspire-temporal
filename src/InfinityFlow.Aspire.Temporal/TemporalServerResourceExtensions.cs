using Aspire.Hosting.ApplicationModel;
using InfinityFlow.Aspire.Temporal;
using InfinityFlow.Aspire.Temporal.Annotations;

namespace Aspire.Hosting;

/// <summary>
/// Extension methods for configuring Temporal server resources.
/// </summary>
public static class TemporalServerResourceExtensions
{
    /// <summary>Sets the log format for the Temporal server.</summary>
    public static IResourceBuilder<T> WithLogFormat<T>(this IResourceBuilder<T> builder, LogFormat format)
        where T : ITemporalServerResource
    {
        builder.Resource.Annotations.Add(new TemporalLogFormatAnnotation(format));
        return builder;
    }

    /// <summary>Sets the log level for the Temporal server.</summary>
    public static IResourceBuilder<T> WithLogLevel<T>(this IResourceBuilder<T> builder, LogLevel level)
        where T : ITemporalServerResource
    {
        builder.Resource.Annotations.Add(new TemporalLogLevelAnnotation(level));
        return builder;
    }

    /// <summary>Sets the database file name for the Temporal server.</summary>
    public static IResourceBuilder<T> WithDbFileName<T>(this IResourceBuilder<T> builder, string dbFileName)
        where T : ITemporalServerResource
    {
        builder.Resource.Annotations.Add(new TemporalDbFileNameAnnotation(dbFileName));
        return builder;
    }

    /// <summary>Sets the IP address for the Temporal server.</summary>
    public static IResourceBuilder<T> WithIp<T>(this IResourceBuilder<T> builder, string ip)
        where T : ITemporalServerResource
    {
        builder.Resource.Annotations.Add(new TemporalIpAnnotation(ip));
        return builder;
    }

    /// <summary>Sets the UI IP address for the Temporal server.</summary>
    public static IResourceBuilder<T> WithUiIp<T>(this IResourceBuilder<T> builder, string uiIp)
        where T : ITemporalServerResource
    {
        builder.Resource.Annotations.Add(new TemporalUiIpAnnotation(uiIp));
        return builder;
    }

    /// <summary>Sets the UI asset path for the Temporal server.</summary>
    public static IResourceBuilder<T> WithUiAssetPath<T>(this IResourceBuilder<T> builder, string assetPath)
        where T : ITemporalServerResource
    {
        builder.Resource.Annotations.Add(new TemporalUiAssetPathAnnotation(assetPath));
        return builder;
    }

    /// <summary>Sets the UI codec endpoint for the Temporal server.</summary>
    public static IResourceBuilder<T> WithUiCodecEndpoint<T>(this IResourceBuilder<T> builder, string codecEndpoint)
        where T : ITemporalServerResource
    {
        builder.Resource.Annotations.Add(new TemporalUiCodecEndpointAnnotation(codecEndpoint));
        return builder;
    }

    /// <summary>Sets the SQLite pragma for the Temporal server.</summary>
    public static IResourceBuilder<T> WithSQLitePragma<T>(this IResourceBuilder<T> builder, SQLitePragma pragma)
        where T : ITemporalServerResource
    {
        builder.Resource.Annotations.Add(new TemporalSQLitePragmaAnnotation(pragma));
        return builder;
    }

    /// <summary>Adds one or more namespaces to the Temporal server.</summary>
    public static IResourceBuilder<T> WithNamespace<T>(this IResourceBuilder<T> builder, params string[] namespaces)
        where T : ITemporalServerResource
    {
        foreach (var ns in namespaces)
            builder.Resource.Annotations.Add(new TemporalNamespaceAnnotation(ns));
        return builder;
    }

    /// <summary>Adds a dynamic config value to the Temporal server.</summary>
    public static IResourceBuilder<T> WithDynamicConfigValue<T>(this IResourceBuilder<T> builder, string key, object value)
        where T : ITemporalServerResource
    {
        builder.Resource.Annotations.Add(new TemporalDynamicConfigAnnotation(key, value));
        return builder;
    }

    /// <summary>Enables logging server configuration to stderr.</summary>
    public static IResourceBuilder<T> WithLogConfig<T>(this IResourceBuilder<T> builder, bool enabled = true)
        where T : ITemporalServerResource
    {
        builder.Resource.Annotations.Add(new TemporalLogConfigAnnotation(enabled));
        return builder;
    }

    /// <summary>Registers a custom search attribute at server launch.</summary>
    public static IResourceBuilder<T> WithSearchAttribute<T>(this IResourceBuilder<T> builder, string key, SearchAttributeType type)
        where T : ITemporalServerResource
    {
        builder.Resource.Annotations.Add(new TemporalSearchAttributeAnnotation(key, type));
        return builder;
    }

    /// <summary>Sets the public base path for the Web UI.</summary>
    public static IResourceBuilder<T> WithUiPublicPath<T>(this IResourceBuilder<T> builder, string publicPath)
        where T : ITemporalServerResource
    {
        builder.Resource.Annotations.Add(new TemporalUiPublicPathAnnotation(publicPath));
        return builder;
    }

    // --- Container-specific endpoint methods ---

    /// <summary>Sets the external gRPC service port for the Temporal container.
    /// The internal container port is always 7233.</summary>
    public static IResourceBuilder<TemporalServerContainerResource> WithServicePort(
        this IResourceBuilder<TemporalServerContainerResource> builder, int port)
    {
        var endpoint = builder.Resource.Annotations.OfType<EndpointAnnotation>()
            .Single(e => e.Name == "server");
        endpoint.Port = port;
        return builder;
    }

    /// <summary>Sets the external UI port for the Temporal container.
    /// The internal container port is always 8233.
    /// Has no effect if <see cref="WithHeadlessUi(IResourceBuilder{TemporalServerContainerResource})"/> was called first.</summary>
    public static IResourceBuilder<TemporalServerContainerResource> WithUiPort(
        this IResourceBuilder<TemporalServerContainerResource> builder, int port)
    {
        var endpoint = builder.Resource.Annotations.OfType<EndpointAnnotation>()
            .SingleOrDefault(e => e.Name == "ui");
        if (endpoint is not null)
            endpoint.Port = port;
        return builder;
    }

    /// <summary>Adds a metrics endpoint to the Temporal container.</summary>
    public static IResourceBuilder<TemporalServerContainerResource> WithMetricsEndpoint(
        this IResourceBuilder<TemporalServerContainerResource> builder, int? port = null)
    {
        builder.WithHttpEndpoint(name: "metrics", port: port, targetPort: 7235);
        return builder;
    }

    /// <summary>Adds an HTTP API endpoint to the Temporal container.</summary>
    public static IResourceBuilder<TemporalServerContainerResource> WithHttpPort(
        this IResourceBuilder<TemporalServerContainerResource> builder, int? port = null)
    {
        builder.WithHttpEndpoint(name: "http", port: port, targetPort: 7234);
        return builder;
    }

    /// <summary>Runs the Temporal container without the UI and removes the UI endpoint.</summary>
    public static IResourceBuilder<TemporalServerContainerResource> WithHeadlessUi(
        this IResourceBuilder<TemporalServerContainerResource> builder)
    {
        builder.Resource.Annotations.Add(new TemporalHeadlessAnnotation(true));
        var uiEndpoint = builder.Resource.Annotations.OfType<EndpointAnnotation>()
            .SingleOrDefault(e => e.Name == "ui");
        if (uiEndpoint is not null)
            builder.Resource.Annotations.Remove(uiEndpoint);
        return builder;
    }

    // --- Executable-specific endpoint methods ---

    /// <summary>Sets the gRPC service port for the Temporal executable.</summary>
    public static IResourceBuilder<TemporalServerExecutableResource> WithServicePort(
        this IResourceBuilder<TemporalServerExecutableResource> builder, int port)
    {
        var endpoint = builder.Resource.Annotations.OfType<EndpointAnnotation>()
            .Single(e => e.Name == "server");
        endpoint.Port = port;
        return builder;
    }

    /// <summary>Sets the UI port for the Temporal executable.
    /// Has no effect if <see cref="WithHeadlessUi(IResourceBuilder{TemporalServerExecutableResource})"/> was called first.</summary>
    public static IResourceBuilder<TemporalServerExecutableResource> WithUiPort(
        this IResourceBuilder<TemporalServerExecutableResource> builder, int port)
    {
        var endpoint = builder.Resource.Annotations.OfType<EndpointAnnotation>()
            .SingleOrDefault(e => e.Name == "ui");
        if (endpoint is not null)
            endpoint.Port = port;
        return builder;
    }

    /// <summary>Adds a metrics endpoint to the Temporal executable.</summary>
    public static IResourceBuilder<TemporalServerExecutableResource> WithMetricsEndpoint(
        this IResourceBuilder<TemporalServerExecutableResource> builder, int? port = null)
    {
        builder.WithHttpEndpoint(name: "metrics", port: port);
        return builder;
    }

    /// <summary>Adds an HTTP API endpoint to the Temporal executable.</summary>
    public static IResourceBuilder<TemporalServerExecutableResource> WithHttpPort(
        this IResourceBuilder<TemporalServerExecutableResource> builder, int? port = null)
    {
        builder.WithHttpEndpoint(name: "http", port: port);
        return builder;
    }

    /// <summary>Runs the Temporal executable without the UI and removes the UI endpoint.</summary>
    public static IResourceBuilder<TemporalServerExecutableResource> WithHeadlessUi(
        this IResourceBuilder<TemporalServerExecutableResource> builder)
    {
        builder.Resource.Annotations.Add(new TemporalHeadlessAnnotation(true));
        var uiEndpoint = builder.Resource.Annotations.OfType<EndpointAnnotation>()
            .SingleOrDefault(e => e.Name == "ui");
        if (uiEndpoint is not null)
            builder.Resource.Annotations.Remove(uiEndpoint);
        return builder;
    }
}
