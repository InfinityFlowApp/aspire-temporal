using DotNet.Testcontainers.Builders;

namespace Aspire.Temporal.Server;

public static class TemporalServerContainerBuilderExtensions
{
    /// <summary>
    /// Adds a temporal server resource instance to the Aspire host. Requires the Temporal Container location to be in your path.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static Task<IResourceBuilder<TemporalServerContainerResource>> AddTemporalServerContainer(this IDistributedApplicationBuilder builder, string name,
        Action<TemporalServerResourceBuilder> callback)
    {
        var rb = new TemporalServerResourceBuilder();
        callback(rb);
        var args = rb.Build();

        return builder.AddTemporalServerContainer(name, args);
    }

    /// <summary>
    /// Adds a temporal server resource instance to the Aspire host. Requires the Temporal Container location to be in your path.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public  static Task<IResourceBuilder<TemporalServerContainerResource>> AddTemporalServerContainer(this IDistributedApplicationBuilder builder, string name)
    {
        return builder.AddTemporalServerContainer(name, new TemporalServerResourceArguments());
    }

    const string Dockerfile = """
    # Define the build stage
    FROM alpine as builder

    # Install curl and tar
    RUN apk add --no-cache curl tar

    # Set build arguments for URL, defaulting ARCH to amd64
    ARG ARCH=amd64
    ARG URL="https://temporal.download/cli/archive/latest?platform=linux"

    # Download the binary based on the architecture
    RUN curl -L "${URL}&arch=${ARCH}" -o temporal.tar.gz

    # Extract the archive
    RUN tar -xzf temporal.tar.gz -C /tmp
    RUN mv /tmp/temporal /temporal

    # Ensure the binary is executable
    RUN chmod +x /temporal

    # Define the final stage based on a scratch image for minimal size
    FROM alpine

    # Copy the binary from the builder stage
    COPY --from=builder /temporal /temporal

    EXPOSE 7233
    EXPOSE 8233
    EXPOSE 9000
    EXPOSE 8080

    # Set the entrypoint to the downloaded binary
    ENTRYPOINT ["/temporal"]
    """;

    private static async Task<IResourceBuilder<TemporalServerContainerResource>> AddTemporalServerContainer(this IDistributedApplicationBuilder builder, string name,
        TemporalServerResourceArguments args)
    {
        await BuildAspireHelperContainer();

        var container = new TemporalServerContainerResource(name, args);

        var resourceBuilder = builder.AddResource(container)
                .WithAnnotation(new ContainerImageAnnotation() { Image = "aspire-temporal-server-helper", Tag = "latest" })
                .WithArgs(args.GetArgs())
                .WithHttpEndpoint(name: "server", hostPort: args.Port, containerPort: 7233).AsHttp2Service(); // Internal port is always 7233

        if (args.Headless is not true)
        {
            resourceBuilder.WithHttpEndpoint(name: "ui", hostPort: args.UiPort, containerPort: 8233); // Internal port is always 8233
        }

        if (args.MetricsPort is not null)
        {
            resourceBuilder.WithHttpEndpoint(name: "metrics", hostPort: args.MetricsPort, containerPort: 9000); // Internal port is always 9000
        }

        if (args.HttpPort is not null)
        {
            resourceBuilder.WithHttpEndpoint(name: "http", hostPort: args.HttpPort, containerPort: 8080); // Internal port is always 8080
        }

        return resourceBuilder;
    }

    private static async Task BuildAspireHelperContainer()
    {
        string tempDir = string.Empty;
        try
        {
            tempDir = Directory.CreateTempSubdirectory("aspire-temporal-server").FullName;
            var dockerFileTemp = Path.Join(tempDir, "Dockerfile");

            await File.WriteAllTextAsync(dockerFileTemp, Dockerfile);

            await using var futureImage = new ImageFromDockerfileBuilder()
                 .WithDockerfileDirectory(tempDir)
                 .WithDeleteIfExists(false)
                 .WithBuildArgument("ARCH", System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture switch
                 {
                     System.Runtime.InteropServices.Architecture.X64 => "amd64",
                     System.Runtime.InteropServices.Architecture.Arm64 => "arm64",
                     _ => throw new NotImplementedException($"Unsupported CPU Architecture: {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}")
                 })
                 .WithBuildArgument("URL", "https://temporal.download/cli/archive/latest?platform=linux")
                 .WithName("aspire-temporal-server-helper:latest")
                 .WithDockerfile("Dockerfile")
                 .Build();

            await futureImage.CreateAsync();
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }
}