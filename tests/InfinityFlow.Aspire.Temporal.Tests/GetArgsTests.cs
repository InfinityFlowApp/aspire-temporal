using InfinityFlow.Aspire.Temporal;

namespace InfinityFlow.Aspire.Temporal.Tests;

public class GetArgsTests
{
    [Fact]
    public void GetArgs_DefaultArguments_ReturnsServerStartDev()
    {
        // Arrange
        var args = new TemporalServerResourceArguments();

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("server", cmdArgs);
        Assert.Contains("start-dev", cmdArgs);
        Assert.Equal(2, cmdArgs.Where(a => a == "server" || a == "start-dev").Count());
    }

    [Fact]
    public void GetArgs_WithDbFileName_IncludesDbFilenameFlag()
    {
        // Arrange
        var args = new TemporalServerResourceArguments
        {
            DbFileName = "/data/temporal.db"
        };

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("--db-filename", cmdArgs);
        Assert.Contains("/data/temporal.db", cmdArgs);
    }

    [Fact]
    public void GetArgs_WithHttpPort_IncludesHttpPortFlag()
    {
        // Arrange
        var args = new TemporalServerResourceArguments
        {
            HttpPort = 8080
        };

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("--http-port", cmdArgs);
        Assert.Contains("8080", cmdArgs);
    }

    [Fact]
    public void GetArgs_WithMetricsPort_IncludesMetricsPortFlag()
    {
        // Arrange
        var args = new TemporalServerResourceArguments
        {
            MetricsPort = 9090
        };

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("--metrics-port", cmdArgs);
        Assert.Contains("9090", cmdArgs);
    }

    [Fact]
    public void GetArgs_WithUiPort_IncludesUiPortFlag()
    {
        // Arrange
        var args = new TemporalServerResourceArguments
        {
            UiPort = 8233
        };

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("--ui-port", cmdArgs);
        Assert.Contains("8233", cmdArgs);
    }

    [Fact]
    public void GetArgs_WithHeadlessTrue_IncludesHeadlessFlag()
    {
        // Arrange
        var args = new TemporalServerResourceArguments
        {
            Headless = true
        };

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("--headless", cmdArgs);
        Assert.Contains("true", cmdArgs);
    }

    [Fact]
    public void GetArgs_WithHeadlessFalse_IncludesHeadlessFlag()
    {
        // Arrange
        var args = new TemporalServerResourceArguments
        {
            Headless = false
        };

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("--headless", cmdArgs);
        Assert.Contains("false", cmdArgs);
    }

    [Fact]
    public void GetArgs_WithIp_IncludesIpFlag()
    {
        // Arrange
        var args = new TemporalServerResourceArguments
        {
            Ip = "192.168.1.100"
        };

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("--ip", cmdArgs);
        Assert.Contains("192.168.1.100", cmdArgs);
    }

    [Fact]
    public void GetArgs_WithDefaultIp_IncludesDefaultIpFlag()
    {
        // Arrange
        var args = new TemporalServerResourceArguments();
        // Default Ip is "0.0.0.0"

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("--ip", cmdArgs);
        Assert.Contains("0.0.0.0", cmdArgs);
    }

    [Fact]
    public void GetArgs_WithUiIp_IncludesUiIpFlag()
    {
        // Arrange
        var args = new TemporalServerResourceArguments
        {
            UiIp = "192.168.1.101"
        };

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("--ui-ip", cmdArgs);
        Assert.Contains("192.168.1.101", cmdArgs);
    }

    [Fact]
    public void GetArgs_WithUiAssetPath_IncludesUiAssetPathFlag()
    {
        // Arrange
        var args = new TemporalServerResourceArguments
        {
            UiAssetPath = "/custom/ui/assets"
        };

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("--ui-asset-path", cmdArgs);
        Assert.Contains("/custom/ui/assets", cmdArgs);
    }

    [Fact]
    public void GetArgs_WithUiCodecEndpoint_IncludesUiCodecEndpointFlag()
    {
        // Arrange
        var args = new TemporalServerResourceArguments
        {
            UiCodecEndpoint = "http://localhost:8081/codec"
        };

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("--ui-codec-endpoint", cmdArgs);
        Assert.Contains("http://localhost:8081/codec", cmdArgs);
    }

