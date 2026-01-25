using InfinityFlow.Aspire.Temporal;

var builder = DistributedApplication.CreateBuilder(args);

// Dynamic Port Allocation (Recommended)
// Aspire automatically assigns random available ports to avoid conflicts.
// This is the recommended approach for development and testing, especially
// when running multiple instances or parallel test runs.
var temporal = builder.AddTemporalServerContainer("temporal", x => x
    .WithLogFormat(LogFormat.Json)
    .WithLogLevel(LogLevel.Info)
    .WithNamespace("test1", "test2")
    .WithDynamicConfigValue("frontend.enableUpdateWorkflowExecution", true)
);
// The server endpoint will be allocated dynamically (e.g., localhost:54321)
// The UI endpoint will be allocated dynamically (e.g., localhost:54322)

// Fixed Port Allocation (Optional)
// Explicitly specify ports when you need consistent port numbers.
// Use this when you have external tools or scripts that depend on specific ports.
var temporalWithPorts = builder.AddTemporalServerContainer("temporalWithPorts", x => x
    .WithPort(12345)        // Server gRPC endpoint will be localhost:12345
    .WithUiPort(23456)      // UI endpoint will be localhost:23456
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
