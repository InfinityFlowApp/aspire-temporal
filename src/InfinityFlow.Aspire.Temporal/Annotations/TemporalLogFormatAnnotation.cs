using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation for the log format (--log-format).</summary>
public sealed record TemporalLogFormatAnnotation(LogFormat Format) : IResourceAnnotation;
