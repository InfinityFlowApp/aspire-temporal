using Aspire.Hosting;
using InfinityFlow.Aspire.Temporal;

namespace InfinityFlow.Aspire.Temporal.Tests;

public class TemporalBuilderValidationTests
{
    [Fact]
    public void WithCommand_NullCommand_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServer("temporal");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => temporal.WithCommand(null!));
    }

    [Fact]
    public void WithCommand_WhitespaceCommand_ThrowsArgumentException()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServer("temporal");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => temporal.WithCommand("   "));
    }

    [Fact]
    public void WithDbFileName_NullFileName_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServer("temporal");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => temporal.WithDbFileName(null!));
    }

    [Fact]
    public void WithDbFileName_WhitespaceFileName_ThrowsArgumentException()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServer("temporal");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => temporal.WithDbFileName("   "));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(65536)]
    [InlineData(70000)]
    public void WithServiceEndpoint_InvalidPort_ThrowsArgumentOutOfRangeException(int invalidPort)
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServer("temporal");

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => temporal.WithServiceEndpoint(invalidPort));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(80)]
    [InlineData(7233)]
    [InlineData(65535)]
    public void WithServiceEndpoint_ValidPort_Succeeds(int validPort)
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServer("temporal");

        // Act
        var result = temporal.WithServiceEndpoint(validPort);

        // Assert
        Assert.NotNull(result);
        var resource = result.Resource().Resource as TemporalServerContainerResource;
        Assert.Equal(validPort, resource!.Arguments.Port);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(65536)]
    [InlineData(70000)]
    public void WithUiEndpoint_InvalidPort_ThrowsArgumentOutOfRangeException(int invalidPort)
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServer("temporal");

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => temporal.WithUiEndpoint(invalidPort));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(65536)]
    public void WithMetricsEndpoint_InvalidPort_ThrowsArgumentOutOfRangeException(int invalidPort)
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServer("temporal");

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => temporal.WithMetricsEndpoint(invalidPort));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(65536)]
    public void WithHttpEndpoint_InvalidPort_ThrowsArgumentOutOfRangeException(int invalidPort)
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServer("temporal");

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => temporal.WithHttpEndpoint(invalidPort));
    }

    [Fact]
    public void WithIp_NullIp_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServer("temporal");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => temporal.WithIp(null!));
    }

    [Fact]
    public void WithUiIp_WhitespaceIp_ThrowsArgumentException()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServer("temporal");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => temporal.WithUiIp("   "));
    }

    [Fact]
    public void WithUiAssetsPath_NullPath_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServer("temporal");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => temporal.WithUiAssetsPath(null!));
    }

    [Fact]
    public void WithUiCodecEndpoint_WhitespaceEndpoint_ThrowsArgumentException()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServer("temporal");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => temporal.WithUiCodecEndpoint("   "));
    }

    [Fact]
    public void WithNamespace_NullArray_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServer("temporal");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => temporal.WithNamespace(null!));
    }

    [Fact]
    public void WithNamespace_ContainsNullElement_ThrowsArgumentException()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServer("temporal");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => temporal.WithNamespace("valid", null!, "another"));
    }

    [Fact]
    public void WithNamespace_ContainsWhitespaceElement_ThrowsArgumentException()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServer("temporal");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => temporal.WithNamespace("valid", "   ", "another"));
    }

    [Fact]
    public void WithDynamicConfigValue_NullKey_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServer("temporal");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => temporal.WithDynamicConfigValue(null!, true));
    }

    [Fact]
    public void WithDynamicConfigValue_WhitespaceKey_ThrowsArgumentException()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServer("temporal");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => temporal.WithDynamicConfigValue("   ", true));
    }

    [Fact]
    public void WithDynamicConfigValue_NullValue_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServer("temporal");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => temporal.WithDynamicConfigValue("key", null!));
    }

    [Fact]
    public void WithServiceEndpoint_NullPort_AllowsDynamicAllocation()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var temporal = builder.AddTemporalServer("temporal")
            .WithServiceEndpoint(null);

        var resource = temporal.Resource().Resource as TemporalServerContainerResource;

        // Assert
        Assert.NotNull(resource);
        Assert.Null(resource.Arguments.Port);
    }

    [Fact]
    public void WithUiEndpoint_NullPort_AllowsDynamicAllocation()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var temporal = builder.AddTemporalServer("temporal")
            .WithUiEndpoint(null);

        var resource = temporal.Resource().Resource as TemporalServerContainerResource;

        // Assert
        Assert.NotNull(resource);
        Assert.Null(resource.Arguments.UiPort);
    }
}
