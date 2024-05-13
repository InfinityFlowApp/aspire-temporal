namespace InfinityFlow.Aspire.Temporal;

/// <summary>
/// Represents the arguments required to configure and start a Temporal server.
/// </summary>
public class TemporalServerResourceArguments
{
    /// <summary>
    /// Gets or sets the command to execute. Default is "temporal".
    /// </summary>
    public string Command { get; set; } = "temporal";

    /// <summary>
    /// Gets or sets the file name where the Temporal state will be persisted. If not set, workflows are lost when the process dies.
    /// </summary>
    public string? DbFileName { get; set; }

    /// <summary>
    /// Gets or sets the port for the frontend gRPC service. Default is 7233.
    /// </summary>
    public int Port { get; set; } = 7233;

    /// <summary>
    /// Gets or sets the port for the frontend HTTP service. If not set, the service is disabled.
    /// </summary>
    public int? HttpPort { get; set; }

    /// <summary>
    /// Gets or sets the port for metrics. If not set, metrics are disabled.
    /// </summary>
    public int? MetricsPort { get; set; }

    /// <summary>
    /// Gets or sets the port for the Web UI. Default is 8233.
    /// </summary>
    public int? UiPort { get; set; } = 8233;

    /// <summary>
    /// Gets or sets a value indicating whether the Web UI is headless. If not set, defaults to false.
    /// </summary>
    public bool? Headless { get; set; }

    /// <summary>
    /// Gets or sets the IPv4 address to bind the frontend service to. Default is 0.0.0.0.
    /// </summary>
    public string? Ip { get; set; } = "0.0.0.0";

    /// <summary>
    /// Gets or sets the IPv4 address to bind the Web UI to. Default is null, which means it uses the same as --ip.
    /// </summary>
    public string? UiIp { get; set; }

    /// <summary>
    /// Gets or sets the path to the UI custom assets.
    /// </summary>
    public string? UiAssetPath { get; set; }

    /// <summary>
    /// Gets or sets the UI remote codec HTTP endpoint.
    /// </summary>
    public string? UiCodecEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the log format. Options are "json" and "pretty". Default is null.
    /// </summary>
    public LogFormat? LogFormat { get; set; }

    /// <summary>
    /// Gets or sets the log level. Options are "debug", "info", "warn", "error", and "fatal". Default is null.
    /// </summary>
    public LogLevel? LogLevel { get; set; }

    /// <summary>
    /// Gets or sets the SQLite pragma. Options are "journal_mode" and "synchronous". Default is null.
    /// </summary>
    public SQLitePragma? SQLitePragma { get; set; }

    /// <summary>
    /// Gets the list of namespaces that should be pre-created. The "default" namespace is always created.
    /// </summary>
    public List<string> Namespaces { get; set; } = [];

    /// <summary>
    /// Gets the list of dynamic config values.
    /// </summary>
    public Dictionary<string, object> DynamicConfigValues { get; set; } = [];

    /// <summary>
    /// Converts the current instance's properties to an array of command-line arguments for starting the Temporal server.
    /// </summary>
    /// <returns>An array of strings representing the command-line arguments.</returns>
    public string[] GetArgs()
    {
        var result = new List<string>
            {
                "server",
                "start-dev"
            };

        AddIfNotNull(result, "--db-filename", DbFileName);
        AddAlways(result, "--port", Port.ToString());

        AddIfNotNull(result, "--http-port", HttpPort?.ToString());
        AddIfNotNull(result, "--metrics-port", MetricsPort?.ToString());
        AddIfNotNull(result, "--ui-port", UiPort?.ToString());
        AddIfNotNull(result, "--headless", Headless?.ToString().ToLowerInvariant());
        AddIfNotNull(result, "--ip", Ip);
        AddIfNotNull(result, "--ui-ip", UiIp);
        AddIfNotNull(result, "--ui-asset-path", UiAssetPath);
        AddIfNotNull(result, "--ui-codec-endpoint", UiCodecEndpoint);

        if (LogFormat.HasValue)
        {
            result.Add("--log-format");
            result.Add(EnumHelpers.LogFormatToString(LogFormat.Value));
        }

        if (LogLevel.HasValue)
        {
            result.Add("--log-level");
            result.Add(EnumHelpers.LogLevelToString(LogLevel.Value));
        }

        if (SQLitePragma.HasValue)
        {
            result.Add("--sqlite-pragma");
            result.Add(EnumHelpers.SQLitePragmaToString(SQLitePragma.Value));
        }

        foreach (var name in Namespaces)
        {
            result.Add("--namespace");
            result.Add(name);
        }

        foreach (var (k, v) in DynamicConfigValues)
        {
            result.Add("--dynamic-config-value");

            result.Add($"{k}={v switch
            {
                string s => $""" "{v}" """,
                bool b => b.ToString().ToLowerInvariant(),
                int i => i.ToString(),
                float f => f.ToString("F"),
                double d => d.ToString("F"),
                long l => l.ToString(),
                _ => null,
            }}");
        }

        return [.. result];
    }

    private static void AddIfNotNull(List<string> list, string argument, string? value)
    {
        if (value is not null)
        {
            list.Add(argument);
            list.Add(value);
        }
    }

    private static void AddAlways(List<string> list, string argument, string value)
    {
        list.Add(argument);
        list.Add(value);
    }
}