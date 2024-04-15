using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GS_Shop_UserManagement.Infrastructure.CustomHealthCheck;

public class ApiHealthCheck : IHealthCheck
{

    private readonly HttpClient _httpClient;

    public ApiHealthCheck(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public  Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.FromResult(HealthCheckResult.Healthy("Health check passed"));

    }
}