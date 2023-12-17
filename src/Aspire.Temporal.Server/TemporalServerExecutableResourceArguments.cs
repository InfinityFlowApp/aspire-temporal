namespace Aspire.Temporal.Server;

public class TemporalServerExecutableResourceArguments
{
    public string? DbFileName { get; set; }
    public int? Port { get; set; }
    public int? HttpPort { get; set; }
    public int? MetricsPort { get; set; }
    public int? UiPort { get; set; }
    public bool? Headless { get; set; }
    public string? Ip { get; set; }
    public string? UiIp { get; set; }
    public string? UiAssetPath { get; set; }
    public string? UiCodecEndpoint { get; set; }

    public LogFormat? LogFormat { get; set; }
    public LogLevel? LogLevel { get; set; }
    public SQLitePragma? SQLitePragma { get; set; }
    public List<string> Namespaces { get; set; } = new();

    public string[] GetArgs()
    {
        var result = new List<string>
        {
            "server",
            "start-dev"
        };

        if (DbFileName is not null)
        {
            result.Add("--db-filename");
            result.Add(DbFileName);
        }

        if (Port is not null)
        {
            result.Add("--port");
            result.Add(Port.ToString()!);
        }

        if (HttpPort is not null)
        {
            result.Add("--http-port");
            result.Add(HttpPort.ToString()!);
        }

        if (MetricsPort is not null)
        {
            result.Add("--metrics-port");
            result.Add(MetricsPort.ToString()!);
        }

        if (UiPort is not null)
        {
            result.Add("--ui-port");
            result.Add(UiPort.ToString()!);
        }

        if (Headless is not null)
        {
            result.Add("--headless");
            result.Add(Headless.ToString()!);
        }

        if (Ip is not null)
        {
            result.Add("--ip");
            result.Add(Ip.ToString()!);
        }

        if (UiIp is not null)
        {
            result.Add("--ui-ip");
            result.Add(UiIp.ToString()!);
        }

        if (UiAssetPath is not null)
        {
            result.Add("--ui-asset-path");
            result.Add(UiAssetPath.ToString());
        }

        if (UiCodecEndpoint is not null)
        {
            result.Add("--ui-codec-endpoint");
            result.Add(UiCodecEndpoint.ToString()!);
        }

        if (LogFormat is not null)
        {
            result.Add("--log-format");
            result.Add(LogFormatToString(LogFormat));
        }

        if (LogLevel is not null)
        {
            result.Add("--log-level");
            result.Add(LogLevelToString(LogLevel));
        }

        if (SQLitePragma is not null)
        {
            result.Add("--sqlite-pragma");
            result.Add(SQLitePragmaToString(SQLitePragma));
        }

        foreach (var name in Namespaces)
        {
            result.Add("--namespace");
            result.Add(name);

        }

        return [.. result];
    }

    private static string LogFormatToString(LogFormat? logFormat)
    {
        return logFormat switch
        {
            Server.LogFormat.Pretty => "pretty",
            Server.LogFormat.Json => "json",
            _ => throw new IndexOutOfRangeException($"{nameof(LogFormat)} outside expected range, got: {logFormat}"),
        };
    }

    private static string LogLevelToString(LogLevel? level)
    {
        return level switch
        {
            Server.LogLevel.Info => "info",
            Server.LogLevel.Warn => "warn",
            Server.LogLevel.Fatal => "fatal",
            Server.LogLevel.Error => "error",
            Server.LogLevel.Debug => "debug",
            _ => throw new IndexOutOfRangeException($"{nameof(LogLevel)} outside expected range, got: {level}"),
        };
    }

    private static string SQLitePragmaToString(SQLitePragma? pragma)
    {
        return pragma switch
        {
            Server.SQLitePragma.JournalMode => "journal_mode",
            Server.SQLitePragma.Synchronous => "synchronous",
            _ => throw new IndexOutOfRangeException($"{nameof(SQLitePragma)} outside expected range, got: {pragma}"),
        };
    }
}
public enum LogFormat
{
    Json,
    Pretty
}

public enum LogLevel
{
    Debug,
    Info,
    Warn,
    Error,
    Fatal
}

public enum SQLitePragma
{
    JournalMode,
    Synchronous
}