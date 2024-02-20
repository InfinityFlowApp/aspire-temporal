namespace Aspire.Temporal.Server;

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