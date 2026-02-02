using Microsoft.Extensions.DependencyInjection;

namespace AI.CodeAgent.Builder.Application;

/// <summary>
/// Dependency injection configuration for the Application layer.
/// Currently placeholder for future application services.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Future: Add MediatR, FluentValidation, AutoMapper, etc.
        // For now, this is a placeholder for application-level service registration

        return services;
    }
}
