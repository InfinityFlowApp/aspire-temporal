using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

public sealed record TemporalSQLitePragmaAnnotation(SQLitePragma Pragma) : IResourceAnnotation;
