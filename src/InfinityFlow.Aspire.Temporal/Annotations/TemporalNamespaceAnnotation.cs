using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation for a namespace to register (--namespace).</summary>
/// <param name="Namespace">The namespace name.</param>
public sealed record TemporalNamespaceAnnotation(string Namespace) : IResourceAnnotation;
