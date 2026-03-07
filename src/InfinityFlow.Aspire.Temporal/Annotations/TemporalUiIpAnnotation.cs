using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation for the UI IP address (--ui-ip).</summary>
/// <param name="UiIp">The UI IP address.</param>
public sealed record TemporalUiIpAnnotation(string UiIp) : IResourceAnnotation;
