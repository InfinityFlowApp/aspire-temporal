using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

public sealed record TemporalDynamicConfigAnnotation(string Key, object Value) : IResourceAnnotation;
