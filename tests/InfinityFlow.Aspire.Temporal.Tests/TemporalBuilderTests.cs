using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using InfinityFlow.Aspire.Temporal;

namespace InfinityFlow.Aspire.Temporal.Tests;

public class TemporalBuilderTests
{
    [Fact]
    public void AddTemporalServer_CreatesContainerResourceByDefault()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var temporal = builder.AddTemporalServer("temporal")
            .WithLogFormat(LogFormat.Json);

        var resource = temporal.Resource().Resource;

        // Assert
        Assert.IsType<TemporalServerContainerResource>(resource);
    }

    [Fact]
    public void AddTemporalServer_WithExecutable_CreatesExecutableResource()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var temporal = builder.AddTemporalServer("temporal")
            .WithExecutable()
            .WithLogFormat(LogFormat.Json);

        var resource = temporal.Resource().Resource;

        // Assert
        Assert.IsType<TemporalServerExecutableResource>(resource);
    }

    [Fact]
    public void AddTemporalServer_WithServiceEndpoint_SetsPort()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var temporal = builder.AddTemporalServer("temporal")
            .WithServiceEndpoint(7233);

        var resource = temporal.Resource().Resource as TemporalServerContainerResource;

        // Assert
        Assert.NotNull(resource);
        Assert.Equal(7233, resource.Arguments.Port);
    }

    [Fact]
    public void AddTemporalServer_WithUiEndpoint_SetsUiPort()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var temporal = builder.AddTemporalServer("temporal")
            .WithUiEndpoint(8233);

        var resource = temporal.Resource().Resource as TemporalServerContainerResource;

        // Assert
        Assert.NotNull(resource);
        Assert.Equal(8233, resource.Arguments.UiPort);
        Assert.False(resource.Arguments.Headless);
    }

    [Fact]
    public void AddTemporalServer_WithMetricsEndpoint_SetsMetricsPort()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var temporal = builder.AddTemporalServer("temporal")
            .WithMetricsEndpoint(9090);

        var resource = temporal.Resource().Resource as TemporalServerContainerResource;

        // Assert
        Assert.NotNull(resource);
        Assert.Equal(9090, resource.Arguments.MetricsPort);
    }

    [Fact]
    public void AddTemporalServer_WithoutUi_SetsHeadlessToTrue()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var temporal = builder.AddTemporalServer("temporal")
            .WithoutUi();

        var resource = temporal.Resource().Resource as TemporalServerContainerResource;

        // Assert
        Assert.NotNull(resource);
        Assert.True(resource.Arguments.Headless);
        Assert.Null(resource.Arguments.UiPort);
    }

    [Fact]
    public void AddTemporalServer_WithLogFormat_SetsLogFormat()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var temporal = builder.AddTemporalServer("temporal")
            .WithLogFormat(LogFormat.Pretty);

        var resource = temporal.Resource().Resource as TemporalServerContainerResource;

        // Assert
        Assert.NotNull(resource);
        Assert.Equal(LogFormat.Pretty, resource.Arguments.LogFormat);
    }

    [Fact]
    public void AddTemporalServer_WithLogLevel_SetsLogLevel()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var temporal = builder.AddTemporalServer("temporal")
            .WithLogLevel(LogLevel.Debug);

        var resource = temporal.Resource().Resource as TemporalServerContainerResource;

        // Assert
        Assert.NotNull(resource);
        Assert.Equal(LogLevel.Debug, resource.Arguments.LogLevel);
    }

    [Fact]
    public void AddTemporalServer_WithNamespace_AddsNamespaces()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var temporal = builder.AddTemporalServer("temporal")
            .WithNamespace("test1", "test2", "test3");

        var resource = temporal.Resource().Resource as TemporalServerContainerResource;

        // Assert
        Assert.NotNull(resource);
        Assert.Contains("test1", resource.Arguments.Namespaces);
        Assert.Contains("test2", resource.Arguments.Namespaces);
        Assert.Contains("test3", resource.Arguments.Namespaces);
    }

    [Fact]
    public void AddTemporalServer_WithDynamicConfigValue_AddsConfigValue()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var temporal = builder.AddTemporalServer("temporal")
            .WithDynamicConfigValue("frontend.enableUpdateWorkflowExecution", true);

        var resource = temporal.Resource().Resource as TemporalServerContainerResource;

        // Assert
        Assert.NotNull(resource);
        Assert.True(resource.Arguments.DynamicConfigValues.ContainsKey("frontend.enableUpdateWorkflowExecution"));
        Assert.True((bool)resource.Arguments.DynamicConfigValues["frontend.enableUpdateWorkflowExecution"]);
    }

    [Fact]
    public void AddTemporalServer_WithDbFileName_SetsDbFileName()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var temporal = builder.AddTemporalServer("temporal")
            .WithDbFileName("/data/temporal.db");

        var resource = temporal.Resource().Resource as TemporalServerContainerResource;

        // Assert
        Assert.NotNull(resource);
        Assert.Equal("/data/temporal.db", resource.Arguments.DbFileName);
    }

    [Fact]
    public void AddTemporalServer_FluentChaining_AllowsMultipleConfigurations()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var temporal = builder.AddTemporalServer("temporal")
            .WithServiceEndpoint(7233)
            .WithUiEndpoint(8233)
            .WithMetricsEndpoint(9090)
            .WithLogFormat(LogFormat.Json)
            .WithLogLevel(LogLevel.Info)
            .WithNamespace("test1", "test2")
            .WithDynamicConfigValue("key", "value");

        var resource = temporal.Resource().Resource as TemporalServerContainerResource;

        // Assert
        Assert.NotNull(resource);
        Assert.Equal(7233, resource.Arguments.Port);
        Assert.Equal(8233, resource.Arguments.UiPort);
        Assert.Equal(9090, resource.Arguments.MetricsPort);
        Assert.Equal(LogFormat.Json, resource.Arguments.LogFormat);
        Assert.Equal(LogLevel.Info, resource.Arguments.LogLevel);
        Assert.Contains("test1", resource.Arguments.Namespaces);
        Assert.Contains("test2", resource.Arguments.Namespaces);
        Assert.Contains("key", resource.Arguments.DynamicConfigValues.Keys);
    }

    [Fact]
    public void AddTemporalServer_WithExecutable_UsesDefaultCommandOnWindows()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var temporal = builder.AddTemporalServer("temporal")
            .WithExecutable();

        var resource = temporal.Resource().Resource as TemporalServerExecutableResource;

        // Assert
        Assert.NotNull(resource);
        var expectedCommand = OperatingSystem.IsWindows() ? "temporal.exe" : "temporal";
        Assert.Equal(expectedCommand, resource.Command);
    }

    [Fact]
    public void AddTemporalServer_WithExecutable_CustomCommand_UsesProvidedCommand()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var temporal = builder.AddTemporalServer("temporal")
            .WithExecutable("/usr/local/bin/temporal", "/opt/temporal");

        var resource = temporal.Resource().Resource as TemporalServerExecutableResource;

        // Assert
        Assert.NotNull(resource);
        Assert.Equal("/usr/local/bin/temporal", resource.Command);
        Assert.Equal("/opt/temporal", resource.WorkingDirectory);
    }

    [Fact]
    public void TemporalResourceBuilder_ThrowsWhenConfiguredAfterBuild()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServer("temporal");

        // Act - Force build by accessing Resource()
        _ = temporal.Resource();

        // Assert - Should throw when trying to configure after build
        Assert.Throws<InvalidOperationException>(() => temporal.WithLogFormat(LogFormat.Json));
    }
}
