namespace Aspire.Temporal.Server;

public class TemporalServerExecutableResource(string name, TemporalServerExecutableResourceArguments arguments) : ExecutableResource(name, command: "temporal", workingDirectory: "", args: arguments.GetArgs()), IResourceWithConnectionString
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