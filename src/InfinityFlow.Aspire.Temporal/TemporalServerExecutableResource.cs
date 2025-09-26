using InfinityFlow.Aspire.Temporal;
using Aspire.Hosting.ApplicationModel;


namespace Aspire.Hosting;

public class TemporalServerExecutableResource(string name, TemporalServerResourceArguments arguments) : ExecutableResource(name, command: "temporal", workingDirectory: ""), IResourceWithConnectionString
{
    public ReferenceExpression ConnectionStringExpression => ReferenceExpression.Create($"{this.GetEndpoint("server").Property(EndpointProperty.HostAndPort)}");
}

public class TemporalServerContainerResource(string name, TemporalServerResourceArguments arguments) : ContainerResource(name,entrypoint: "/temporal"), IResourceWithConnectionString, IResourceWithEnvironment
{
    public ReferenceExpression ConnectionStringExpression => ReferenceExpression.Create($"{this.GetEndpoint("server").Property(EndpointProperty.HostAndPort)}");

    public TemporalServerResourceArguments Arguments { get; } = arguments;
}
