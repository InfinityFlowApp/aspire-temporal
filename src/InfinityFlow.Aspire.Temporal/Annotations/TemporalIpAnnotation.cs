using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

public sealed record TemporalIpAnnotation(string Ip) : IResourceAnnotation;
