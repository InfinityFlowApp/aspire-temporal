using Aspire.Hosting.ApplicationModel;
using InfinityFlow.Aspire.Temporal;
using InfinityFlow.Aspire.Temporal.Annotations;
using Xunit;

namespace InfinityFlow.Aspire.Temporal.Tests;

public class AnnotationTests
{
    [Fact]
    public void TemporalLogFormatAnnotation_StoresValue()
    {
        var annotation = new TemporalLogFormatAnnotation(LogFormat.Json);
        Assert.Equal(LogFormat.Json, annotation.Format);
    }

    [Fact]
    public void TemporalLogLevelAnnotation_StoresValue()
    {
        var annotation = new TemporalLogLevelAnnotation(LogLevel.Info);
        Assert.Equal(LogLevel.Info, annotation.Level);
    }

    [Fact]
    public void TemporalNamespaceAnnotation_StoresValue()
    {
        var annotation = new TemporalNamespaceAnnotation("test-ns");
        Assert.Equal("test-ns", annotation.Namespace);
    }

    [Fact]
    public void TemporalDynamicConfigAnnotation_StoresKeyAndValue()
    {
        var annotation = new TemporalDynamicConfigAnnotation("key", true);
        Assert.Equal("key", annotation.Key);
        Assert.Equal(true, annotation.Value);
    }

    [Fact]
    public void TemporalDbFileNameAnnotation_StoresValue()
    {
        var annotation = new TemporalDbFileNameAnnotation("/tmp/temporal.db");
        Assert.Equal("/tmp/temporal.db", annotation.FileName);
    }

    [Fact]
    public void TemporalHeadlessAnnotation_StoresValue()
    {
        var annotation = new TemporalHeadlessAnnotation(true);
        Assert.True(annotation.Headless);
    }

    [Fact]
    public void TemporalIpAnnotation_StoresValue()
    {
        var annotation = new TemporalIpAnnotation("127.0.0.1");
        Assert.Equal("127.0.0.1", annotation.Ip);
    }

    [Fact]
    public void TemporalSQLitePragmaAnnotation_StoresValue()
    {
        var annotation = new TemporalSQLitePragmaAnnotation(SQLitePragma.JournalMode);
        Assert.Equal(SQLitePragma.JournalMode, annotation.Pragma);
    }

    [Fact]
    public void AllAnnotations_ImplementIResourceAnnotation()
    {
        Assert.IsAssignableFrom<IResourceAnnotation>(new TemporalLogFormatAnnotation(LogFormat.Json));
        Assert.IsAssignableFrom<IResourceAnnotation>(new TemporalLogLevelAnnotation(LogLevel.Info));
        Assert.IsAssignableFrom<IResourceAnnotation>(new TemporalDbFileNameAnnotation("f"));
        Assert.IsAssignableFrom<IResourceAnnotation>(new TemporalHeadlessAnnotation(true));
        Assert.IsAssignableFrom<IResourceAnnotation>(new TemporalIpAnnotation("0.0.0.0"));
        Assert.IsAssignableFrom<IResourceAnnotation>(new TemporalUiIpAnnotation("0.0.0.0"));
        Assert.IsAssignableFrom<IResourceAnnotation>(new TemporalUiAssetPathAnnotation("/path"));
        Assert.IsAssignableFrom<IResourceAnnotation>(new TemporalUiCodecEndpointAnnotation("http://x"));
        Assert.IsAssignableFrom<IResourceAnnotation>(new TemporalSQLitePragmaAnnotation(SQLitePragma.JournalMode));
        Assert.IsAssignableFrom<IResourceAnnotation>(new TemporalNamespaceAnnotation("ns"));
        Assert.IsAssignableFrom<IResourceAnnotation>(new TemporalDynamicConfigAnnotation("k", "v"));
    }
}
