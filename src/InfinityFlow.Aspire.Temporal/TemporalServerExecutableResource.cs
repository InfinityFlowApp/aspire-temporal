using InfinityFlow.Aspire.Temporal;

namespace Aspire.Hosting;

public class TemporalServerExecutableResource(string name, TemporalServerResourceArguments arguments) : ExecutableResource(name, command: "temporal", workingDirectory: "", args: arguments.GetArgs()), IResourceWithConnectionString
{
    public string? GetConnectionString()
    {
        if (!this.TryGetAllocatedEndPoints(out var endpoints))
        {
            throw new DistributedApplicationException("Expected allocated endpoints!");
        }

        var server = endpoints.Single(x => x.Name == "server");

        return server.EndPointString;
    }
}

public class TemporalServerContainerResource(string name, TemporalServerResourceArguments arguments) : ContainerResource(name,entrypoint: "/temporal"), IResourceWithConnectionString, IResourceWithEnvironment
{
    public TemporalServerResourceArguments Arguments { get; } = arguments;

    public string? GetConnectionString()
    {
        if (!this.TryGetAllocatedEndPoints(out var endpoints))
        {
            throw new DistributedApplicationException("Expected allocated endpoints!");
        }

        var server = endpoints.Single(x => x.Name == "server");

        return server.EndPointString;
    }
}
