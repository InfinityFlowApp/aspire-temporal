using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation for the database file name (--db-filename).</summary>
/// <param name="FileName">The database file path.</param>
public sealed record TemporalDbFileNameAnnotation(string FileName) : IResourceAnnotation;
