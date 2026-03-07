using Aspire.Hosting.ApplicationModel;
using InfinityFlow.Aspire.Temporal;

namespace Aspire.Hosting;

public static class TemporalServerContainerBuilderExtensions
{
    internal const string TemporalServerImageName = "temporalio/admin-tools";
    internal const string TemporalServerImageTag = "1.28.2-tctl-1.18.1-cli-1.1.1";

    public static IResourceBuilder<TemporalServerContainerResource> AddTemporalServerContainer(
        this IDistributedApplicationBuilder builder, string name)
    {
        var resource = new TemporalServerContainerResource(name);
        return builder.AddResource(resource)
            .WithImage(TemporalServerImageName, TemporalServerImageTag)
            .WithEntrypoint("temporal");
    }
}
