# InfinityFlow.Aspire.Temporal

[![NuGet](https://img.shields.io/nuget/v/InfinityFlow.Aspire.Temporal.svg?style=flat)](https://www.nuget.org/packages/InfinityFlow.Aspire.Temporal)
 
[![Discord](https://discordapp.com/api/guilds/1148334798524383292/widget.png?style=banner2)](https://discord.gg/PXJFbP7PKk)

Aspire extension to start the temporal cli dev server as an container or executable resource. 
**Note: Only container works as expected. See https://github.com/dotnet/aspire/issues/1637 and https://github.com/temporalio/cli/issues/316**


## Contents:
- [Pre-Requisites](#pre-requisites)
- [Getting Started](#getting-started)
- [Observability](#observability)
- [Configuration](#configuration)

## Pre-requisites

- [Temporal CLI](https://github.com/temporalio/cli) (ensure the binary is in your PATH)
- An Aspire project. See [Aspire docs](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview) to get started.

## Getting Started

### 1. Install the nuget package

```sh
dotnet add package InfinityFlow.Aspire.Temporal
```

### 2. Add Temporal dev server to your Aspire AppHost Program.cs

```csharp
// AppHost/Program.cs
using Aspire.Temporal.Server;

var builder = DistributedApplication.CreateBuilder(args);

// Use the default server options
var temporal = await builder.AddTemporalServerContainer("temporal")

// OR customise server options with builder
//      see config section for details
var temporal = await builder.AddTemporalServerContainer("temporal", x => x
    .WithLogFormat(LogFormat.Json)
    .WithLogLevel(LogLevel.Info)
    .WithNamespace("test1", "test2"));
```

### 3. Run the Aspire application

You should see Temporal running under the Executables tab.

**Dynamic Port Allocation (Default)**:
By default, Aspire automatically assigns random available ports to avoid conflicts.
This is especially useful for:
- Running multiple Temporal instances simultaneously
- Parallel test execution
- CI/CD environments where port conflicts are common

You can view the allocated ports in the Aspire dashboard. For example:
- Server: http://localhost:54321 (dynamically allocated)
- UI: http://localhost:54322 (dynamically allocated)

**Fixed Ports (Optional)**:
If you need consistent port numbers, explicitly specify them:
```csharp
var temporal = builder.AddTemporalServerContainer("temporal", x => x
    .WithPort(7233)      // Server on http://localhost:7233
    .WithUiPort(8233));  // UI on http://localhost:8233
```

![Aspire dashboard temporal exe](./docs/aspire-dashboard-exe.png)

### 4. Configure Client/Worker Applications

#### Install the Client Library

```sh
dotnet add package InfinityFlow.Aspire.Temporal.Client
```

#### Reference Temporal in AppHost

Include Temporal in your Aspire orchestration and reference it from your applications:

```csharp
// ./samples/AppHost/Program.cs

var temporal = builder.AddTemporalServerContainer("temporal");

builder.AddProject<Projects.Worker>("worker")
    .WithReference(temporal);  // Injects connection string automatically

builder.AddProject<Projects.Api>("api")
    .WithReference(temporal);
```

#### Configure Temporal Client

The client library provides a fluent API with automatic OpenTelemetry configuration:

```csharp
// Register a client - ./samples/Api/Program.cs
builder.AddTemporalClient()
    .ConfigureOptions(opts =>
    {
        opts.Namespace = "default";
    })
    .WithInterceptors(interceptors =>
    {
        // Add custom interceptors if needed
        interceptors.Add(new MyCustomInterceptor());
    })
    .WithTracingSources(sources =>
    {
        // Customize tracing sources (default: Temporalio.Client, Temporalio.Workflows, Temporalio.Activities)
        sources.Add("MyCustomSource");
    });

// Or register a worker - ./samples/Worker/Program.cs
builder.AddTemporalWorker("my-task-queue")
    .ConfigureOptions(opts =>
    {
        opts.Namespace = "default";
    })
    .Services
    .AddScopedActivities<MyActivities>()
    .AddWorkflow<MyWorkflow>();
```

**Key Features**:
- **Automatic connection resolution**: Connection string is automatically retrieved from Aspire configuration
- **OpenTelemetry integration**: Tracing and metrics configured automatically for Aspire dashboard
- **Fluent API**: Chain configuration methods for clean, readable code
- **Customizable**: Disable tracing/metrics, add custom interceptors, configure tracing sources

**Advanced Configuration**:

```csharp
builder.AddTemporalClient(connectionName: "temporal")
    .WithoutTracing()           // Disable automatic tracing
    .WithoutMetrics()           // Disable automatic metrics
    .ConfigureOptions(opts =>   // Direct access to TemporalClientConnectOptions
    {
        opts.Namespace = "production";
        opts.Identity = "my-service";
    });
```

## Observability

The `InfinityFlow.Aspire.Temporal.Client` package provides **automatic OpenTelemetry configuration** for seamless integration with the Aspire dashboard:

- **Automatic Tracing**: Configures `TracingInterceptor` to capture workflow and activity traces
- **Automatic Metrics**: Configures `CustomMetricMeter` to export Temporal metrics
- **Activity Sources**: Registers `Temporalio.Client`, `Temporalio.Workflows`, and `Temporalio.Activities` sources
- **Aspire Dashboard Integration**: All telemetry flows to the Aspire dashboard automatically

**Basic Setup** (automatic observability):
```csharp
// Client automatically configured with tracing and metrics
builder.AddTemporalClient();
```

**Service Defaults Configuration** (optional, for centralized setup):

If you want to configure OpenTelemetry in your ServiceDefaults project, you can use:

```csharp
// ServiceDefaults/Extensions.cs
builder.Logging.AddTemporalObservability();

// Or configure individually:
builder.Services.ConfigureOpenTelemetryTracerProvider(tracing =>
    tracing.AddTemporalTracing());

builder.Services.ConfigureOpenTelemetryMeterProvider(metrics =>
    metrics.AddTemporalMetrics());
```

See the sample folder for complete examples:
- [sample/Api/Program.cs](./sample/Api/Program.cs) for an example client
- [sample/Worker/Program.cs](./sample/Worker/Program.cs) for an example worker
- [sample/ServiceDefaults/Extensions.cs](./sample/ServiceDefaults/Extensions.cs) for service defaults configuration

Once configured, you'll see tracing and metrics in the Aspire dashboard:

#### Tracing

![aspire dashboard temporal tracing](./docs/aspire-dashboard-temporal-tracing.png)

#### Metrics

![aspire dashboard temporal metrics](./docs/aspire-dashboard-temporal-metrics.png)


## Configuration

The dev server can be configured with a fluent builder

```csharp
var temporal = builder.AddTemporalServerContainer("temporal", builder => builder
    .WithPort(7233)          // Optional: specify fixed port (default: dynamic allocation)
    .WithLogFormat(LogFormat.Json))
```

You can run `temporal server start-dev --help` to get more information about the CLI flags on the dev server. All available flags are mapped to a method on the builder.

Available methods:

```csharp
builder
    .WithDbFileName("/location/of/persistent/file") // --db-filename
    .WithNamespace("namespace-name", ...)           // --namespace
    .WithPort(7233)                                 // --port (optional: null = dynamic allocation)
    .WithHttpPort(7234)                             // --http-port (optional: null = disabled)
    .WithMetricsPort(7235)                          // --metrics-port (optional: null = disabled)
    .WithUiPort(8233)                               // --ui-port (optional: null = dynamic allocation)
    .WithHeadlessUi(true)                           // --headless (disables UI)
    .WithIp("127.0.0.1")                            // --ip
    .WithUiIp("127.0.0.1")                          // --ui-ip
    .WithUiAssetPath("/location/of/custom/assets")  // --ui-asset-path
    .WithUiCodecEndpoint("http://localhost:8080")   // --ui-codec-endpoint
    .WithLogFormat(LogFormat.Pretty)                // --log-format
    .WithLogLevel(LogLevel.Info)                    // --log-level
    .WithSQLitePragma(SQLitePragma.JournalMode)     // --sqlite-pragma
```

### Port Allocation Behavior

**Dynamic Allocation (Recommended)**:
- Don't specify `.WithPort()` or `.WithUiPort()` - Aspire will allocate random available ports
- Best for development, testing, and CI/CD environments
- Eliminates port conflicts when running multiple instances

**Fixed Ports**:
- Explicitly call `.WithPort(7233)` to use a specific port
- Use when external tools or scripts depend on consistent port numbers
- May cause conflicts if multiple instances run simultaneously

## Breaking Changes in v1.0

**Port allocation behavior has changed**:

**v0.x (old)**:
```csharp
var temporal = builder.AddTemporalServerContainer("temporal");
// Resulted in fixed ports: localhost:7233, localhost:8233
```

**v1.0 (new)**:
```csharp
var temporal = builder.AddTemporalServerContainer("temporal");
// Results in dynamic ports: localhost:54321, localhost:54322 (random)
```

**Migration**:
To maintain the old behavior with fixed ports, explicitly specify them:
```csharp
var temporal = builder.AddTemporalServerContainer("temporal", x => x
    .WithPort(7233)      // Server on fixed port 7233
    .WithUiPort(8233));  // UI on fixed port 8233
```

This change follows standard [Aspire networking patterns](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/networking-overview) and eliminates port conflicts in multi-instance scenarios.
