using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using InfinityFlow.Aspire.Temporal;

namespace InfinityFlow.Aspire.Temporal.Tests;

public class TemporalResourceTests
{
    [Fact]
    public void AddTemporalServerContainer_WithoutPorts_CreatesResourceWithNullPorts()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var temporal = builder.AddTemporalServerContainer("temporal", x => x
            .WithLogFormat(LogFormat.Json)
            .WithLogLevel(LogLevel.Info)
            .WithNamespace("test"));

        // Assert
        var resource = temporal.Resource as TemporalServerContainerResource;
        Assert.NotNull(resource);
        Assert.NotNull(resource.Arguments);

        // Port should be null (dynamic allocation)
        Assert.Null(resource.Arguments.Port);
        Assert.Null(resource.Arguments.UiPort);
    }

    [Fact]
    public void AddTemporalServerContainer_WithFixedPorts_CreatesResourceWithSpecifiedPorts()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        int serverPort = 7233;
        int uiPort = 8233;

        // Act
        var temporal = builder.AddTemporalServerContainer("temporal-fixed", x => x
            .WithPort(serverPort)
            .WithUiPort(uiPort)
            .WithLogFormat(LogFormat.Json));

        // Assert
        var resource = temporal.Resource as TemporalServerContainerResource;
        Assert.NotNull(resource);
        Assert.NotNull(resource.Arguments);

        // Ports should be set to specified values
        Assert.Equal(serverPort, resource.Arguments.Port);
        Assert.Equal(uiPort, resource.Arguments.UiPort);
    }

    [Fact]
    public void TemporalServerResourceArguments_DefaultPort_IsNull()
    {
        // Arrange & Act
        var args = new TemporalServerResourceArguments();

        // Assert
        Assert.Null(args.Port);
        Assert.Null(args.UiPort);
    }

    [Fact]
    public void TemporalServerResourceBuilder_WithPort_SetsPort()
    {
        // Arrange
        var builder = new TemporalServerResourceBuilder();

        // Act
        builder.WithPort(7233);
        var args = builder.Build();

        // Assert
        Assert.Equal(7233, args.Port);
    }

    [Fact]
    public void TemporalServerResourceBuilder_WithNullPort_LeavesPortNull()
    {
        // Arrange
        var builder = new TemporalServerResourceBuilder();

        // Act
        builder.WithPort(null);
        var args = builder.Build();

        // Assert
        Assert.Null(args.Port);
    }

    [Fact]
    public void TemporalServerResourceArguments_GetArgs_WithNullPort_OmitsPortFlag()
    {
        // Arrange
        var args = new TemporalServerResourceArguments
        {
            Port = null
        };

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.DoesNotContain("--port", cmdArgs);
    }

    [Fact]
    public void TemporalServerResourceArguments_GetArgs_WithPort_IncludesPortFlag()
    {
        // Arrange
        var args = new TemporalServerResourceArguments
        {
            Port = 7233
        };

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("--port", cmdArgs);
        Assert.Contains("7233", cmdArgs);
    }
}
