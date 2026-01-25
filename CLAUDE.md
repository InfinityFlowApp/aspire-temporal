# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

InfinityFlow.Aspire.Temporal is a .NET Aspire extension suite that provides:

1. **Server Hosting** (`InfinityFlow.Aspire.Temporal`) - Run Temporal server as container (default) or executable
2. **Client Integration** (`InfinityFlow.Aspire.Temporal.Client`) - Simplified client/worker setup with automatic OpenTelemetry

The library follows Aspire best practices with dynamic port allocation, fluent APIs, and seamless observability integration.

**Note:** Container deployment is the recommended approach. Executable deployment has known limitations (see https://github.com/dotnet/aspire/issues/1637 and https://github.com/temporalio/cli/issues/316).

## Build and Development Commands

### Build the solution
```bash
dotnet build InfinityFlow.Aspire.Temporal.sln
```

### Build specific project
```bash
dotnet build src/InfinityFlow.Aspire.Temporal/InfinityFlow.Aspire.Temporal.csproj
```

### Pack NuGet package
```bash
dotnet pack src/InfinityFlow.Aspire.Temporal/InfinityFlow.Aspire.Temporal.csproj
```

### Run sample application
```bash
dotnet run --project sample/AppHost/AppHost.csproj
```

### Manage package references
```bash
# Add package reference
dotnet add <project.csproj> package <PackageName>

# Update package reference
dotnet add <project.csproj> package <PackageName>

# Remove package reference
dotnet remove <project.csproj> package <PackageName>
```

## Architecture

### Packages

1. **InfinityFlow.Aspire.Temporal** - Server hosting integration
   - Target: net10.0
   - Dependencies: Aspire.Hosting.AppHost 13.1.0
   - Dynamic port allocation by default (following Aspire patterns)

2. **InfinityFlow.Aspire.Temporal.Client** - Client/worker integration
   - Target: net10.0
   - Dependencies: Temporalio 1.4.0, OpenTelemetry.Extensions.Hosting 1.11.0
   - Automatic OpenTelemetry tracing and metrics configuration

### Server Hosting Modes

1. **Container Resource (Default)** - Uses `temporalio/admin-tools:1.28.2-tctl-1.18.1-cli-1.1.1` Docker image
2. **Executable Resource** - Runs local Temporal CLI binary with platform-aware defaults

Both modes share the same configuration model through `TemporalServerResourceArguments`.

### Server Hosting - Key Classes

- **TemporalBuilderExtensions** (src/InfinityFlow.Aspire.Temporal/TemporalBuilderExtensions.cs)
  - **NEW**: `AddTemporalServer()` - Unified entry point (replaces AddTemporalServerContainer/Executable)
  - Returns `TemporalResourceBuilder` for fluent configuration
  - Extension method for `WithReference()` to seamlessly integrate with Aspire projects

- **TemporalResourceBuilder** (src/InfinityFlow.Aspire.Temporal/TemporalBuilderExtensions.cs:36)
  - **NEW**: Simplified fluent builder without lambda callbacks
  - Semantic methods: `WithServiceEndpoint()`, `WithUiEndpoint()`, `WithMetricsEndpoint()`
  - `WithExecutable()` - Switch to executable mode (defaults: temporal.exe on Windows, temporal on Linux/macOS)
  - Lazy resource building prevents double-registration issues
  - Throws `InvalidOperationException` if configured after resource is built

- **TemporalServerResourceBuilder** (src/InfinityFlow.Aspire.Temporal/TemporalServerResourceBuilder.cs:5)
  - **LEGACY**: Original fluent builder (still supported for backward compatibility)
  - Used internally by lambda callback pattern
  - Returns `TemporalServerResourceArguments` via `Build()`

- **TemporalServerResourceArguments** (src/InfinityFlow.Aspire.Temporal/TemporalServerResourceArguments.cs:6)
  - Configuration model with nullable ports (enables dynamic allocation)
  - `GetArgs()` converts properties to CLI arguments for `temporal server start-dev`
  - Validates dynamic config value types (string, bool, int, float, double, long)
  - Throws `ArgumentException` for unsupported types (prevents silent failures)

- **TemporalServerContainerBuilderExtensions** (src/InfinityFlow.Aspire.Temporal/TemporalServerContainerBuilderExtensions.cs:6)
  - **LEGACY**: Original `AddTemporalServerContainer()` with lambda callbacks
  - Still supported for backward compatibility

- **Resource Classes** (src/InfinityFlow.Aspire.Temporal/TemporalServerExecutableResource.cs)
  - `TemporalServerContainerResource` - inherits `ContainerResource`, implements `IResourceWithConnectionString`
  - `TemporalServerExecutableResource` - inherits `ExecutableResource`, implements `IResourceWithConnectionString`
  - Connection string resolves to server endpoint host:port

### Client Integration - Key Classes

- **AspireTemporalExtensions** (src/InfinityFlow.Aspire.Temporal.Client/AspireTemporalExtensions.cs:18)
  - `AddTemporalClient()` - Register Temporal client with automatic connection string resolution
  - `AddTemporalWorker(taskQueue)` - Register Temporal worker with hosted service
  - Returns `TemporalClientBuilder` for fluent configuration

- **TemporalClientBuilder** (src/InfinityFlow.Aspire.Temporal.Client/AspireTemporalExtensions.cs:61)
  - Fluent API for configuring interceptors, tracing, and client options
  - `WithInterceptors()` - Add custom interceptors (TracingInterceptor added automatically)
  - `WithTracingSources()` - Configure OpenTelemetry activity sources
  - `ConfigureOptions()` - Direct access to `TemporalClientConnectOptions`
  - `WithoutTracing()` / `WithoutMetrics()` - Disable automatic observability
  - `.Services` property provides access to `IServiceCollection`
  - `.Worker` property provides access to `ITemporalWorkerServiceOptionsBuilder` (worker mode only)
  - **Lazy registration pattern**: Services are registered on first property access
  - **Error handling**: All user callbacks wrapped in try-catch with contextual error messages

- **TemporalServiceDefaultsExtensions** (src/InfinityFlow.Aspire.Temporal.Client/TemporalServiceDefaultsExtensions.cs:9)
  - `AddTemporalTracing()` - Register OpenTelemetry activity sources in service defaults
  - `AddTemporalMetrics()` - Register OpenTelemetry meter in service defaults
  - Call these in ServiceDefaults/Extensions.cs for centralized observability configuration

### Configuration Patterns

#### Server Hosting - Simplified API (Recommended)

Direct fluent builder without lambda callbacks:
```csharp
// Container mode (default) with dynamic ports
var temporal = builder.AddTemporalServer("temporal")
    .WithLogFormat(LogFormat.Json)
    .WithLogLevel(LogLevel.Info)
    .WithNamespace("ns1", "ns2")
    .WithDynamicConfigValue("frontend.enableUpdateWorkflowExecution", true);

// Container mode with fixed ports
var temporal = builder.AddTemporalServer("temporal")
    .WithServiceEndpoint(7233)    // gRPC server
    .WithUiEndpoint(8233)          // Web UI
    .WithMetricsEndpoint(9090)     // Metrics endpoint
    .WithLogFormat(LogFormat.Json);

// Executable mode (local Temporal CLI)
var temporal = builder.AddTemporalServer("temporal")
    .WithExecutable()  // Uses temporal.exe on Windows, temporal on Linux/macOS
    .WithLogFormat(LogFormat.Json)
    .WithNamespace("test");

// Executable mode with custom command/directory
var temporal = builder.AddTemporalServer("temporal")
    .WithExecutable("/usr/local/bin/temporal", "/opt/temporal")
    .WithLogFormat(LogFormat.Json);

// Reference in projects
builder.AddProject<Projects.Worker>("worker")
    .WithReference(temporal);  // Works seamlessly with extension method
```

#### Server Hosting - Legacy API (Backward Compatible)

Lambda callback pattern (still supported):
```csharp
var temporal = builder.AddTemporalServerContainer("temporal", x => x
    .WithPort(7233)
    .WithUiPort(8233)
    .WithLogFormat(LogFormat.Json)
    .WithLogLevel(LogLevel.Info)
    .WithNamespace("ns1", "ns2")
    .WithDynamicConfigValue("key", value));
```

#### Client Integration

Fluent API with automatic observability:
```csharp
// In AppHost - reference the server
var temporal = builder.AddTemporalServer("temporal");
builder.AddProject<Projects.Worker>("worker")
    .WithReference(temporal);

// In Worker - automatic connection string resolution and OpenTelemetry
var clientBuilder = builder.AddTemporalWorker("my-task-queue")
    .ConfigureOptions(opts => opts.Namespace = "production")
    .WithInterceptors(interceptors => {
        interceptors.Add(new MyCustomInterceptor());
    })
    .WithTracingSources(sources => {
        sources.Add("MyApp.CustomSource");
    });

// Access services to add workflows/activities
clientBuilder.Worker!
    .AddScopedActivities<MyActivities>()
    .AddWorkflow<MyWorkflow>();

// Or for client-only (no worker)
builder.AddTemporalClient()
    .ConfigureOptions(opts => opts.Namespace = "default")
    .WithoutTracing()    // Optional: disable tracing
    .WithoutMetrics();   // Optional: disable metrics
```

All `temporal server start-dev` CLI flags map to builder methods.

### Enums

Three enums define Temporal CLI options (src/InfinityFlow.Aspire.Temporal/Enums.cs):
- `LogFormat`: Json, Pretty
- `LogLevel`: Debug, Info, Warn, Error, Fatal
- `SQLitePragma`: JournalMode, Synchronous

`EnumHelpers` class converts enums to CLI string values.

## Project Structure

- **src/InfinityFlow.Aspire.Temporal/** - Server hosting library
  - Target framework: net10.0
  - NuGet package ID: InfinityFlow.Aspire.Temporal
  - Dependencies: Aspire.Hosting.AppHost 13.1.0

- **src/InfinityFlow.Aspire.Temporal.Client/** - Client/worker integration library
  - Target framework: net10.0
  - NuGet package ID: InfinityFlow.Aspire.Temporal.Client
  - Dependencies: Temporalio 1.4.0, OpenTelemetry.Extensions.Hosting 1.11.0

- **sample/** - Example Aspire application demonstrating usage
  - **AppHost/** - Aspire orchestration project showing both container and executable resources
  - **Worker/** - Example Temporal worker with workflow and activity implementations
  - **Api/** - Example API client
  - **ServiceDefaults/** - Shared service configuration (includes Temporal observability setup)

## Temporal Integration

### Connection String

Both resources implement `IResourceWithConnectionString`. The connection string is automatically injected as `ConnectionStrings:<resource-name>` in referenced projects.

Example: A resource named "temporal" exposes `builder.Configuration["ConnectionStrings:temporal"]` which resolves to the gRPC server endpoint (e.g., "localhost:7233").

### Observability

**Automatic Integration (Recommended)**

The client library configures OpenTelemetry automatically:
- **TracingInterceptor** added automatically (unless `WithoutTracing()` called)
- **CustomMetricMeter** configured with proper DI lifecycle
- **Activity sources** registered: `Temporalio.Client`, `Temporalio.Workflows`, `Temporalio.Activities`
- All telemetry flows to Aspire dashboard automatically

No manual configuration needed when using `AddTemporalClient()` or `AddTemporalWorker()`.

**Manual Integration (Advanced)**

For service defaults configuration:
```csharp
// In ServiceDefaults/Extensions.cs
builder.Services.ConfigureOpenTelemetryTracerProvider(tracing =>
    tracing.AddTemporalTracing());

builder.Services.ConfigureOpenTelemetryMeterProvider(metrics =>
    metrics.AddTemporalMetrics());
```

**Critical Implementation Details**:
- `Meter` instances must be registered as singleton in DI for proper disposal
- `TracingInterceptor` creates activity sources that must be registered with OpenTelemetry
- Connection string is resolved from `ConnectionStrings:<resource-name>` configuration

### Endpoints

**Dynamic Port Allocation (Default)**:
- Aspire automatically assigns available ports to avoid conflicts
- Recommended for development, testing, and CI/CD
- Example: server might be localhost:54321, UI localhost:54322

**Container Resource Endpoints**:
- **server**: gRPC endpoint (targetPort: 7233) - HTTP/2 service
- **ui**: Web UI (targetPort: 8233) - HTTP service
- **metrics**: Optional metrics endpoint (targetPort: 7235)
- **http**: Optional HTTP API endpoint (targetPort: 7234)

**Executable Resource Endpoints**:
- Similar structure to container
- No targetPort mapping (direct host ports)

**Fixed Ports (Optional)**:
```csharp
var temporal = builder.AddTemporalServer("temporal")
    .WithServiceEndpoint(7233)
    .WithUiEndpoint(8233);
```

## Package Management

This is a packable library project (IsPackable=true) with package validation enabled. When modifying:
- Update `PackageVersion` in src/InfinityFlow.Aspire.Temporal/InfinityFlow.Aspire.Temporal.csproj:10
- Include package assets from assets/ folder (packageIcon.png, README.md, LICENSE)
- Documentation XML is generated automatically
- SourceLink is configured for GitHub

## Important Notes

### Port Allocation
- **Default behavior**: Ports are `null` which triggers Aspire dynamic allocation
- **Fixed ports**: Use `WithServiceEndpoint(port)`, `WithUiEndpoint(port)` for specific ports
- Container uses `0.0.0.0` as default IP binding (configurable via `WithIp()`)

### Namespaces
- The "default" namespace is always created by Temporal
- Additional namespaces configured via `WithNamespace("ns1", "ns2")`

### Dynamic Configuration
- Supports types: string, bool, int, float, double, long
- Throws `ArgumentException` for unsupported types (prevents silent failures)
- Example: `.WithDynamicConfigValue("frontend.enableUpdateWorkflowExecution", true)`

### Error Handling
- All user callbacks wrapped in try-catch with contextual error messages
- Missing connection strings provide list of available connections
- Null/whitespace tracing sources throw `InvalidOperationException`
- Resource configuration after build throws `InvalidOperationException`

### Memory Management
- `Meter` instances registered as singleton for proper disposal
- `TemporalRuntime` created with telemetry configuration
- All IDisposable resources managed by DI container

### Breaking Changes (v1.0.0)
- Default port allocation changed from fixed (7233, 8233) to dynamic (null)
- `AddTemporalServerContainer()` replaced by `AddTemporalServer()` (legacy still works)
- Client API returns `TemporalClientBuilder` instead of `IServiceCollection`
- Semantic endpoint methods: `WithServiceEndpoint()` instead of `WithPort()`
