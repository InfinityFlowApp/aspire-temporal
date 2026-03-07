using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation for the UI asset path (--ui-asset-path).</summary>
/// <param name="AssetPath">The asset path.</param>
public sealed record TemporalUiAssetPathAnnotation(string AssetPath) : IResourceAnnotation;
