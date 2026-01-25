using InfinityFlow.Aspire.Temporal;

var builder = DistributedApplication.CreateBuilder(args);

// Dynamic Port Allocation (Recommended)
// Aspire automatically assigns random available ports to avoid conflicts.
// This is the recommended approach for development and testing, especially
// when running multiple instances or parallel test runs.
var temporal = builder.AddTemporalServer("temporal")
    .WithLogFormat(LogFormat.Json)
    .WithLogLevel(LogLevel.Info)
    .WithNamespace("test1", "test2")
    .WithDynamicConfigValue("frontend.enableUpdateWorkflowExecution", true);
// The server endpoint will be allocated dynamically (e.g., localhost:54321)
// The UI endpoint will be allocated dynamically (e.g., localhost:54322)

// Fixed Port Allocation (Optional)
// Explicitly specify ports when you need consistent port numbers.
// Use this when you have external tools or scripts that depend on specific ports.
var temporalWithPorts = builder.AddTemporalServer("temporalWithPorts")
    .WithServiceEndpoint(12345)  // Server gRPC endpoint will be localhost:12345
    .WithUiEndpoint(23456)        // UI endpoint will be localhost:23456
    .WithLogFormat(LogFormat.Json)
    .WithLogLevel(LogLevel.Info)
    .WithNamespace("test1", "test2")
    .WithDynamicConfigValue("frontend.enableUpdateWorkflowExecution", true);

// Example: Using a local Temporal executable instead of container
// var temporalExecutable = builder.AddTemporalServer("temporal-local")
//     .WithExecutable()  // Uses platform-specific default: temporal.exe on Windows, temporal on Linux/macOS
//     .WithLogFormat(LogFormat.Json)
//     .WithNamespace("test1");

builder.AddProject<Projects.Api>("api")
    .WithReference(temporal);

builder.AddProject<Projects.Worker>("worker")
    .WithReference(temporal);


builder.Build().Run();
