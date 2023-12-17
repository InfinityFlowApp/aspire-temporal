namespace Aspire.Temporal.Server;

public class TemporalServerExecutableResourceBuilder
{
    private readonly TemporalServerExecutableResourceArguments args = new TemporalServerExecutableResourceArguments();

    /// <summary>
    /// File in which to persist Temporal state (by default, Workflows are lost when the process dies)
    /// </summary>
    /// <returns></returns>
    public TemporalServerExecutableResourceBuilder WithDbFileName(string dbFileName)
    {
        args.DbFileName = dbFileName;
        return this;
    }

    /// <summary>
    /// Port for the frontend gRPC service (default: 7233)
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    public TemporalServerExecutableResourceBuilder WithPort(int port)
    {
        args.Port = port;
        return this;
    }


    /// <summary>
    /// Port for the frontend HTTP service (default: disabled)
    /// </summary>
    /// <param name="httpPort"></param>
    /// <returns></returns>
    public TemporalServerExecutableResourceBuilder WithHttpPort(int httpPort)
    {
        args.HttpPort = httpPort;
        return this;
    }

    /// <summary>
    /// Port for /metrics (default: disabled)
    /// </summary>
    /// <param name="metricsPort"></param>
    /// <returns></returns>
    public TemporalServerExecutableResourceBuilder WithMetricsPort(int metricsPort)
    {
        args.MetricsPort = metricsPort;
        return this;
    }

    /// <summary>
    /// Port for the Web UI (default: --port + 1000, e.g. 8233)
    /// </summary>
    /// <param name="uiPort"></param>
    /// <returns></returns>
    public TemporalServerExecutableResourceBuilder WithUiPort(int uiPort)
    {
        args.UiPort = uiPort;
        return this;
    }

    /// <summary>
    /// Disable the Web UI(default: false)
    /// </summary>
    /// <param name="headless"></param>
    /// <returns></returns>
    public TemporalServerExecutableResourceBuilder WithHeadlessUi(bool headless)
    {
        args.Headless = headless;
        return this;
    }

    /// <summary>
    /// IPv4 address to bind the frontend service to (default: "127.0.0.1")
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    public TemporalServerExecutableResourceBuilder WithIp(string ip)
    {
        args.Ip = ip;
        return this;
    }

    /// <summary>
    /// IPv4 address to bind the Web UI to (default: same as --ip)   
    /// </summary>
    /// <param name="uiIp"></param>
    /// <returns></returns>
    public TemporalServerExecutableResourceBuilder WithUiIp(string uiIp)
    {
        args.UiIp = uiIp;
        return this;
    }

    /// <summary>
    /// UI custom assets path 
    /// </summary>
    /// <param name="assetsPath"></param>
    /// <returns></returns>
    public TemporalServerExecutableResourceBuilder WithUiAssetsPath(string assetsPath)
    {
        args.UiAssetPath = assetsPath;
        return this;
    }

    /// <summary>
    /// UI remote codec HTTP endpoint
    /// </summary>
    /// <param name="codecEndpoint"></param>
    /// <returns></returns>
    public TemporalServerExecutableResourceBuilder WithUiCodecEndpoint(string codecEndpoint)
    {
        args.UiCodecEndpoint = codecEndpoint;
        return this;
    }

    /// <summary>
    /// Set the log formatting. Options: ["json", "pretty"]. (default: "json")
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public TemporalServerExecutableResourceBuilder WithLogFormat(LogFormat format)
    {
        args.LogFormat = format;
        return this;
    }

    /// <summary>
    /// Set the log level. Options: ["debug" "info" "warn" "error" "fatal"]. (default: "info")
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public TemporalServerExecutableResourceBuilder WithLogLevel(LogLevel level)
    {
        args.LogLevel = level;
        return this;
    }

    /// <summary>
    /// Specify SQLite pragma statements in pragma=value format. Pragma options: ["journal_mode" "synchronous"]
    /// </summary>
    /// <param name="pragma"></param>
    /// <returns></returns>
    public TemporalServerExecutableResourceBuilder WithSQLitePragma(SQLitePragma pragma)
    {
        args.SQLitePragma = pragma;
        return this;
    }

    /// <summary>
    /// Specify namespaces that should be pre-created (namespace "default" is always created)
    /// </summary>
    /// <param name="ns"></param>
    /// <returns></returns>
    public TemporalServerExecutableResourceBuilder WithNamespace(params string[] namespaces)
    {
        args.Namespaces.AddRange(namespaces);
        return this;
    }

    public TemporalServerExecutableResourceArguments Build()
    {
        return args;
    }
}