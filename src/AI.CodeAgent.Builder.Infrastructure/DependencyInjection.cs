using AI.CodeAgent.Builder.Application.Common.Interfaces;
using AI.CodeAgent.Builder.Domain.Common;
using AI.CodeAgent.Builder.Infrastructure.Persistence;
using AI.CodeAgent.Builder.Infrastructure.Persistence.Repositories;
using AI.CodeAgent.Builder.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AI.CodeAgent.Builder.Infrastructure;

/// <summary>
/// Dependency injection configuration for the Infrastructure layer.
/// Registers all infrastructure services, repositories, and database context.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register SQLite database context
        var databasePath = GetDatabasePath(configuration);
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite($"Data Source={databasePath}"));

        // Register database context interface
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register generic repository
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Register services
        services.AddSingleton<ITemplateService, TemplateService>();

        return services;
    }

    private static string GetDatabasePath(IConfiguration configuration)
    {
        var databaseFileName = configuration["Database:FileName"] ?? "aiagentbuilder.db";
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataPath, "AI.CodeAgent.Builder");
        
        // Ensure directory exists
        Directory.CreateDirectory(appFolder);
        
        return Path.Combine(appFolder, databaseFileName);
    }
}
