using Aspire.Hosting.ApplicationModel;
using InfinityFlow.Aspire.Temporal;

namespace Aspire.Hosting;

/// <summary>
/// Extension methods for adding Temporal server to an Aspire application with a simplified fluent API.
/// </summary>
public static class TemporalBuilderExtensions
{
    internal const string TemporalImageName = "temporalio/admin-tools";
    internal const string TemporalImageTag = "1.28.2-tctl-1.18.1-cli-1.1.1";

    /// <summary>
    /// Adds a Temporal server to the Aspire application.
    /// By default, uses a container. Call .WithExecutable() to use a local executable instead.
    /// </summary>
    /// <param name="builder">The distributed application builder.</param>
    /// <param name="name">The name of the resource.</param>
    /// <returns>A builder for configuring the Temporal server.</returns>
    public static TemporalResourceBuilder AddTemporalServer(
        this IDistributedApplicationBuilder builder,
        string name)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new TemporalResourceBuilder(builder, name);
    }

    /// <summary>
    /// Adds a reference to a Temporal server resource.
    /// </summary>
    /// <typeparam name="TDestination">The destination resource type.</typeparam>
    /// <param name="builder">The resource builder.</param>
    /// <param name="temporalBuilder">The Temporal resource builder.</param>
    /// <returns>The resource builder for method chaining.</returns>
    public static IResourceBuilder<TDestination> WithReference<TDestination>(
        this IResourceBuilder<TDestination> builder,
        TemporalResourceBuilder temporalBuilder)
        where TDestination : IResourceWithEnvironment
    {
        return builder.WithReference(temporalBuilder.Resource());
    }
}

/// <summary>
/// Builder for configuring a Temporal server resource.
/// Supports both container (default) and executable modes.
/// </summary>
public sealed class TemporalResourceBuilder
{
    private readonly IDistributedApplicationBuilder _builder;
    private readonly string _name;
    private readonly TemporalServerResourceArguments _args = new();
    private bool _useExecutable;
    private string? _executableCommand;
    private string? _executableWorkingDirectory;
    private IResourceBuilder<IResourceWithConnectionString>? _resourceBuilder;

    internal TemporalResourceBuilder(IDistributedApplicationBuilder builder, string name)
    {
        _builder = builder;
        _name = name;
    }

    /// <summary>
    /// Configures the Temporal server to run as a local executable instead of a container.
    /// </summary>
    /// <param name="command">The command to execute. Defaults to "temporal.exe" on Windows or "temporal" on Linux/macOS.</param>
    /// <param name="workingDirectory">The working directory for the executable. Defaults to empty string.</param>
    /// <returns>The builder for method chaining.</returns>
    public TemporalResourceBuilder WithExecutable(string? command = null, string? workingDirectory = null)
    {
        ThrowIfAlreadyBuilt();

        _useExecutable = true;
        _executableCommand = command ?? GetDefaultExecutableCommand();
        _executableWorkingDirectory = workingDirectory ?? string.Empty;
        return this;
    }

