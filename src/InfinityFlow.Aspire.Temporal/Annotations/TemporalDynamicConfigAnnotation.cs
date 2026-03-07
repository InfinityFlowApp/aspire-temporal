using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation for a dynamic config key-value pair (--dynamic-config-value).</summary>
/// <param name="Key">The config key.</param>
/// <param name="Value">The config value.</param>
public sealed record TemporalDynamicConfigAnnotation(string Key, object Value) : IResourceAnnotation;
