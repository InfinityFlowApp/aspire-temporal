using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation for a SQLite pragma setting (--sqlite-pragma).</summary>
public sealed record TemporalSQLitePragmaAnnotation(SQLitePragma Pragma) : IResourceAnnotation;
