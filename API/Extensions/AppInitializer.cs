using Infrastructure.DbContexts;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace API.Extensions;
[ExcludeFromCodeCoverage]

internal static class AppInitializer
{
    internal static async Task<IApplicationBuilder> InitializeApp(this WebApplication app, Serilog.ILogger logger)
    {
        await app.InitializeDatabase(logger);
        return app;
    }
    private static async Task<IApplicationBuilder> InitializeDatabase(this IApplicationBuilder app, Serilog.ILogger _logger)
    {

        _logger.Information("Iniciando inicialização do banco de dados...");
        using var serviceScope = app.ApplicationServices.CreateScope();
        var initalizer = serviceScope.ServiceProvider.GetRequiredService<DataSeeder>();
        var appDbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

        await appDbContext.Database.MigrateAsync();
        await initalizer.Initialize();
        _logger.Information("Banco de dados inicializado com sucesso.");
        return await Task.FromResult(app);
    }
}