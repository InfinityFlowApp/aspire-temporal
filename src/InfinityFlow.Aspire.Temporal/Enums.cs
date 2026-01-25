namespace InfinityFlow.Aspire.Temporal;

/// <summary>
/// Log format options for the Temporal server.
/// </summary>
public enum LogFormat
{
    /// <summary>
    /// JSON log format.
    /// </summary>
    Json,
    /// <summary>
    /// Pretty-printed log format.
    /// </summary>
    Pretty
}

/// <summary>
/// Log level options for the Temporal server.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Debug level logging.
    /// </summary>
    Debug,
    /// <summary>
    /// Info level logging.
    /// </summary>
    Info,
    /// <summary>
    /// Warning level logging.
    /// </summary>
    Warn,
    /// <summary>
    /// Error level logging.
    /// </summary>
    Error,
    /// <summary>
    /// Fatal level logging.
    /// </summary>
    Fatal
}

/// <summary>
/// SQLite pragma options for the Temporal server.
/// </summary>
public enum SQLitePragma
{
    /// <summary>
    /// Journal mode pragma.
    /// </summary>
    JournalMode,
    /// <summary>
    /// Synchronous pragma.
    /// </summary>
    Synchronous
}

/// <summary>
/// Helper methods for converting enums to Temporal CLI argument strings.
/// </summary>
public static class EnumHelpers
{
    /// <summary>
    /// Converts a LogFormat enum to its Temporal CLI string representation.
    /// </summary>
    /// <param name="logFormat">The log format to convert.</param>
    /// <returns>The string representation for Temporal CLI.</returns>
    public static string LogFormatToString(LogFormat logFormat) => logFormat switch
    {
        LogFormat.Pretty => "pretty",
        LogFormat.Json => "json",
        _ => throw new ArgumentOutOfRangeException(nameof(logFormat), logFormat, null),
    };

    /// <summary>
    /// Converts a LogLevel enum to its Temporal CLI string representation.
    /// </summary>
    /// <param name="level">The log level to convert.</param>
    /// <returns>The string representation for Temporal CLI.</returns>
    public static string LogLevelToString(LogLevel level) => level switch
    {
        LogLevel.Debug => "debug",
        LogLevel.Info => "info",
        LogLevel.Warn => "warn",
        LogLevel.Error => "error",
        LogLevel.Fatal => "fatal",
        _ => throw new ArgumentOutOfRangeException(nameof(level), level, null),
    };

    /// <summary>
    /// Converts a SQLitePragma enum to its Temporal CLI string representation.
    /// </summary>
    /// <param name="pragma">The SQLite pragma to convert.</param>
    /// <returns>The string representation for Temporal CLI.</returns>
    public static string SQLitePragmaToString(SQLitePragma pragma) => pragma switch
    {
        SQLitePragma.JournalMode => "journal_mode",
        SQLitePragma.Synchronous => "synchronous",
        _ => throw new ArgumentOutOfRangeException(nameof(pragma), pragma, null),
    };
}