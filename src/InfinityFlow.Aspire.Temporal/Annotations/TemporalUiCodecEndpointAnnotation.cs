using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

public sealed record TemporalUiCodecEndpointAnnotation(string CodecEndpoint) : IResourceAnnotation;
