using Aspire.Hosting.ApplicationModel;

namespace Aspire.Hosting;

/// <summary>A Temporal dev server running as an executable resource.</summary>
public class TemporalServerExecutableResource(string name) : ExecutableResource(name, command: "temporal", workingDirectory: ""), InfinityFlow.Aspire.Temporal.ITemporalServerResource
{
    /// <inheritdoc />
    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create($"{this.GetEndpoint("server").Property(EndpointProperty.HostAndPort)}");
}
