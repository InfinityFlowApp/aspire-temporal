using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using InfinityFlow.Aspire.Temporal;
using InfinityFlow.Aspire.Temporal.Annotations;
using Xunit;

namespace InfinityFlow.Aspire.Temporal.Tests;

public class TemporalArgsBuilderTests
{
    [Fact]
    public void BuildArgs_NoAnnotations_ReturnsBaseArgs()
    {
        var resource = new TemporalServerContainerResource("test");
        var args = TemporalServerArgsBuilder.BuildArgs(resource);
        Assert.Equal(["server", "start-dev", "--ip", "0.0.0.0"], args);
    }

    [Fact]
    public void BuildArgs_WithLogFormat_IncludesFlag()
    {
        var resource = new TemporalServerContainerResource("test");
        resource.Annotations.Add(new TemporalLogFormatAnnotation(LogFormat.Json));
        var args = TemporalServerArgsBuilder.BuildArgs(resource);
        Assert.Contains("--log-format", args);
        Assert.Contains("json", args);
    }

    [Fact]
    public void BuildArgs_WithLogLevel_IncludesFlag()
    {
        var resource = new TemporalServerContainerResource("test");
        resource.Annotations.Add(new TemporalLogLevelAnnotation(LogLevel.Warn));
        var args = TemporalServerArgsBuilder.BuildArgs(resource);
        Assert.Contains("--log-level", args);
        Assert.Contains("warn", args);
    }

    [Fact]
    public void BuildArgs_WithDbFileName_IncludesFlag()
    {
        var resource = new TemporalServerContainerResource("test");
        resource.Annotations.Add(new TemporalDbFileNameAnnotation("/tmp/temporal.db"));
        var args = TemporalServerArgsBuilder.BuildArgs(resource);
        Assert.Contains("--db-filename", args);
        Assert.Contains("/tmp/temporal.db", args);
    }

    [Fact]
    public void BuildArgs_WithHeadless_IncludesFlag()
    {
        var resource = new TemporalServerContainerResource("test");
        resource.Annotations.Add(new TemporalHeadlessAnnotation(true));
        var args = TemporalServerArgsBuilder.BuildArgs(resource);
        Assert.Contains("--headless", args);
        Assert.Contains("true", args);
    }

    [Fact]
    public void BuildArgs_WithIp_OverridesDefault()
    {
        var resource = new TemporalServerContainerResource("test");
        resource.Annotations.Add(new TemporalIpAnnotation("127.0.0.1"));
        var args = TemporalServerArgsBuilder.BuildArgs(resource);
        Assert.Contains("--ip", args);
        Assert.Contains("127.0.0.1", args);
        Assert.DoesNotContain("0.0.0.0", args);
    }

    [Fact]
    public void BuildArgs_WithMultipleNamespaces_IncludesAll()
    {
        var resource = new TemporalServerContainerResource("test");
        resource.Annotations.Add(new TemporalNamespaceAnnotation("ns1"));
        resource.Annotations.Add(new TemporalNamespaceAnnotation("ns2"));
        var args = TemporalServerArgsBuilder.BuildArgs(resource);
        var nsFlags = args.Count(a => a == "--namespace");
        Assert.Equal(2, nsFlags);
        Assert.Contains("ns1", args);
        Assert.Contains("ns2", args);
    }

    [Fact]
    public void BuildArgs_WithDynamicConfig_Bool_IncludesFlag()
    {
        var resource = new TemporalServerContainerResource("test");
        resource.Annotations.Add(new TemporalDynamicConfigAnnotation("frontend.enableX", true));
        var args = TemporalServerArgsBuilder.BuildArgs(resource);
        Assert.Contains("--dynamic-config-value", args);
        Assert.Contains("frontend.enableX=true", args);
    }

    [Fact]
    public void BuildArgs_WithDynamicConfig_String_IncludesQuotedFlag()
    {
        var resource = new TemporalServerContainerResource("test");
        resource.Annotations.Add(new TemporalDynamicConfigAnnotation("key", "value"));
        var args = TemporalServerArgsBuilder.BuildArgs(resource);
        Assert.Contains("--dynamic-config-value", args);
        Assert.Contains("key=\"value\"", args);
    }

