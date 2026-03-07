using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation for the server IP address (--ip).</summary>
/// <param name="Ip">The IP address to bind.</param>
public sealed record TemporalIpAnnotation(string Ip) : IResourceAnnotation;
