using Microsoft.Extensions.Diagnostics.HealthChecks;
using Temporalio.Client;

namespace InfinityFlow.Aspire.Temporal.Client;

internal sealed class TemporalHealthCheck(ITemporalClient client) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await client.Connection.CheckHealthAsync(options: new RpcOptions { CancellationToken = cancellationToken });
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Temporal server is not reachable.", ex);
        }
    }
}
