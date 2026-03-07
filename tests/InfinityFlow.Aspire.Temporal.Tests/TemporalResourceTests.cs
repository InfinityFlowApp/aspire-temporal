using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using InfinityFlow.Aspire.Temporal;
using Xunit;

namespace InfinityFlow.Aspire.Temporal.Tests;

public class TemporalResourceTests
{
    [Fact]
    public void TemporalServerContainerResource_ImplementsITemporalServerResource()
    {
        var resource = new TemporalServerContainerResource("test");
        Assert.IsAssignableFrom<ITemporalServerResource>(resource);
        Assert.IsAssignableFrom<IResourceWithConnectionString>(resource);
        Assert.IsAssignableFrom<ContainerResource>(resource);
    }

    [Fact]
    public void TemporalServerExecutableResource_ImplementsITemporalServerResource()
    {
        var resource = new TemporalServerExecutableResource("test");
        Assert.IsAssignableFrom<ITemporalServerResource>(resource);
        Assert.IsAssignableFrom<IResourceWithConnectionString>(resource);
        Assert.IsAssignableFrom<ExecutableResource>(resource);
    }

    [Fact]
    public void TemporalServerContainerResource_ConnectionString_UsesServerEndpoint()
    {
        var resource = new TemporalServerContainerResource("test");
        Assert.NotNull(resource.ConnectionStringExpression);
        Assert.Equal("{0}", resource.ConnectionStringExpression.Format);
    }
}
