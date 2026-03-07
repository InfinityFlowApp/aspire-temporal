using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

public sealed record TemporalDbFileNameAnnotation(string FileName) : IResourceAnnotation;
