using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

public sealed record TemporalLogLevelAnnotation(LogLevel Level) : IResourceAnnotation;
