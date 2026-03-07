using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation to run the server without the Web UI (--headless).</summary>
/// <param name="Headless">Whether to run headless.</param>
public sealed record TemporalHeadlessAnnotation(bool Headless) : IResourceAnnotation;
