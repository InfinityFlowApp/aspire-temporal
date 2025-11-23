# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

InfinityFlow.Aspire.Temporal is a .NET Aspire extension that enables running the Temporal CLI dev server as either a container or executable resource. The library provides integration between .NET Aspire and Temporal workflow orchestration.

**Note:** Container deployment is the recommended and well-supported approach. Executable deployment has known limitations (see https://github.com/dotnet/aspire/issues/1637 and https://github.com/temporalio/cli/issues/316).

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

### Core Components

The library consists of two main integration patterns:

1. **Container Resource** (`TemporalServerContainerResource`) - Uses the `temporalio/admin-tools:latest` Docker image
2. **Executable Resource** (`TemporalServerExecutableResource`) - Runs the Temporal CLI binary directly

Both resources share the same configuration model through `TemporalServerResourceArguments`.

### Key Classes

- **TemporalServerResourceBuilder** (src/InfinityFlow.Aspire.Temporal/TemporalServerResourceBuilder.cs:5)
  - Fluent builder for configuring Temporal server options
  - Provides methods for all CLI flags (ports, namespaces, logging, UI settings, etc.)
  - Returns `TemporalServerResourceArguments` via `Build()`

- **TemporalServerResourceArguments** (src/InfinityFlow.Aspire.Temporal/TemporalServerResourceArguments.cs:6)
  - Holds all configuration properties (ports, log settings, namespaces, dynamic config)
  - `GetArgs()` method converts properties to CLI arguments for `temporal server start-dev`
  - Supports dynamic config values via `DynamicConfigValues` dictionary

- **TemporalServerContainerBuilderExtensions** (src/InfinityFlow.Aspire.Temporal/TemporalServerContainerBuilderExtensions.cs:6)
  - `AddTemporalServerContainer()` extension methods for `IDistributedApplicationBuilder`
  - Configures container with proper endpoints (server:7233 gRPC, ui:8233 HTTP, optional metrics/http)
  - Server endpoint is marked as HTTP/2 service via `.AsHttp2Service()`

- **TemporalServerExecutableBuilderExtensions** (src/InfinityFlow.Aspire.Temporal/TemporalServerExecutableBuilderExtensions.cs:6)
  - `AddTemporalServerExecutable()` extension methods
  - Configures endpoints with defaults (UI defaults to port+1000, metrics to 9000)

- **Resource Classes** (src/InfinityFlow.Aspire.Temporal/TemporalServerExecutableResource.cs)
  - Both inherit from Aspire base classes (`ExecutableResource`, `ContainerResource`)
  - Implement `IResourceWithConnectionString` - connection string is the server endpoint host:port
  - Container resource also implements `IResourceWithEnvironment`

### Configuration Pattern

The library uses a fluent builder pattern:
```csharp
builder.AddTemporalServerContainer("temporal", x => x
    .WithLogFormat(LogFormat.Json)
    .WithLogLevel(LogLevel.Info)
    .WithNamespace("ns1", "ns2")
    .WithDynamicConfigValue("key", value));
```

All `temporal server start-dev` CLI flags map to builder methods (src/InfinityFlow.Aspire.Temporal/TemporalServerResourceBuilder.cs).

### Enums

Three enums define Temporal CLI options (src/InfinityFlow.Aspire.Temporal/Enums.cs):
- `LogFormat`: Json, Pretty
- `LogLevel`: Debug, Info, Warn, Error, Fatal
- `SQLitePragma`: JournalMode, Synchronous

`EnumHelpers` class converts enums to CLI string values.

## Project Structure

- **src/InfinityFlow.Aspire.Temporal/** - Main library code
  - Target framework: net9.0
  - NuGet package ID: InfinityFlow.Aspire.Temporal
  - Dependencies: Aspire.Hosting.AppHost 9.5.0

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

The sample demonstrates Temporal observability integration (sample/Worker/Program.cs, sample/ServiceDefaults/Extensions.cs):
- Uses `Temporalio.Extensions.DiagnosticSource` for tracing (`TracingInterceptor`)
- Uses `Temporalio.Extensions.OpenTelemetry` for metrics (`CustomMetricMeter`)
- Custom meter and tracing sources must be added to Aspire service defaults

### Endpoints

Container resource endpoints:
- **server**: gRPC endpoint (default 7233) - HTTP/2 service
- **ui**: Web UI (default 8233) - HTTP service
- **metrics**: Optional metrics endpoint
- **http**: Optional HTTP API endpoint

Executable resource endpoints have similar structure but with different default handling.

## Package Management

This is a packable library project (IsPackable=true) with package validation enabled. When modifying:
- Update `PackageVersion` in src/InfinityFlow.Aspire.Temporal/InfinityFlow.Aspire.Temporal.csproj:10
- Include package assets from assets/ folder (packageIcon.png, README.md, LICENSE)
- Documentation XML is generated automatically
- SourceLink is configured for GitHub

## Important Notes

- Container resource uses `0.0.0.0` as default IP binding (src/InfinityFlow.Aspire.Temporal/TemporalServerResourceArguments.cs:46)
- Port defaults: server=7233, UI=8233
- The "default" namespace is always created by Temporal
- Dynamic config values support string, bool, int, float, double, long types (src/InfinityFlow.Aspire.Temporal/TemporalServerResourceArguments.cs:140-149)