    [Fact]
    public void BuildArgs_WithDynamicConfig_Int_IncludesFlag()
    {
        var resource = new TemporalServerContainerResource("test");
        resource.Annotations.Add(new TemporalDynamicConfigAnnotation("limit", 100));
        var args = TemporalServerArgsBuilder.BuildArgs(resource);
        Assert.Contains("limit=100", args);
    }

    [Fact]
    public void BuildArgs_WithDynamicConfig_Float_IncludesFlag()
    {
        var resource = new TemporalServerContainerResource("test");
        resource.Annotations.Add(new TemporalDynamicConfigAnnotation("ratio", 1.5f));
        var args = TemporalServerArgsBuilder.BuildArgs(resource);
        Assert.Contains("--dynamic-config-value", args);
        Assert.Contains(args, a => a.StartsWith("ratio="));
    }

    [Fact]
    public void BuildArgs_WithDynamicConfig_Double_IncludesFlag()
    {
        var resource = new TemporalServerContainerResource("test");
        resource.Annotations.Add(new TemporalDynamicConfigAnnotation("ratio", 2.5d));
        var args = TemporalServerArgsBuilder.BuildArgs(resource);
        Assert.Contains("--dynamic-config-value", args);
        Assert.Contains(args, a => a.StartsWith("ratio="));
    }

    [Fact]
    public void BuildArgs_WithDynamicConfig_Long_IncludesFlag()
    {
        var resource = new TemporalServerContainerResource("test");
        resource.Annotations.Add(new TemporalDynamicConfigAnnotation("bignum", 9999999999L));
        var args = TemporalServerArgsBuilder.BuildArgs(resource);
        Assert.Contains("--dynamic-config-value", args);
        Assert.Contains("bignum=9999999999", args);
    }

    [Fact]
    public void BuildArgs_WithDynamicConfig_UnsupportedType_Throws()
    {
        var resource = new TemporalServerContainerResource("test");
        resource.Annotations.Add(new TemporalDynamicConfigAnnotation("key", new object()));
        Assert.Throws<ArgumentException>(() => TemporalServerArgsBuilder.BuildArgs(resource));
    }

    [Fact]
    public void BuildArgs_WithSQLitePragma_IncludesFlag()
    {
        var resource = new TemporalServerContainerResource("test");
        resource.Annotations.Add(new TemporalSQLitePragmaAnnotation(SQLitePragma.JournalMode));
        var args = TemporalServerArgsBuilder.BuildArgs(resource);
        Assert.Contains("--sqlite-pragma", args);
        Assert.Contains("journal_mode", args);
    }

    [Fact]
    public void BuildArgs_WithUiIp_IncludesFlag()
    {
        var resource = new TemporalServerContainerResource("test");
        resource.Annotations.Add(new TemporalUiIpAnnotation("192.168.1.1"));
        var args = TemporalServerArgsBuilder.BuildArgs(resource);
        Assert.Contains("--ui-ip", args);
        Assert.Contains("192.168.1.1", args);
    }

    [Fact]
    public void BuildArgs_WithUiAssetPath_IncludesFlag()
    {
        var resource = new TemporalServerContainerResource("test");
        resource.Annotations.Add(new TemporalUiAssetPathAnnotation("/assets"));
        var args = TemporalServerArgsBuilder.BuildArgs(resource);
        Assert.Contains("--ui-asset-path", args);
        Assert.Contains("/assets", args);
    }

    [Fact]
    public void BuildArgs_WithUiCodecEndpoint_IncludesFlag()
    {
        var resource = new TemporalServerContainerResource("test");
        resource.Annotations.Add(new TemporalUiCodecEndpointAnnotation("http://codec:8080"));
        var args = TemporalServerArgsBuilder.BuildArgs(resource);
        Assert.Contains("--ui-codec-endpoint", args);
        Assert.Contains("http://codec:8080", args);
    }
}
