using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation for a dynamic config key-value pair (--dynamic-config-value).</summary>
public sealed record TemporalDynamicConfigAnnotation(string Key, object Value) : IResourceAnnotation;
