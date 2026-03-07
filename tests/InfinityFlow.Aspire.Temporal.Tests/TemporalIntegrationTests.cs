using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using InfinityFlow.Aspire.Temporal;
using Microsoft.Extensions.DependencyInjection;
using Temporalio.Api.Enums.V1;
using Temporalio.Api.OperatorService.V1;
using Temporalio.Client;
using Xunit;

namespace InfinityFlow.Aspire.Temporal.Tests;

[Collection("Integration")]
[Trait("Category", "Integration")]
public class TemporalIntegrationTests
{
    [Fact]
    public async Task SearchAttributes_AreRegisteredOnServer()
    {
        var ct = TestContext.Current.CancellationToken;
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.TestAppHost>(ct);

        var temporal = builder.AddTemporalServerContainer("temporal-search")
            .WithSearchAttribute("CustomKeyword", SearchAttributeType.Keyword)
            .WithSearchAttribute("CustomText", SearchAttributeType.Text)
            .WithSearchAttribute("CustomBool", SearchAttributeType.Bool)
            .WithSearchAttribute("CustomDatetime", SearchAttributeType.Datetime)
            .WithNamespace("integration-test");

        // Make the server endpoint non-proxied so the Temporal gRPC client can connect directly.
        temporal.WithEndpoint("server", e => { e.IsProxied = false; e.UriScheme = "http"; });

        await using var app = await builder.BuildAsync(ct);

        var rns = app.Services.GetRequiredService<ResourceNotificationService>();
        await app.StartAsync(ct);

        await rns.WaitForResourceAsync("temporal-search", KnownResourceStates.Running, ct)
            .WaitAsync(TimeSpan.FromSeconds(120), ct);

        var serverEndpoint = temporal.GetEndpoint("server");
        var uri = new Uri(serverEndpoint.Url);

        // Allow server to fully initialize search attributes
        await Task.Delay(3000, ct);

        var client = await TemporalClient.ConnectAsync(
            new TemporalClientConnectOptions($"{uri.Host}:{uri.Port}")
            {
                Namespace = "default",
            });

        var response = await client.Connection.OperatorService.ListSearchAttributesAsync(
            new ListSearchAttributesRequest { Namespace = "default" });

        Assert.Contains(response.CustomAttributes, kvp =>
            kvp.Key == "CustomKeyword" && kvp.Value == IndexedValueType.Keyword);
        Assert.Contains(response.CustomAttributes, kvp =>
            kvp.Key == "CustomText" && kvp.Value == IndexedValueType.Text);
        Assert.Contains(response.CustomAttributes, kvp =>
            kvp.Key == "CustomBool" && kvp.Value == IndexedValueType.Bool);
        Assert.Contains(response.CustomAttributes, kvp =>
            kvp.Key == "CustomDatetime" && kvp.Value == IndexedValueType.Datetime);

        await app.StopAsync(ct);
    }

    [Fact]
    public async Task Namespace_IsCreatedOnServer()
    {
        var ct = TestContext.Current.CancellationToken;
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.TestAppHost>(ct);

        var temporal = builder.AddTemporalServerContainer("temporal-ns")
            .WithNamespace("custom-ns-1", "custom-ns-2");

        temporal.WithEndpoint("server", e => { e.IsProxied = false; e.UriScheme = "http"; });

        await using var app = await builder.BuildAsync(ct);

        var rns = app.Services.GetRequiredService<ResourceNotificationService>();
        await app.StartAsync(ct);

        await rns.WaitForResourceAsync("temporal-ns", KnownResourceStates.Running, ct)
            .WaitAsync(TimeSpan.FromSeconds(120), ct);

        var serverEndpoint = temporal.GetEndpoint("server");
        var uri = new Uri(serverEndpoint.Url);

        await Task.Delay(3000, ct);

        var client = await TemporalClient.ConnectAsync(
            new TemporalClientConnectOptions($"{uri.Host}:{uri.Port}")
            {
                Namespace = "custom-ns-1",
            });

        var describeResponse = await client.WorkflowService.DescribeNamespaceAsync(
            new Temporalio.Api.WorkflowService.V1.DescribeNamespaceRequest
            {
                Namespace = "custom-ns-1",
            });

        Assert.Equal("custom-ns-1", describeResponse.NamespaceInfo.Name);

        var describeResponse2 = await client.WorkflowService.DescribeNamespaceAsync(
            new Temporalio.Api.WorkflowService.V1.DescribeNamespaceRequest
            {
                Namespace = "custom-ns-2",
            });

        Assert.Equal("custom-ns-2", describeResponse2.NamespaceInfo.Name);

        await app.StopAsync(ct);
    }
}
