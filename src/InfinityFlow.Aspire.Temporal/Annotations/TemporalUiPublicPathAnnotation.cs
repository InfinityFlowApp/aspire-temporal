using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation to set the public base path for the Web UI.</summary>
public sealed record TemporalUiPublicPathAnnotation(string PublicPath) : IResourceAnnotation;
