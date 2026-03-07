using Aspire.Hosting.ApplicationModel;

namespace Aspire.Hosting;

/// <summary>A Temporal dev server running as a container resource.</summary>
/// <param name="name">The resource name.</param>
public class TemporalServerContainerResource(string name) : ContainerResource(name), InfinityFlow.Aspire.Temporal.ITemporalServerResource, IResourceWithEnvironment
{
    /// <inheritdoc />
    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create($"{this.GetEndpoint("server").Property(EndpointProperty.HostAndPort)}");
}