    [Theory]
    [InlineData(LogFormat.Json, "json")]
    [InlineData(LogFormat.Pretty, "pretty")]
    public void GetArgs_WithLogFormat_IncludesLogFormatFlag(LogFormat format, string expected)
    {
        // Arrange
        var args = new TemporalServerResourceArguments
        {
            LogFormat = format
        };

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("--log-format", cmdArgs);
        Assert.Contains(expected, cmdArgs);
    }

    [Theory]
    [InlineData(LogLevel.Debug, "debug")]
    [InlineData(LogLevel.Info, "info")]
    [InlineData(LogLevel.Warn, "warn")]
    [InlineData(LogLevel.Error, "error")]
    [InlineData(LogLevel.Fatal, "fatal")]
    public void GetArgs_WithLogLevel_IncludesLogLevelFlag(LogLevel level, string expected)
    {
        // Arrange
        var args = new TemporalServerResourceArguments
        {
            LogLevel = level
        };

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("--log-level", cmdArgs);
        Assert.Contains(expected, cmdArgs);
    }

    [Theory]
    [InlineData(SQLitePragma.JournalMode, "journal_mode=WAL")]
    [InlineData(SQLitePragma.Synchronous, "synchronous=NORMAL")]
    public void GetArgs_WithSQLitePragma_IncludesSQLitePragmaFlag(SQLitePragma pragma, string expected)
    {
        // Arrange
        var args = new TemporalServerResourceArguments
        {
            SQLitePragma = pragma
        };

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("--sqlite-pragma", cmdArgs);
        // Temporal CLI expects pragma=value format
        Assert.Contains(expected, cmdArgs);
    }

    [Fact]
    public void GetArgs_WithSingleNamespace_IncludesNamespaceFlag()
    {
        // Arrange
        var args = new TemporalServerResourceArguments();
        args.Namespaces.Add("production");

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("--namespace", cmdArgs);
        Assert.Contains("production", cmdArgs);
    }

    [Fact]
    public void GetArgs_WithMultipleNamespaces_IncludesAllNamespaceFlags()
    {
        // Arrange
        var args = new TemporalServerResourceArguments();
        args.Namespaces.AddRange(new[] { "production", "staging", "development" });

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        var namespaceCount = cmdArgs.Count(a => a == "--namespace");
        Assert.Equal(3, namespaceCount);
        Assert.Contains("production", cmdArgs);
        Assert.Contains("staging", cmdArgs);
        Assert.Contains("development", cmdArgs);
    }

    [Fact]
    public void GetArgs_WithDynamicConfigString_IncludesQuotedValue()
    {
        // Arrange
        var args = new TemporalServerResourceArguments();
        args.DynamicConfigValues.Add("frontend.enableUpdateWorkflowExecution", "enabled");

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("--dynamic-config-value", cmdArgs);
        // String values are formatted with quotes and spaces: key= "value"
        var configArg = cmdArgs.FirstOrDefault(a => a.StartsWith("frontend.enableUpdateWorkflowExecution="));
        Assert.NotNull(configArg);
        Assert.Equal("frontend.enableUpdateWorkflowExecution= \"enabled\" ", configArg);
    }

    [Fact]
    public void GetArgs_WithDynamicConfigBool_IncludesLowercaseValue()
    {
        // Arrange
        var args = new TemporalServerResourceArguments();
        args.DynamicConfigValues.Add("feature.enabled", true);

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("--dynamic-config-value", cmdArgs);
        Assert.Contains("feature.enabled=true", cmdArgs);
    }

    [Fact]
    public void GetArgs_WithDynamicConfigInt_IncludesNumericValue()
    {
        // Arrange
        var args = new TemporalServerResourceArguments();
        args.DynamicConfigValues.Add("limit.maxWorkflows", 1000);

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("--dynamic-config-value", cmdArgs);
        Assert.Contains("limit.maxWorkflows=1000", cmdArgs);
    }

    [Fact]
    public void GetArgs_WithDynamicConfigFloat_IncludesFormattedValue()
    {
        // Arrange
        var args = new TemporalServerResourceArguments();
        args.DynamicConfigValues.Add("threshold.value", 0.75f);

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("--dynamic-config-value", cmdArgs);
        var configArg = cmdArgs.FirstOrDefault(a => a.StartsWith("threshold.value="));
        Assert.NotNull(configArg);
        Assert.Contains("0.75", configArg); // Float formatted with "F"
    }

