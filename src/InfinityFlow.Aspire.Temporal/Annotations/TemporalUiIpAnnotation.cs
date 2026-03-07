using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation for the UI IP address (--ui-ip).</summary>
public sealed record TemporalUiIpAnnotation(string UiIp) : IResourceAnnotation;
