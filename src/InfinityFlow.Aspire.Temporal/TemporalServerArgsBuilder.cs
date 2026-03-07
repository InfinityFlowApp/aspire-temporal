using Aspire.Hosting.ApplicationModel;
using InfinityFlow.Aspire.Temporal.Annotations;

namespace InfinityFlow.Aspire.Temporal;

/// <summary>Builds CLI arguments from resource annotations for the Temporal dev server.</summary>
public static class TemporalServerArgsBuilder
{
    /// <summary>Reads annotations from the resource and returns the CLI argument array.</summary>
    public static string[] BuildArgs(IResource resource)
    {
        var result = new List<string> { "server", "start-dev" };

        var dbFileName = resource.Annotations.OfType<TemporalDbFileNameAnnotation>().LastOrDefault();
        if (dbFileName is not null)
        {
            result.Add("--db-filename");
            result.Add(dbFileName.FileName);
        }

        var headless = resource.Annotations.OfType<TemporalHeadlessAnnotation>().LastOrDefault();
        if (headless is not null)
        {
            result.Add("--headless");
            result.Add(headless.Headless.ToString().ToLowerInvariant());
        }

        var ip = resource.Annotations.OfType<TemporalIpAnnotation>().LastOrDefault();
        result.Add("--ip");
        result.Add(ip?.Ip ?? "0.0.0.0");

        var uiIp = resource.Annotations.OfType<TemporalUiIpAnnotation>().LastOrDefault();
        if (uiIp is not null)
        {
            result.Add("--ui-ip");
            result.Add(uiIp.UiIp);
        }

        var uiAssetPath = resource.Annotations.OfType<TemporalUiAssetPathAnnotation>().LastOrDefault();
        if (uiAssetPath is not null)
        {
            result.Add("--ui-asset-path");
            result.Add(uiAssetPath.AssetPath);
        }

        var uiCodecEndpoint = resource.Annotations.OfType<TemporalUiCodecEndpointAnnotation>().LastOrDefault();
        if (uiCodecEndpoint is not null)
        {
            result.Add("--ui-codec-endpoint");
            result.Add(uiCodecEndpoint.CodecEndpoint);
        }

        var logFormat = resource.Annotations.OfType<TemporalLogFormatAnnotation>().LastOrDefault();
        if (logFormat is not null)
        {
            result.Add("--log-format");
            result.Add(EnumHelpers.LogFormatToString(logFormat.Format));
        }

        var logLevel = resource.Annotations.OfType<TemporalLogLevelAnnotation>().LastOrDefault();
        if (logLevel is not null)
        {
            result.Add("--log-level");
            result.Add(EnumHelpers.LogLevelToString(logLevel.Level));
        }

        var sqlitePragma = resource.Annotations.OfType<TemporalSQLitePragmaAnnotation>().LastOrDefault();
        if (sqlitePragma is not null)
        {
            result.Add("--sqlite-pragma");
            result.Add(EnumHelpers.SQLitePragmaToString(sqlitePragma.Pragma));
        }

        var logConfig = resource.Annotations.OfType<TemporalLogConfigAnnotation>().LastOrDefault();
        if (logConfig is not null && logConfig.Enabled)
        {
            result.Add("--log-config");
        }

        var uiPublicPath = resource.Annotations.OfType<TemporalUiPublicPathAnnotation>().LastOrDefault();
        if (uiPublicPath is not null)
        {
            result.Add("--ui-public-path");
            result.Add(uiPublicPath.PublicPath);
        }

        foreach (var ns in resource.Annotations.OfType<TemporalNamespaceAnnotation>())
        {
            result.Add("--namespace");
            result.Add(ns.Namespace);
        }

        foreach (var sa in resource.Annotations.OfType<TemporalSearchAttributeAnnotation>())
        {
            result.Add("--search-attribute");
            result.Add($"{sa.Key}={EnumHelpers.SearchAttributeTypeToString(sa.Type)}");
        }

        foreach (var dc in resource.Annotations.OfType<TemporalDynamicConfigAnnotation>())
        {
            result.Add("--dynamic-config-value");
            var formatted = dc.Value switch
            {
                string s => $"{dc.Key}=\"{s}\"",
                bool b => $"{dc.Key}={b.ToString().ToLowerInvariant()}",
                int i => $"{dc.Key}={i}",
                float f => $"{dc.Key}={f:F}",
                double d => $"{dc.Key}={d:F}",
                long l => $"{dc.Key}={l}",
                _ => throw new ArgumentException(
                    $"Unsupported dynamic config value type '{dc.Value.GetType().Name}' for key '{dc.Key}'. " +
                    "Supported types: string, bool, int, float, double, long.")
            };
            result.Add(formatted);
        }

        return [.. result];
    }
}
