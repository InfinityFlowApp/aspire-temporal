using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using InfinityFlow.Aspire.Temporal;
using InfinityFlow.Aspire.Temporal.Annotations;
using Xunit;

namespace InfinityFlow.Aspire.Temporal.Tests;

public class TemporalFluentApiTests
{
    [Fact]
    public void WithLogFormat_AddsAnnotation()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("test")
            .WithLogFormat(LogFormat.Json);
        var annotation = temporal.Resource.Annotations.OfType<TemporalLogFormatAnnotation>().Single();
        Assert.Equal(LogFormat.Json, annotation.Format);
    }

    [Fact]
    public void WithLogLevel_AddsAnnotation()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("test")
            .WithLogLevel(LogLevel.Warn);
        var annotation = temporal.Resource.Annotations.OfType<TemporalLogLevelAnnotation>().Single();
        Assert.Equal(LogLevel.Warn, annotation.Level);
    }

    [Fact]
    public void WithNamespace_AddsMultipleAnnotations()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("test")
            .WithNamespace("ns1", "ns2");
        var annotations = temporal.Resource.Annotations.OfType<TemporalNamespaceAnnotation>().ToList();
        Assert.Equal(2, annotations.Count);
        Assert.Equal("ns1", annotations[0].Namespace);
        Assert.Equal("ns2", annotations[1].Namespace);
    }

    [Fact]
    public void WithDynamicConfigValue_AddsAnnotation()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("test")
            .WithDynamicConfigValue("key", true);
        var annotation = temporal.Resource.Annotations.OfType<TemporalDynamicConfigAnnotation>().Single();
        Assert.Equal("key", annotation.Key);
        Assert.Equal(true, annotation.Value);
    }

    [Fact]
    public void FluentChaining_WorksAcrossAllMethods()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("test")
            .WithLogFormat(LogFormat.Json)
            .WithLogLevel(LogLevel.Info)
            .WithNamespace("ns1")
            .WithDynamicConfigValue("key", true)
            .WithDbFileName("/tmp/test.db")
            .WithIp("127.0.0.1")
            .WithSQLitePragma(SQLitePragma.JournalMode);

        Assert.Single(temporal.Resource.Annotations.OfType<TemporalLogFormatAnnotation>());
        Assert.Single(temporal.Resource.Annotations.OfType<TemporalLogLevelAnnotation>());
        Assert.Single(temporal.Resource.Annotations.OfType<TemporalNamespaceAnnotation>());
        Assert.Single(temporal.Resource.Annotations.OfType<TemporalDynamicConfigAnnotation>());
        Assert.Single(temporal.Resource.Annotations.OfType<TemporalDbFileNameAnnotation>());
        Assert.Single(temporal.Resource.Annotations.OfType<TemporalIpAnnotation>());
        Assert.Single(temporal.Resource.Annotations.OfType<TemporalSQLitePragmaAnnotation>());
    }

    [Fact]
    public void FluentMethods_WorkOnExecutableResource()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerExecutable("test")
            .WithLogFormat(LogFormat.Pretty)
            .WithNamespace("ns1");
        Assert.Single(temporal.Resource.Annotations.OfType<TemporalLogFormatAnnotation>());
        Assert.Single(temporal.Resource.Annotations.OfType<TemporalNamespaceAnnotation>());
    }
}
