using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation for the server IP address (--ip).</summary>
public sealed record TemporalIpAnnotation(string Ip) : IResourceAnnotation;
