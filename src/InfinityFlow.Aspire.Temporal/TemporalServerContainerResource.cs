using Aspire.Hosting.ApplicationModel;

namespace Aspire.Hosting;

public class TemporalServerContainerResource(string name) : ContainerResource(name), InfinityFlow.Aspire.Temporal.ITemporalServerResource, IResourceWithEnvironment
{
    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create($"{this.GetEndpoint("server").Property(EndpointProperty.HostAndPort)}");
}
