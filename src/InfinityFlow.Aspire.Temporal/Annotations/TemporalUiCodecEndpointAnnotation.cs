using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation for the UI codec endpoint (--ui-codec-endpoint).</summary>
/// <param name="CodecEndpoint">The codec endpoint URL.</param>
public sealed record TemporalUiCodecEndpointAnnotation(string CodecEndpoint) : IResourceAnnotation;
