using Aspire.Temporal.Server;

var builder = DistributedApplication.CreateBuilder(args);

const string TemporalDbConfigName = "TemporalDbFileName";

if (builder.Configuration[TemporalDbConfigName] is not string temporalDb)
{
    throw new Exception($"Set the {TemporalDbConfigName} in your appsettings.Development.json");
}

var temporal = builder.AddTemporalServerExecutable("temporal", x => x
    .WithLogFormat(LogFormat.Pretty)
    .WithLogLevel(LogLevel.Info)
    .WithDbFileName(temporalDb)
    .WithNamespace("test1", "test2"));

builder.AddProject<Projects.Api>("api")
    .WithReference(temporal);

builder.AddProject<Projects.Worker>("worker")
    .WithReference(temporal);

builder.Build().Run();