    /// <summary>
    /// Sets the command to execute.
    /// </summary>
    /// <param name="command">The command name.</param>
    /// <returns>The builder for method chaining.</returns>
    public TemporalResourceBuilder WithCommand(string command)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(command);
        ThrowIfAlreadyBuilt();
        _args.Command = command;
        return this;
    }

    /// <summary>
    /// Sets the file in which to persist Temporal state.
    /// By default, workflows are lost when the process dies.
    /// </summary>
    /// <param name="dbFileName">The database file path.</param>
    /// <returns>The builder for method chaining.</returns>
    public TemporalResourceBuilder WithDbFileName(string dbFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dbFileName);
        ThrowIfAlreadyBuilt();
        _args.DbFileName = dbFileName;
        return this;
    }

    /// <summary>
    /// Configures the port for the frontend gRPC service endpoint.
    /// If not set, Aspire will allocate a random available port.
    /// </summary>
    /// <param name="port">The port number (1-65535), or null for dynamic allocation.</param>
    /// <returns>The builder for method chaining.</returns>
    public TemporalResourceBuilder WithServiceEndpoint(int? port = null)
    {
        if (port.HasValue && (port.Value < 1 || port.Value > 65535))
        {
            throw new ArgumentOutOfRangeException(nameof(port), port.Value,
                "Port must be between 1 and 65535.");
        }

        ThrowIfAlreadyBuilt();
        _args.Port = port;
        return this;
    }

    /// <summary>
    /// Configures the port for the Web UI endpoint.
    /// If not set, Aspire will allocate a random available port.
    /// </summary>
    /// <param name="port">The port number (1-65535), or null for dynamic allocation.</param>
    /// <returns>The builder for method chaining.</returns>
    public TemporalResourceBuilder WithUiEndpoint(int? port = null)
    {
        if (port.HasValue && (port.Value < 1 || port.Value > 65535))
        {
            throw new ArgumentOutOfRangeException(nameof(port), port.Value,
                "Port must be between 1 and 65535.");
        }

        ThrowIfAlreadyBuilt();
        _args.UiPort = port;
        _args.Headless = false;
        return this;
    }

    /// <summary>
    /// Configures the port for the metrics endpoint.
    /// </summary>
    /// <param name="port">The port number (1-65535).</param>
    /// <returns>The builder for method chaining.</returns>
    public TemporalResourceBuilder WithMetricsEndpoint(int port)
    {
        if (port < 1 || port > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(port), port,
                "Port must be between 1 and 65535.");
        }

        ThrowIfAlreadyBuilt();
        _args.MetricsPort = port;
        return this;
    }

    /// <summary>
    /// Configures the port for the HTTP endpoint.
    /// </summary>
    /// <param name="port">The port number (1-65535).</param>
    /// <returns>The builder for method chaining.</returns>
    public TemporalResourceBuilder WithHttpEndpoint(int port)
    {
        if (port < 1 || port > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(port), port,
                "Port must be between 1 and 65535.");
        }

        ThrowIfAlreadyBuilt();
        _args.HttpPort = port;
        return this;
    }

    /// <summary>
    /// Disables the Web UI (runs in headless mode).
    /// </summary>
    /// <returns>The builder for method chaining.</returns>
    public TemporalResourceBuilder WithoutUi()
    {
        ThrowIfAlreadyBuilt();
        _args.Headless = true;
        _args.UiPort = null;
        return this;
    }

    /// <summary>
    /// Sets the IPv4 address to bind the frontend service to.
    /// </summary>
    /// <param name="ip">The IP address (default: "0.0.0.0").</param>
    /// <returns>The builder for method chaining.</returns>
    public TemporalResourceBuilder WithIp(string ip)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ip);
        ThrowIfAlreadyBuilt();
        _args.Ip = ip;
        return this;
    }

    /// <summary>
    /// Sets the IPv4 address to bind the Web UI to.
    /// </summary>
    /// <param name="uiIp">The IP address.</param>
    /// <returns>The builder for method chaining.</returns>
    public TemporalResourceBuilder WithUiIp(string uiIp)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uiIp);
        ThrowIfAlreadyBuilt();
        _args.UiIp = uiIp;
        return this;
    }

    /// <summary>
    /// Sets the UI custom assets path.
    /// </summary>
    /// <param name="assetsPath">The assets path.</param>
    /// <returns>The builder for method chaining.</returns>
    public TemporalResourceBuilder WithUiAssetsPath(string assetsPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assetsPath);
        ThrowIfAlreadyBuilt();
        _args.UiAssetPath = assetsPath;
        return this;
    }

    /// <summary>
    /// Sets the UI remote codec HTTP endpoint.
    /// </summary>
    /// <param name="codecEndpoint">The codec endpoint URL.</param>
    /// <returns>The builder for method chaining.</returns>
    public TemporalResourceBuilder WithUiCodecEndpoint(string codecEndpoint)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(codecEndpoint);
        ThrowIfAlreadyBuilt();
        _args.UiCodecEndpoint = codecEndpoint;
        return this;
    }

    /// <summary>
    /// Sets the log format.
    /// </summary>
    /// <param name="format">The log format (json or pretty).</param>
    /// <returns>The builder for method chaining.</returns>
    public TemporalResourceBuilder WithLogFormat(LogFormat format)
    {
        ThrowIfAlreadyBuilt();
        _args.LogFormat = format;
        return this;
    }

    /// <summary>
    /// Sets the log level.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <returns>The builder for method chaining.</returns>
    public TemporalResourceBuilder WithLogLevel(LogLevel level)
    {
        ThrowIfAlreadyBuilt();
        _args.LogLevel = level;
        return this;
    }

    /// <summary>
    /// Sets SQLite pragma statements.
    /// </summary>
    /// <param name="pragma">The SQLite pragma.</param>
    /// <returns>The builder for method chaining.</returns>
    public TemporalResourceBuilder WithSQLitePragma(SQLitePragma pragma)
    {
        ThrowIfAlreadyBuilt();
        _args.SQLitePragma = pragma;
        return this;
    }

    /// <summary>
    /// Specifies namespaces that should be pre-created.
    /// The "default" namespace is always created.
    /// </summary>
    /// <param name="namespaces">The namespace names.</param>
    /// <returns>The builder for method chaining.</returns>
    public TemporalResourceBuilder WithNamespace(params string[] namespaces)
    {
        ArgumentNullException.ThrowIfNull(namespaces);

        foreach (var ns in namespaces)
        {
            if (string.IsNullOrWhiteSpace(ns))
            {
                throw new ArgumentException(
                    "Namespace names cannot be null or whitespace. " +
                    "Check your WithNamespace call for empty or null values.",
                    nameof(namespaces));
            }
        }

        ThrowIfAlreadyBuilt();
        _args.Namespaces.AddRange(namespaces);
        return this;
    }

    /// <summary>
    /// Specifies a dynamic config value.
    /// </summary>
    /// <param name="key">The config key.</param>
    /// <param name="value">The config value.</param>
    /// <returns>The builder for method chaining.</returns>
    public TemporalResourceBuilder WithDynamicConfigValue(string key, object value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);

        ThrowIfAlreadyBuilt();
        _args.DynamicConfigValues.Add(key, value);
        return this;
    }

    /// <summary>
    /// Builds and returns the resource builder for the Temporal server.
    /// Call this method when you need to pass the resource to methods like WithReference().
    /// </summary>
    /// <returns>A resource builder for the Temporal server.</returns>
    public IResourceBuilder<IResourceWithConnectionString> Resource()
    {
        if (_resourceBuilder is not null)
        {
            return _resourceBuilder;
        }

        _resourceBuilder = _useExecutable
            ? BuildExecutable()
            : BuildContainer();

        return _resourceBuilder;
    }

    private IResourceBuilder<IResourceWithConnectionString> BuildContainer()
    {
        var resource = new TemporalServerContainerResource(_name, _args);

        var resourceBuilder = _builder.AddResource(resource)
            .WithImage(TemporalBuilderExtensions.TemporalImageName, TemporalBuilderExtensions.TemporalImageTag)
            .WithArgs(_args.GetArgs())
            .WithEntrypoint("temporal")
            .WithHttpsEndpoint(name: "server", port: _args.Port, targetPort: 7233)
            .AsHttp2Service();

        if (_args.Headless is not true)
        {
            resourceBuilder.WithHttpEndpoint(name: "ui", port: _args.UiPort, targetPort: 8233);
        }

        if (_args.MetricsPort is not null)
        {
            resourceBuilder.WithHttpEndpoint(name: "metrics", port: _args.MetricsPort, targetPort: 7235);
        }

        if (_args.HttpPort is not null)
        {
            resourceBuilder.WithHttpEndpoint(name: "http", port: _args.HttpPort, targetPort: 7234);
        }

        return resourceBuilder;
    }

    private IResourceBuilder<IResourceWithConnectionString> BuildExecutable()
    {
        var command = _executableCommand ?? GetDefaultExecutableCommand();
        _args.Command = command;

        var resource = new TemporalServerExecutableResource(_name, _args, command, _executableWorkingDirectory ?? string.Empty);

        var resourceBuilder = _builder.AddResource(resource)
            .WithArgs(_args.GetArgs())
            .WithHttpEndpoint(port: _args.Port, name: "server")
            .AsHttp2Service();

        if (_args.Headless is not true)
        {
            resourceBuilder.WithHttpEndpoint(port: _args.UiPort, name: "ui");
        }

        if (_args.MetricsPort is not null)
        {
            resourceBuilder.WithHttpEndpoint(port: _args.MetricsPort, name: "metrics");
        }

        if (_args.HttpPort is not null)
        {
            resourceBuilder.WithHttpEndpoint(port: _args.HttpPort, name: "http");
        }

        return resourceBuilder;
    }

    private static string GetDefaultExecutableCommand()
    {
        return OperatingSystem.IsWindows() ? "temporal.exe" : "temporal";
    }

    private void ThrowIfAlreadyBuilt()
    {
        if (_resourceBuilder is not null)
        {
            throw new InvalidOperationException(
                "Cannot modify Temporal resource configuration after it has been built. " +
                "Ensure all configuration methods are called before the resource is used.");
        }
    }
}

