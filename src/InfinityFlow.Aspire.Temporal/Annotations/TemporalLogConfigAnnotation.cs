using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation to enable logging server configuration to stderr.</summary>
/// <param name="Enabled">Whether to enable log config output.</param>
public sealed record TemporalLogConfigAnnotation(bool Enabled) : IResourceAnnotation;
