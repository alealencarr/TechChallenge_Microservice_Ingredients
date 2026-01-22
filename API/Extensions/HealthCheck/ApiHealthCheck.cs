using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics.CodeAnalysis;

namespace API.Extensions.HealthCheck;

[ExcludeFromCodeCoverage]

public class ApiHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        // Se este código for executado, significa que a API está "viva"
        // e respondendo. Você pode adicionar lógicas mais complexas aqui no futuro,
        // como verificar alguma configuração essencial ou um serviço interno.
        return Task.FromResult(HealthCheckResult.Healthy("A API está online e funcionando."));
    }
}