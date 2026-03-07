using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation for a SQLite pragma setting (--sqlite-pragma).</summary>
/// <param name="Pragma">The SQLite pragma.</param>
public sealed record TemporalSQLitePragmaAnnotation(SQLitePragma Pragma) : IResourceAnnotation;
