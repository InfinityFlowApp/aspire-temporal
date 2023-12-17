namespace Aspire.Temporal.Server;

public static class TemporalServerBuilderExtensions
{
    /// <summary>
    /// Adds a temporal server resource instance to the Aspire host. Requires the Temporal executable location to be in your path.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static IResourceBuilder<TemporalServerExecutableResource> AddTemporalServerExecutable(this IDistributedApplicationBuilder builder, string name,
        Func<TemporalServerExecutableResourceBuilder, TemporalServerExecutableResourceArguments> callback)
    {
        return builder.AddResource(new TemporalServerExecutableResource(name, callback(new TemporalServerExecutableResourceBuilder())));
    }

    /// <summary>
    /// Adds a temporal server resource instance to the Aspire host. Requires the Temporal executable location to be in your path.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IResourceBuilder<TemporalServerExecutableResource> AddTemporalServerExecutable(this IDistributedApplicationBuilder builder, string name)
    {
        return builder.AddResource(new TemporalServerExecutableResource(name, new TemporalServerExecutableResourceArguments()));
    }
}