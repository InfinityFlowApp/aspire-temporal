using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

public sealed record TemporalUiAssetPathAnnotation(string AssetPath) : IResourceAnnotation;
