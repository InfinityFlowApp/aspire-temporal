using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation for the UI codec endpoint (--ui-codec-endpoint).</summary>
public sealed record TemporalUiCodecEndpointAnnotation(string CodecEndpoint) : IResourceAnnotation;
