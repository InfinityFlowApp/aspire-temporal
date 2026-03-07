using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

public sealed record TemporalUiIpAnnotation(string UiIp) : IResourceAnnotation;
