using Aspire.Temporal.Server;

var builder = DistributedApplication.CreateBuilder(args);

var temporal = await builder.AddTemporalServerContainer("temporal", x => x
    .WithLogFormat(LogFormat.Json)
    .WithLogLevel(LogLevel.Info)
    .WithNamespace("test1", "test2"));

builder.AddProject<Projects.Api>("api")
    .WithReference(temporal);

builder.AddProject<Projects.Worker>("worker")
    .WithReference(temporal);

builder.Build().Run();