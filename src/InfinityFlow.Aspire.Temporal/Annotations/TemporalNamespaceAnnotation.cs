using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation for a namespace to register (--namespace).</summary>
public sealed record TemporalNamespaceAnnotation(string Namespace) : IResourceAnnotation;
