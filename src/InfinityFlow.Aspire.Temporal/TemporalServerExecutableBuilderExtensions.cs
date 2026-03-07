using Aspire.Hosting.ApplicationModel;
using InfinityFlow.Aspire.Temporal;

namespace Aspire.Hosting;

/// <summary>
/// Extension methods for adding a Temporal server executable resource.
/// </summary>
public static class TemporalServerExecutableBuilderExtensions
{
    /// <summary>
    /// Adds a Temporal dev server as an executable resource.
    /// </summary>
    public static IResourceBuilder<TemporalServerExecutableResource> AddTemporalServerExecutable(
        this IDistributedApplicationBuilder builder, string name)
    {
        var resource = new TemporalServerExecutableResource(name);

        var resourceBuilder = builder.AddResource(resource)
            .WithArgs(ctx =>
            {
                var args = TemporalServerArgsBuilder.BuildArgs(resource);
                foreach (var arg in args)
                    ctx.Args.Add(arg);
            })
            .WithHttpsEndpoint(name: "server").AsHttp2Service()
            .WithHttpEndpoint(name: "ui");

        return resourceBuilder;
    }
}
