using InfinityFlow.Aspire.Temporal;
using Aspire.Hosting.ApplicationModel;


namespace Aspire.Hosting;

/// <summary>
/// Represents a Temporal server executable resource for Aspire orchestration.
/// </summary>
/// <param name="name">The name of the resource.</param>
/// <param name="arguments">The configuration arguments for the Temporal server.</param>
public class TemporalServerExecutableResource(string name, TemporalServerResourceArguments arguments) : ExecutableResource(name, command: "temporal", workingDirectory: ""), IResourceWithConnectionString
{
    /// <summary>
    /// Gets the connection string expression for the Temporal server.
    /// </summary>
    public ReferenceExpression ConnectionStringExpression => ReferenceExpression.Create($"{this.GetEndpoint("server").Property(EndpointProperty.HostAndPort)}");
}

/// <summary>
/// Represents a Temporal server container resource for Aspire orchestration.
/// </summary>
/// <param name="name">The name of the resource.</param>
/// <param name="arguments">The configuration arguments for the Temporal server.</param>
public class TemporalServerContainerResource(string name, TemporalServerResourceArguments arguments) : ContainerResource(name,entrypoint: "/temporal"), IResourceWithConnectionString, IResourceWithEnvironment
{
    /// <summary>
    /// Gets the connection string expression for the Temporal server.
    /// </summary>
    public ReferenceExpression ConnectionStringExpression => ReferenceExpression.Create($"{this.GetEndpoint("server").Property(EndpointProperty.HostAndPort)}");

    /// <summary>
    /// Gets the configuration arguments for the Temporal server.
    /// </summary>
    public TemporalServerResourceArguments Arguments { get; } = arguments;
}
