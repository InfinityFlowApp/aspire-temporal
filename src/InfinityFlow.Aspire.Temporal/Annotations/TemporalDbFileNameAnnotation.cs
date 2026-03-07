using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation for the database file name (--db-filename).</summary>
public sealed record TemporalDbFileNameAnnotation(string FileName) : IResourceAnnotation;
