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

    [Fact]
    public void WithUiIp_AddsAnnotation()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("test")
            .WithUiIp("192.168.1.1");
        var annotation = temporal.Resource.Annotations.OfType<TemporalUiIpAnnotation>().Single();
        Assert.Equal("192.168.1.1", annotation.UiIp);
    }

    [Fact]
    public void WithUiAssetPath_AddsAnnotation()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("test")
            .WithUiAssetPath("/custom/assets");
        var annotation = temporal.Resource.Annotations.OfType<TemporalUiAssetPathAnnotation>().Single();
        Assert.Equal("/custom/assets", annotation.AssetPath);
    }

    [Fact]
    public void WithUiCodecEndpoint_AddsAnnotation()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("test")
            .WithUiCodecEndpoint("http://codec:8080");
        var annotation = temporal.Resource.Annotations.OfType<TemporalUiCodecEndpointAnnotation>().Single();
        Assert.Equal("http://codec:8080", annotation.CodecEndpoint);
    }

    [Fact]
    public void WithHeadlessUi_AddsAnnotation_OnExecutable()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerExecutable("test")
            .WithHeadlessUi();
        var annotation = temporal.Resource.Annotations.OfType<TemporalHeadlessAnnotation>().Single();
        Assert.True(annotation.Headless);
    }

    [Fact]
    public void WithLogConfig_AddsAnnotation()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("test")
            .WithLogConfig();
        var annotation = temporal.Resource.Annotations.OfType<TemporalLogConfigAnnotation>().Single();
        Assert.True(annotation.Enabled);
    }

    [Fact]
    public void WithSearchAttribute_AddsAnnotation()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("test")
            .WithSearchAttribute("MyAttr", SearchAttributeType.Keyword);
        var annotation = temporal.Resource.Annotations.OfType<TemporalSearchAttributeAnnotation>().Single();
        Assert.Equal("MyAttr", annotation.Key);
        Assert.Equal(SearchAttributeType.Keyword, annotation.Type);
    }

    [Fact]
    public void WithSearchAttribute_SupportsMultiple()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("test")
            .WithSearchAttribute("Attr1", SearchAttributeType.Text)
            .WithSearchAttribute("Attr2", SearchAttributeType.Int);
        var annotations = temporal.Resource.Annotations.OfType<TemporalSearchAttributeAnnotation>().ToList();
        Assert.Equal(2, annotations.Count);
    }

    [Fact]
    public void WithUiPublicPath_AddsAnnotation()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("test")
            .WithUiPublicPath("/temporal");
        var annotation = temporal.Resource.Annotations.OfType<TemporalUiPublicPathAnnotation>().Single();
        Assert.Equal("/temporal", annotation.PublicPath);
    }

    [Fact]
    public void WithDataVolume_AddsDbFileNameAnnotation()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("test")
            .WithDataVolume();
        var annotation = temporal.Resource.Annotations.OfType<TemporalDbFileNameAnnotation>().Single();
        Assert.Equal("/data/temporal.db", annotation.FileName);
    }

    [Fact]
    public void WithDataVolume_AddsVolumeAnnotation()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("test")
            .WithDataVolume();
        var volumes = temporal.Resource.Annotations.OfType<ContainerMountAnnotation>()
            .Where(a => a.Target == "/data")
            .ToList();
        Assert.Single(volumes);
        Assert.Equal(ContainerMountType.Volume, volumes[0].Type);
    }

    [Fact]
    public void WithDataVolume_CustomName_UsesProvidedName()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("test")
            .WithDataVolume("my-custom-volume");
        var volume = temporal.Resource.Annotations.OfType<ContainerMountAnnotation>()
            .Single(a => a.Target == "/data");
        Assert.Equal("my-custom-volume", volume.Source);
    }

    [Fact]
    public void WithDataBindMount_AddsDbFileNameAnnotation()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("test")
            .WithDataBindMount("/host/data");
        var annotation = temporal.Resource.Annotations.OfType<TemporalDbFileNameAnnotation>().Single();
        Assert.Equal("/data/temporal.db", annotation.FileName);
    }

    [Fact]
    public void WithDataBindMount_AddsBindMountAnnotation()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("test")
            .WithDataBindMount("/host/data");
        var mount = temporal.Resource.Annotations.OfType<ContainerMountAnnotation>()
            .Single(a => a.Target == "/data");
        Assert.Equal(ContainerMountType.BindMount, mount.Type);
        Assert.Equal("/host/data", mount.Source);
    }

    [Fact]
    public void WithDataVolume_RespectsExistingDbFileName()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("test")
            .WithDbFileName("/custom/path.db")
            .WithDataVolume();
        var annotation = temporal.Resource.Annotations.OfType<TemporalDbFileNameAnnotation>().Single();
        Assert.Equal("/custom/path.db", annotation.FileName);
        var volume = temporal.Resource.Annotations.OfType<ContainerMountAnnotation>()
            .Single(a => a.Type == ContainerMountType.Volume);
        Assert.Equal("/custom", volume.Target);
    }

    [Fact]
    public void WithDataBindMount_RespectsExistingDbFileName()
    {
        var builder = DistributedApplication.CreateBuilder();
        var temporal = builder.AddTemporalServerContainer("test")
            .WithDbFileName("/custom/path.db")
            .WithDataBindMount("/host/data");
        var annotation = temporal.Resource.Annotations.OfType<TemporalDbFileNameAnnotation>().Single();
        Assert.Equal("/custom/path.db", annotation.FileName);
        var mount = temporal.Resource.Annotations.OfType<ContainerMountAnnotation>()
            .Single(a => a.Type == ContainerMountType.BindMount);
        Assert.Equal("/custom", mount.Target);
    }
}
