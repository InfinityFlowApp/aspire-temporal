namespace InfinityFlow.Aspire.Temporal;

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

public enum SearchAttributeType
{
    Text,
    Keyword,
    Int,
    Double,
    Bool,
    Datetime,
    KeywordList
}

public static class EnumHelpers
{
    public static string LogFormatToString(LogFormat logFormat) => logFormat switch
    {
        LogFormat.Pretty => "pretty",
        LogFormat.Json => "json",
        _ => throw new ArgumentOutOfRangeException(nameof(logFormat), logFormat, null),
    };

    public static string LogLevelToString(LogLevel level) => level switch
    {
        LogLevel.Debug => "debug",
        LogLevel.Info => "info",
        LogLevel.Warn => "warn",
        LogLevel.Error => "error",
        LogLevel.Fatal => "fatal",
        _ => throw new ArgumentOutOfRangeException(nameof(level), level, null),
    };

    public static string SQLitePragmaToString(SQLitePragma pragma) => pragma switch
    {
        SQLitePragma.JournalMode => "journal_mode",
        SQLitePragma.Synchronous => "synchronous",
        _ => throw new ArgumentOutOfRangeException(nameof(pragma), pragma, null),
    };

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