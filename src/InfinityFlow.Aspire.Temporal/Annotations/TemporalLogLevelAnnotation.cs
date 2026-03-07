using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation for the log level (--log-level).</summary>
/// <param name="Level">The log level.</param>
public sealed record TemporalLogLevelAnnotation(LogLevel Level) : IResourceAnnotation;
