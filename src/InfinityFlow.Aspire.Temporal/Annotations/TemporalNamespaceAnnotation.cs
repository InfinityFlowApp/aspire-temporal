using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

public sealed record TemporalNamespaceAnnotation(string Namespace) : IResourceAnnotation;
