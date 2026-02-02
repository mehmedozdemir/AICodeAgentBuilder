using AI.CodeAgent.Builder.Application.Common.Interfaces.AI;
using AI.CodeAgent.Builder.Application.Common.Interfaces.Persistence;
using AI.CodeAgent.Builder.Application.Common.Interfaces.Templates;
using AI.CodeAgent.Builder.Domain.Common;
using AI.CodeAgent.Builder.Infrastructure.AI;
using AI.CodeAgent.Builder.Infrastructure.Configuration;
using AI.CodeAgent.Builder.Infrastructure.Persistence;
using AI.CodeAgent.Builder.Infrastructure.Persistence.Repositories;
using AI.CodeAgent.Builder.Infrastructure.Templates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AI.CodeAgent.Builder.Infrastructure;

/// <summary>
/// Infrastructure layer dependency injection configuration.
/// Registers all persistence, AI, and template services.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuration
        services.Configure<DatabaseSettings>(configuration.GetSection(nameof(DatabaseSettings)));
        services.Configure<AIProviderSettings>(configuration.GetSection(nameof(AIProviderSettings)));
        services.Configure<TemplateSettings>(configuration.GetSection(nameof(TemplateSettings)));

        // Persistence
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var databaseSettings = configuration
                .GetSection(nameof(DatabaseSettings))
                .Get<DatabaseSettings>() ?? new DatabaseSettings();

            options.UseSqlite(
                databaseSettings.ConnectionString,
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

            if (databaseSettings.EnableSensitiveDataLogging)
            {
                options.EnableSensitiveDataLogging();
            }

            if (databaseSettings.EnableDetailedErrors)
            {
                options.EnableDetailedErrors();
            }
        });

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITechStackRepository, TechStackRepository>();
        services.AddScoped<IArchitecturePatternRepository, ArchitecturePatternRepository>();
        services.AddScoped<IEngineeringRuleRepository, EngineeringRuleRepository>();
        services.AddScoped<IProjectProfileRepository, ProjectProfileRepository>();
        services.AddScoped<IAIResponseRepository, AIResponseRepository>();

        // AI Providers
        services.AddHttpClient<OpenAIProvider>();
        services.AddHttpClient<AzureOpenAIProvider>();
        services.AddSingleton<IAIProviderFactory, AIProviderFactory>();

        // Default AI provider (based on configuration)
        services.AddScoped<IAIProvider>(sp =>
        {
            var factory = sp.GetRequiredService<IAIProviderFactory>();
            return factory.CreateProvider();
        });

        // Template Service
        services.AddSingleton<ITemplateService, TemplateService>();

        return services;
    }

    /// <summary>
    /// Ensures the database is created and applies pending migrations.
    /// Call this during application startup.
    /// </summary>
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Apply migrations automatically
        await context.Database.MigrateAsync();
    }
}

