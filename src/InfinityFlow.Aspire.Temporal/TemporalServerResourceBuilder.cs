using InfinityFlow.Aspire.Temporal;

namespace Aspire.Hosting;

public class TemporalServerResourceBuilder
{
    private TemporalServerResourceArguments Args { get; } = new();

    /// <summary>
    /// Command to execute (default: "temporal")
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public TemporalServerResourceBuilder WithCommand(string command = "temporal")
    {
        Args.Command = command;
        return this;
    }

    /// <summary>
    /// File in which to persist Temporal state (by default, Workflows are lost when the process dies)
    /// </summary>
    /// <returns></returns>
    public TemporalServerResourceBuilder WithDbFileName(string dbFileName)
    {
        Args.DbFileName = dbFileName;
        return this;
    }

    /// <summary>
    /// Port for the frontend gRPC service (default: 7233)
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    public TemporalServerResourceBuilder WithPort(int port)
    {
        Args.Port = port;
        return this;
    }

    /// <summary>
    /// Port for the frontend HTTP service (default: disabled)
    /// </summary>
    /// <param name="httpPort"></param>
    /// <returns></returns>
    public TemporalServerResourceBuilder WithHttpPort(int httpPort)
    {
        Args.HttpPort = httpPort;
        return this;
    }

    /// <summary>
    /// Port for /metrics (default: disabled)
    /// </summary>
    /// <param name="metricsPort"></param>
    /// <returns></returns>
    public TemporalServerResourceBuilder WithMetricsPort(int metricsPort)
    {
        Args.MetricsPort = metricsPort;
        return this;
    }

    /// <summary>
    /// Port for the Web UI (default: --port + 1000, e.g. 8233)
    /// </summary>
    /// <param name="uiPort"></param>
    /// <returns></returns>
    public TemporalServerResourceBuilder WithUiPort(int uiPort)
    {
        Args.UiPort = uiPort;
        return this;
    }

    /// <summary>
    /// Disable the Web UI(default: false)
    /// </summary>
    /// <param name="headless"></param>
    /// <returns></returns>
    public TemporalServerResourceBuilder WithHeadlessUi(bool headless)
    {
        Args.Headless = headless;
        return this;
    }

    /// <summary>
    /// IPv4 address to bind the frontend service to (default: "127.0.0.1")
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    public TemporalServerResourceBuilder WithIp(string ip)
    {
        Args.Ip = ip;
        return this;
    }

    /// <summary>
    /// IPv4 address to bind the Web UI to (default: same as --ip)
    /// </summary>
    /// <param name="uiIp"></param>
    /// <returns></returns>
    public TemporalServerResourceBuilder WithUiIp(string uiIp)
    {
        Args.UiIp = uiIp;
        return this;
    }

    /// <summary>
    /// UI custom assets path
    /// </summary>
    /// <param name="assetsPath"></param>
    /// <returns></returns>
    public TemporalServerResourceBuilder WithUiAssetsPath(string assetsPath)
    {
        Args.UiAssetPath = assetsPath;
        return this;
    }

    /// <summary>
    /// UI remote codec HTTP endpoint
    /// </summary>
    /// <param name="codecEndpoint"></param>
    /// <returns></returns>
    public TemporalServerResourceBuilder WithUiCodecEndpoint(string codecEndpoint)
    {
        Args.UiCodecEndpoint = codecEndpoint;
        return this;
    }

    /// <summary>
    /// Set the log formatting. Options: ["json", "pretty"]. (default: "json")
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public TemporalServerResourceBuilder WithLogFormat(LogFormat format)
    {
        Args.LogFormat = format;
        return this;
    }

    /// <summary>
    /// Set the log level. Options: ["debug" "info" "warn" "error" "fatal"]. (default: "info")
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public TemporalServerResourceBuilder WithLogLevel(LogLevel level)
    {
        Args.LogLevel = level;
        return this;
    }

    /// <summary>
    /// Specify SQLite pragma statements in pragma=value format. Pragma options: ["journal_mode" "synchronous"]
    /// </summary>
    /// <param name="pragma"></param>
    /// <returns></returns>
    public TemporalServerResourceBuilder WithSQLitePragma(SQLitePragma pragma)
    {
        Args.SQLitePragma = pragma;
        return this;
    }

    /// <summary>
    /// Specify namespaces that should be pre-created (namespace "default" is always created)
    /// </summary>
    /// <param name="namespaces"></param>
    /// <returns></returns>
    public TemporalServerResourceBuilder WithNamespace(params string[] namespaces)
    {
        Args.Namespaces.AddRange(namespaces);
        return this;
    }

    public TemporalServerResourceArguments Build() => Args;

    /// <summary>
    /// Specify dynamic config values that should be configured.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public TemporalServerResourceBuilder WithDynamicConfigValue(string key, object value)
    {
        Args.DynamicConfigValues.Add(key, value);
        return this;
    }
}
