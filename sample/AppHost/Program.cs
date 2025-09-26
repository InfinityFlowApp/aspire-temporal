using InfinityFlow.Aspire.Temporal;

var builder = DistributedApplication.CreateBuilder(args);

var temporal = builder.AddTemporalServerContainer("temporal", x => x
    .WithLogFormat(LogFormat.Json)
    .WithLogLevel(LogLevel.Info)
    .WithNamespace("test1", "test2")
    .WithDynamicConfigValue("frontend.enableUpdateWorkflowExecution", true)
);

var temporalWithPorts = builder.AddTemporalServerContainer("temporalWithPorts", x => x
    .WithPort(12345)
    .WithUiPort(23456)
    .WithLogFormat(LogFormat.Json)
    .WithLogLevel(LogLevel.Info)
    .WithNamespace("test1", "test2")
    .WithDynamicConfigValue("frontend.enableUpdateWorkflowExecution", true)
);

builder.AddProject<Projects.Api>("api")
    .WithReference(temporal);

builder.AddProject<Projects.Worker>("worker")
    .WithReference(temporal);


builder.Build().Run();
