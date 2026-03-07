using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

public sealed record TemporalLogFormatAnnotation(LogFormat Format) : IResourceAnnotation;
