using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation for the log format (--log-format).</summary>
/// <param name="Format">The log format.</param>
public sealed record TemporalLogFormatAnnotation(LogFormat Format) : IResourceAnnotation;