    [Fact]
    public void GetArgs_WithMultipleDynamicConfigs_IncludesAllFlags()
    {
        // Arrange
        var args = new TemporalServerResourceArguments();
        args.DynamicConfigValues.Add("config1", "value1");
        args.DynamicConfigValues.Add("config2", true);
        args.DynamicConfigValues.Add("config3", 42);

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        var configCount = cmdArgs.Count(a => a == "--dynamic-config-value");
        Assert.Equal(3, configCount);
    }

    [Fact]
    public void GetArgs_WithUnsupportedDynamicConfigType_ThrowsArgumentException()
    {
        // Arrange
        var args = new TemporalServerResourceArguments();
        args.DynamicConfigValues.Add("invalid", new { value = "object" });

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => args.GetArgs());
        Assert.Contains("Unsupported type", exception.Message);
        Assert.Contains("invalid", exception.Message);
    }

    [Fact]
    public void GetArgs_WithAllFlags_IncludesAllArguments()
    {
        // Arrange
        var args = new TemporalServerResourceArguments
        {
            DbFileName = "/data/temporal.db",
            Port = 7233,
            HttpPort = 7234,
            MetricsPort = 7235,
            UiPort = 8233,
            Headless = false,
            Ip = "0.0.0.0",
            UiIp = "0.0.0.0",
            UiAssetPath = "/assets",
            UiCodecEndpoint = "http://codec",
            LogFormat = LogFormat.Json,
            LogLevel = LogLevel.Info,
            SQLitePragma = SQLitePragma.JournalMode
        };
        args.Namespaces.Add("test1");
        args.Namespaces.Add("test2");
        args.DynamicConfigValues.Add("key1", "value1");
        args.DynamicConfigValues.Add("key2", true);

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.Contains("server", cmdArgs);
        Assert.Contains("start-dev", cmdArgs);
        Assert.Contains("--db-filename", cmdArgs);
        Assert.Contains("--port", cmdArgs);
        Assert.Contains("--http-port", cmdArgs);
        Assert.Contains("--metrics-port", cmdArgs);
        Assert.Contains("--ui-port", cmdArgs);
        Assert.Contains("--headless", cmdArgs);
        Assert.Contains("--ip", cmdArgs);
        Assert.Contains("--ui-ip", cmdArgs);
        Assert.Contains("--ui-asset-path", cmdArgs);
        Assert.Contains("--ui-codec-endpoint", cmdArgs);
        Assert.Contains("--log-format", cmdArgs);
        Assert.Contains("--log-level", cmdArgs);
        Assert.Contains("--sqlite-pragma", cmdArgs);
        Assert.Equal(2, cmdArgs.Count(a => a == "--namespace"));
        Assert.Equal(2, cmdArgs.Count(a => a == "--dynamic-config-value"));
    }

    [Fact]
    public void GetArgs_WithNullOptionalFields_OmitsThoseFlags()
    {
        // Arrange
        var args = new TemporalServerResourceArguments
        {
            DbFileName = null,
            Port = null,
            HttpPort = null,
            MetricsPort = null,
            UiPort = null,
            Headless = null,
            UiIp = null,
            UiAssetPath = null,
            UiCodecEndpoint = null,
            LogFormat = null,
            LogLevel = null,
            SQLitePragma = null
        };

        // Act
        var cmdArgs = args.GetArgs();

        // Assert
        Assert.DoesNotContain("--db-filename", cmdArgs);
        Assert.DoesNotContain("--port", cmdArgs);
        Assert.DoesNotContain("--http-port", cmdArgs);
        Assert.DoesNotContain("--metrics-port", cmdArgs);
        Assert.DoesNotContain("--ui-port", cmdArgs);
        Assert.DoesNotContain("--headless", cmdArgs);
        Assert.DoesNotContain("--ui-ip", cmdArgs);
        Assert.DoesNotContain("--ui-asset-path", cmdArgs);
        Assert.DoesNotContain("--ui-codec-endpoint", cmdArgs);
        Assert.DoesNotContain("--log-format", cmdArgs);
        Assert.DoesNotContain("--log-level", cmdArgs);
        Assert.DoesNotContain("--sqlite-pragma", cmdArgs);
        // IP should still be present (has default value)
        Assert.Contains("--ip", cmdArgs);
    }
}
