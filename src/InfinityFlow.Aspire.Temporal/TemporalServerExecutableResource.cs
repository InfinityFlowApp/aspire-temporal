using InfinityFlow.Aspire.Temporal;
using Aspire.Hosting.ApplicationModel;


namespace Aspire.Hosting;

public class TemporalServerExecutableResource(string name, TemporalServerResourceArguments arguments) : ExecutableResource(name, command: "temporal", workingDirectory: ""), IResourceWithConnectionString
{
    public ReferenceExpression ConnectionStringExpression => ReferenceExpression.Create($"{GetConnectionString()}");

    public string? GetConnectionString()
    {
        var endpoints = this.GetEndpoints().Where(e => e.IsAllocated).ToList();
        if (endpoints.Count==0)
        {
            throw new DistributedApplicationException("Expected allocated endpoints!");
        }

        var server = endpoints.SingleOrDefault(x => x.EndpointName == "server");

        return $"{server?.Host}:{server?.Port}";
    }
}

public class TemporalServerContainerResource(string name, TemporalServerResourceArguments arguments) : ContainerResource(name,entrypoint: "/temporal"), IResourceWithConnectionString, IResourceWithEnvironment
{
    public ReferenceExpression ConnectionStringExpression => ReferenceExpression.Create($"{GetConnectionString()}");

    public TemporalServerResourceArguments Arguments { get; } = arguments;

    public string? GetConnectionString()
    {
        var endpoints = this.GetEndpoints().Where(e => e.IsAllocated).ToList();
        if (endpoints.Count == 0)
        {
            throw new DistributedApplicationException("Expected allocated endpoints!");
        }

        var server = endpoints.SingleOrDefault(x => x.EndpointName == "server");

        return $"{server?.Host}:{server?.Port}";
    }
}
