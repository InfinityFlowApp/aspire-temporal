using Aspire.Hosting.ApplicationModel;
using InfinityFlow.Aspire.Temporal;

namespace Aspire.Hosting;

public static class TemporalServerExecutableBuilderExtensions
{
    public static IResourceBuilder<TemporalServerExecutableResource> AddTemporalServerExecutable(
        this IDistributedApplicationBuilder builder, string name)
    {
        var resource = new TemporalServerExecutableResource(name);
        return builder.AddResource(resource);
    }
}
