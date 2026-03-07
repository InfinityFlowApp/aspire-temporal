namespace InfinityFlow.Aspire.Temporal;

/// <summary>Temporal server log output format.</summary>
public enum LogFormat
{
    /// <summary>JSON format.</summary>
    Json,
    /// <summary>Pretty-printed format.</summary>
    Pretty
}

/// <summary>Temporal server log level.</summary>
public enum LogLevel
{
    /// <summary>Debug level.</summary>
    Debug,
    /// <summary>Info level.</summary>
    Info,
    /// <summary>Warn level.</summary>
    Warn,
    /// <summary>Error level.</summary>
    Error,
    /// <summary>Fatal level.</summary>
    Fatal
}

/// <summary>SQLite pragma settings for the Temporal server.</summary>
public enum SQLitePragma
{
    /// <summary>Journal mode pragma.</summary>
    JournalMode,
    /// <summary>Synchronous pragma.</summary>
    Synchronous
}

/// <summary>Temporal search attribute value types.</summary>
public enum SearchAttributeType
{
    /// <summary>Text type.</summary>
    Text,
    /// <summary>Keyword type.</summary>
    Keyword,
    /// <summary>Int type.</summary>
    Int,
    /// <summary>Double type.</summary>
    Double,
    /// <summary>Bool type.</summary>
    Bool,
    /// <summary>Datetime type.</summary>
    Datetime,
    /// <summary>KeywordList type.</summary>
    KeywordList
}

/// <summary>Converts Temporal enums to CLI string values.</summary>
public static class EnumHelpers
{
    /// <summary>Converts a LogFormat to its CLI string.</summary>
    public static string LogFormatToString(LogFormat logFormat) => logFormat switch
    {
        LogFormat.Pretty => "pretty",
        LogFormat.Json => "json",
        _ => throw new ArgumentOutOfRangeException(nameof(logFormat), logFormat, null),
    };

    /// <summary>Converts a LogLevel to its CLI string.</summary>
    public static string LogLevelToString(LogLevel level) => level switch
    {
        LogLevel.Debug => "debug",
        LogLevel.Info => "info",
        LogLevel.Warn => "warn",
        LogLevel.Error => "error",
        LogLevel.Fatal => "fatal",
        _ => throw new ArgumentOutOfRangeException(nameof(level), level, null),
    };

    /// <summary>Converts a SQLitePragma to its CLI string.</summary>
    public static string SQLitePragmaToString(SQLitePragma pragma) => pragma switch
    {
        SQLitePragma.JournalMode => "journal_mode",
        SQLitePragma.Synchronous => "synchronous",
        _ => throw new ArgumentOutOfRangeException(nameof(pragma), pragma, null),
    };

    /// <summary>Converts a SearchAttributeType to its CLI string.</summary>
    public static string SearchAttributeTypeToString(SearchAttributeType type) => type switch
    {
        SearchAttributeType.Text => "Text",
        SearchAttributeType.Keyword => "Keyword",
        SearchAttributeType.Int => "Int",
        SearchAttributeType.Double => "Double",
        SearchAttributeType.Bool => "Bool",
        SearchAttributeType.Datetime => "Datetime",
        SearchAttributeType.KeywordList => "KeywordList",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}