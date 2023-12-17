namespace Aspire.Temporal.Server;

public class TemporalServerExecutableResource : ExecutableResource, IResourceWithConnectionString
{
    public TemporalServerExecutableResource(string name, TemporalServerExecutableResourceArguments arguments)
        : base(name, command: "temporal", workingDirectory: "", args: arguments.GetArgs())
    {
    }

    public string? GetConnectionString()
    {
        if (!this.TryGetAllocatedEndPoints(out var endpoints))
        {
            throw new DistributedApplicationException("Expected allocated endpoints!");
        }

        var server = endpoints.Single(x => x.Name == "server");

        return $"{server.Address}:{server.Port}";
    }
}