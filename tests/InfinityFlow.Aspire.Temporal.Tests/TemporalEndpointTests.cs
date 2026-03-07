using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using InfinityFlow.Aspire.Temporal;
using InfinityFlow.Aspire.Temporal.Annotations;
using Xunit;

namespace InfinityFlow.Aspire.Temporal.Tests;

public class TemporalEndpointTests
{
    [Fact]
    public void AddTemporalServerContainer_DefaultPorts_HasServerAndUiEndpoints()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("temporal");

        var serverEndpoint = temporal.Resource.Annotations.OfType<EndpointAnnotation>()
            .SingleOrDefault(e => e.Name == "server");
        var uiEndpoint = temporal.Resource.Annotations.OfType<EndpointAnnotation>()
            .SingleOrDefault(e => e.Name == "ui");

        Assert.NotNull(serverEndpoint);
        Assert.Equal(7233, serverEndpoint.TargetPort);
        Assert.Null(serverEndpoint.Port);

        Assert.NotNull(uiEndpoint);
        Assert.Equal(8233, uiEndpoint.TargetPort);
        Assert.Null(uiEndpoint.Port);
    }

    [Fact]
    public void AddTemporalServerContainer_WithServicePort_SetsFixedPort()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("temporal")
            .WithServicePort(7233);

        var serverEndpoint = temporal.Resource.Annotations.OfType<EndpointAnnotation>()
            .Single(e => e.Name == "server");
        Assert.Equal(7233, serverEndpoint.Port);
        Assert.Equal(7233, serverEndpoint.TargetPort);
    }

    [Fact]
    public void AddTemporalServerContainer_WithUiPort_SetsFixedPort()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("temporal")
            .WithUiPort(8233);

        var uiEndpoint = temporal.Resource.Annotations.OfType<EndpointAnnotation>()
            .Single(e => e.Name == "ui");
        Assert.Equal(8233, uiEndpoint.Port);
        Assert.Equal(8233, uiEndpoint.TargetPort);
    }

    [Fact]
    public void AddTemporalServerContainer_WithHeadless_NoUiEndpoint()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("temporal")
            .WithHeadlessUi();

        var uiEndpoint = temporal.Resource.Annotations.OfType<EndpointAnnotation>()
            .SingleOrDefault(e => e.Name == "ui");
        Assert.Null(uiEndpoint);
    }

    [Fact]
    public void AddTemporalServerContainer_WithMetricsEndpoint_AddsEndpoint()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("temporal")
            .WithMetricsEndpoint(9090);

        var metricsEndpoint = temporal.Resource.Annotations.OfType<EndpointAnnotation>()
            .SingleOrDefault(e => e.Name == "metrics");
        Assert.NotNull(metricsEndpoint);
        Assert.Equal(9090, metricsEndpoint.Port);
        Assert.Equal(7235, metricsEndpoint.TargetPort);
    }

    [Fact]
    public void AddTemporalServerContainer_WithHttpPort_AddsEndpoint()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("temporal")
            .WithHttpPort(7234);

        var httpEndpoint = temporal.Resource.Annotations.OfType<EndpointAnnotation>()
            .SingleOrDefault(e => e.Name == "http");
        Assert.NotNull(httpEndpoint);
        Assert.Equal(7234, httpEndpoint.Port);
        Assert.Equal(7234, httpEndpoint.TargetPort);
    }

    [Fact]
    public void AddTemporalServerContainer_ArgsPassedToContainer()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("temporal")
            .WithLogFormat(LogFormat.Json)
            .WithNamespace("test1");

        var argsAnnotations = temporal.Resource.Annotations.OfType<CommandLineArgsCallbackAnnotation>();
        Assert.NotEmpty(argsAnnotations);
    }

    [Fact]
    public void AddTemporalServerContainer_DefaultImage_IsPinned()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("temporal");

        var imageAnnotation = temporal.Resource.Annotations.OfType<ContainerImageAnnotation>().Single();
        Assert.Equal("temporalio/admin-tools", imageAnnotation.Image);
        Assert.Equal(TemporalServerContainerBuilderExtensions.TemporalServerImageTag, imageAnnotation.Tag);
    }

    [Fact]
    public void AddTemporalServerExecutable_DefaultPorts_HasServerAndUiEndpoints()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerExecutable("temporal");

        var serverEndpoint = temporal.Resource.Annotations.OfType<EndpointAnnotation>()
            .SingleOrDefault(e => e.Name == "server");
        var uiEndpoint = temporal.Resource.Annotations.OfType<EndpointAnnotation>()
            .SingleOrDefault(e => e.Name == "ui");

        Assert.NotNull(serverEndpoint);
        Assert.Null(serverEndpoint.Port);
        Assert.NotNull(uiEndpoint);
        Assert.Null(uiEndpoint.Port);
    }

    [Fact]
    public void AddTemporalServerExecutable_WithServicePort_SetsFixedPort()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerExecutable("temporal")
            .WithServicePort(7233);

        var serverEndpoint = temporal.Resource.Annotations.OfType<EndpointAnnotation>()
            .Single(e => e.Name == "server");
        Assert.Equal(7233, serverEndpoint.Port);
    }

    [Fact]
    public void AddTemporalServerExecutable_WithUiPort_SetsFixedPort()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerExecutable("temporal")
            .WithUiPort(8233);

        var uiEndpoint = temporal.Resource.Annotations.OfType<EndpointAnnotation>()
            .Single(e => e.Name == "ui");
        Assert.Equal(8233, uiEndpoint.Port);
    }

    [Fact]
    public void AddTemporalServerExecutable_WithMetricsEndpoint_AddsEndpoint()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerExecutable("temporal")
            .WithMetricsEndpoint(9090);

        var metricsEndpoint = temporal.Resource.Annotations.OfType<EndpointAnnotation>()
            .SingleOrDefault(e => e.Name == "metrics");
        Assert.NotNull(metricsEndpoint);
        Assert.Equal(9090, metricsEndpoint.Port);
        Assert.Null(metricsEndpoint.TargetPort);
    }

    [Fact]
    public void AddTemporalServerExecutable_WithHttpPort_AddsEndpoint()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerExecutable("temporal")
            .WithHttpPort(7234);

        var httpEndpoint = temporal.Resource.Annotations.OfType<EndpointAnnotation>()
            .SingleOrDefault(e => e.Name == "http");
        Assert.NotNull(httpEndpoint);
        Assert.Equal(7234, httpEndpoint.Port);
        Assert.Null(httpEndpoint.TargetPort);
    }

    [Fact]
    public void AddTemporalServerExecutable_WithHeadlessUi_NoUiEndpoint()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerExecutable("temporal")
            .WithHeadlessUi();

        var uiEndpoint = temporal.Resource.Annotations.OfType<EndpointAnnotation>()
            .SingleOrDefault(e => e.Name == "ui");
        Assert.Null(uiEndpoint);
    }

    [Fact]
    public void AddTemporalServerExecutable_ArgsPassedToExecutable()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerExecutable("temporal")
            .WithLogFormat(LogFormat.Json);

        var argsAnnotations = temporal.Resource.Annotations.OfType<CommandLineArgsCallbackAnnotation>();
        Assert.NotEmpty(argsAnnotations);
    }

}
