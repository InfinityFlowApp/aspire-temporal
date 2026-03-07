using Aspire.Hosting.ApplicationModel;

namespace InfinityFlow.Aspire.Temporal.Annotations;

public sealed record TemporalHeadlessAnnotation(bool Headless) : IResourceAnnotation;
