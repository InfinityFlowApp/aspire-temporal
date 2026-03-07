using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using InfinityFlow.Aspire.Temporal;
using InfinityFlow.Aspire.Temporal.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Temporalio.Api.Enums.V1;
using Temporalio.Api.OperatorService.V1;
using Temporalio.Client;
using Xunit;

namespace InfinityFlow.Aspire.Temporal.Tests;

[Trait("Category", "Integration")]
public class TemporalIntegrationTests
{
    [Fact]
    public async Task SearchAttributes_AreRegisteredOnServer()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.TestAppHost>();

        var temporal = builder.AddTemporalServerContainer("temporal")
            .WithSearchAttribute("CustomKeyword", SearchAttributeType.Keyword)
            .WithSearchAttribute("CustomText", SearchAttributeType.Text)
            .WithSearchAttribute("CustomBool", SearchAttributeType.Bool)
            .WithSearchAttribute("CustomDatetime", SearchAttributeType.Datetime)
            .WithNamespace("integration-test");

        // Add a non-proxied HTTP endpoint for direct gRPC access in tests.
        // The main HTTPS endpoint uses an Aspire TLS proxy, which the
        // Temporal gRPC client cannot easily consume in test scenarios.
        temporal.WithEndpoint(scheme: "http", targetPort: 7233, name: "grpc-direct", isProxied: false);

        await using var app = await builder.BuildAsync();

        var rns = app.Services.GetRequiredService<ResourceNotificationService>();
        await app.StartAsync();

        await rns.WaitForResourceAsync("temporal", KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromSeconds(120));

        var directEndpoint = temporal.Resource.Annotations
            .OfType<EndpointAnnotation>()
            .Single(e => e.Name == "grpc-direct");

        var address = directEndpoint.AllocatedEndpoint!.Address;
        var port = directEndpoint.AllocatedEndpoint!.Port;

        // Allow server to fully initialize search attributes
        await Task.Delay(3000);

        var client = await TemporalClient.ConnectAsync(
            new TemporalClientConnectOptions($"{address}:{port}")
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

        await app.StopAsync();
    }

    [Fact]
    public async Task Namespace_IsCreatedOnServer()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.TestAppHost>();

        var temporal = builder.AddTemporalServerContainer("temporal")
            .WithNamespace("custom-ns-1", "custom-ns-2");

        temporal.WithEndpoint(scheme: "http", targetPort: 7233, name: "grpc-direct", isProxied: false);

        await using var app = await builder.BuildAsync();

        var rns = app.Services.GetRequiredService<ResourceNotificationService>();
        await app.StartAsync();

        await rns.WaitForResourceAsync("temporal", KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromSeconds(120));

        var directEndpoint = temporal.Resource.Annotations
            .OfType<EndpointAnnotation>()
            .Single(e => e.Name == "grpc-direct");

        var address = directEndpoint.AllocatedEndpoint!.Address;
        var port = directEndpoint.AllocatedEndpoint!.Port;

        await Task.Delay(3000);

        var client = await TemporalClient.ConnectAsync(
            new TemporalClientConnectOptions($"{address}:{port}")
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

        await app.StopAsync();
    }
}
