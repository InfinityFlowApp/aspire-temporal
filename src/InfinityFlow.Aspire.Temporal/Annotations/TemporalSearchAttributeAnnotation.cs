using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation to register a custom search attribute at server launch.</summary>
public sealed record TemporalSearchAttributeAnnotation(string Key, SearchAttributeType Type) : IResourceAnnotation;
