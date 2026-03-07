using Aspire.Hosting.ApplicationModel;
using InfinityFlow.Aspire.Temporal;

namespace Aspire.Hosting;

/// <summary>
/// Extension methods for adding a Temporal server container resource.
/// </summary>
public static class TemporalServerContainerBuilderExtensions
{
    internal const string TemporalServerImageName = "temporalio/admin-tools";
    internal const string TemporalServerImageTag = "1.28.2-tctl-1.18.1-cli-1.1.1";

    /// <summary>
    /// Adds a Temporal dev server as a container resource.
    /// </summary>
    public static IResourceBuilder<TemporalServerContainerResource> AddTemporalServerContainer(
        this IDistributedApplicationBuilder builder, string name)
    {
        var resource = new TemporalServerContainerResource(name);

        var resourceBuilder = builder.AddResource(resource)
            .WithImage(TemporalServerImageName, TemporalServerImageTag)
            .WithEntrypoint("temporal")
            .WithArgs(ctx =>
            {
                var args = TemporalServerArgsBuilder.BuildArgs(resource);
                foreach (var arg in args)
                    ctx.Args.Add(arg);
            })
            .WithHttpsEndpoint(name: "server", targetPort: 7233).AsHttp2Service()
            .WithHttpEndpoint(name: "ui", targetPort: 8233);

        return resourceBuilder;
    }
}
