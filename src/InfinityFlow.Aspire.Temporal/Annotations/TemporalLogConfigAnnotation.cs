using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation to enable logging server configuration to stderr.</summary>
public sealed record TemporalLogConfigAnnotation(bool Enabled) : IResourceAnnotation;
