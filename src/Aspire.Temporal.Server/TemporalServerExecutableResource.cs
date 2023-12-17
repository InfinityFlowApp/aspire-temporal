namespace Aspire.Temporal.Server;

public class TemporalServerExecutableResource : ExecutableResource
{
    public TemporalServerExecutableResource(string name, TemporalServerExecutableResourceArguments arguments)
        : base(name, command: "temporal", workingDirectory: "", args: arguments.GetArgs())
    {
    }
}