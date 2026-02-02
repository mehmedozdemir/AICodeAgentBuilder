using AI.CodeAgent.Builder.Application.Services.AIGeneration;
using AI.CodeAgent.Builder.Application.Services.ArtifactGeneration;
using AI.CodeAgent.Builder.Application.Services.Categories;
using AI.CodeAgent.Builder.Application.Services.ProjectProfiles;
using AI.CodeAgent.Builder.Application.Services.TechStacks;
using Microsoft.Extensions.DependencyInjection;

namespace AI.CodeAgent.Builder.Application;

/// <summary>
/// Dependency injection configuration for the Application layer.
/// Registers all application services following Clean Architecture principles.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers Application layer services.
    /// Application layer depends ONLY on Domain layer.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register application services
        services.AddScoped<CategoryService>();
        services.AddScoped<TechStackService>();
        services.AddScoped<ProjectProfileService>();
        services.AddScoped<AIGenerationService>();
        services.AddScoped<ArtifactGenerationService>();

        // Future enhancements:
        // - Add MediatR for CQRS pattern
        // - Add FluentValidation for request validation
        // - Add AutoMapper for object mapping
        // - Add caching services
        // - Add background job scheduling

        return services;
    }
}
