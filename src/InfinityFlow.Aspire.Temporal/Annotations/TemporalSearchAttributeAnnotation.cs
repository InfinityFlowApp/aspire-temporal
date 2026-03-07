using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

/// <summary>Annotation to register a custom search attribute at server launch.</summary>
/// <param name="Key">The search attribute name.</param>
/// <param name="Type">The search attribute type.</param>
public sealed record TemporalSearchAttributeAnnotation(string Key, SearchAttributeType Type) : IResourceAnnotation;
