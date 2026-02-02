using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using AI.CodeAgent.Builder.UI.ViewModels;
using AI.CodeAgent.Builder.UI.Views;
using AI.CodeAgent.Builder.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using ApplicationDI = AI.CodeAgent.Builder.Application.DependencyInjection;
using AvaloniaApplication = Avalonia.Application;

namespace AI.CodeAgent.Builder.UI;

public partial class App : AvaloniaApplication
{
    private IHost? _host;

    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        ConfigureServices();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            // Initialize database and seed data
            InitializeDatabaseAsync().Wait();

            var mainViewModel = ServiceProvider.GetRequiredService<MainViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            desktop.Exit += OnExit;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices()
    {
        var builder = Host.CreateDefaultBuilder();

        builder.ConfigureServices((context, services) =>
        {
            // Add Application and Infrastructure layers
            ApplicationDI.AddApplication(services);
            AI.CodeAgent.Builder.Infrastructure.DependencyInjection.AddInfrastructure(services, context.Configuration);

            // Register UI Services
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogService, DialogService>();

            // Register UI facade services that wrap Application layer
            services.AddScoped<UI.Services.ICategoryService>(sp =>
            {
                var appService = sp.GetRequiredService<Application.Services.Categories.CategoryService>();
                return new CategoryServiceFacade(appService);
            });
            
            services.AddScoped<UI.Services.ITechStackService>(sp =>
            {
                var appService = sp.GetRequiredService<Application.Services.TechStacks.TechStackService>();
                return new TechStackServiceFacade(appService);
            });
            
            services.AddScoped<UI.Services.IAIGenerationService>(sp =>
            {
                var appService = sp.GetRequiredService<Application.Services.AIGeneration.AIGenerationService>();
                return new AIGenerationServiceFacade(appService);
            });
            
            services.AddScoped<UI.Services.IArchitectureService>(sp =>
            {
                var repo = sp.GetRequiredService<Application.Common.Interfaces.Persistence.IArchitecturePatternRepository>();
                return new ArchitectureServiceFacade(repo);
            });
            
            services.AddScoped<UI.Services.IEngineeringRuleService>(sp =>
            {
                var repo = sp.GetRequiredService<Application.Common.Interfaces.Persistence.IEngineeringRuleRepository>();
                return new EngineeringRuleServiceFacade(repo);
            });
            
            services.AddScoped<UI.Services.IArtifactGenerationService>(sp =>
            {
                var appService = sp.GetRequiredService<Application.Services.ArtifactGeneration.ArtifactGenerationService>();
                return new ArtifactGenerationServiceFacade(appService);
            });

            // Register Main Shell ViewModel
            services.AddSingleton<MainViewModel>();

            // Register Screen ViewModels (Transient - new instance per navigation)
            services.AddTransient<HomeViewModel>();
            services.AddTransient<CategoryManagementViewModel>();
            services.AddTransient<HistoryViewModel>();

            // Register Project Builder Wizard ViewModels
            // These are singleton within the wizard session, resolved by ProjectBuilderViewModel
            services.AddTransient<ProjectBuilderViewModel>();
            services.AddTransient<ProjectMetadataViewModel>();
            services.AddTransient<StackSelectionViewModel>();
            services.AddTransient<StackParameterViewModel>();
            services.AddTransient<ArchitectureRulesViewModel>();
            services.AddTransient<PreviewGenerateViewModel>();
        });

        _host = builder.Build();
        ServiceProvider = _host.Services;
    }

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        _host?.Dispose();
    }

    private async Task InitializeDatabaseAsync()
    {
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.CodeAgent.Builder.Infrastructure.Persistence.ApplicationDbContext>();
        
        // Apply migrations
        await dbContext.Database.MigrateAsync();
        
        // Seed initial data
        await AI.CodeAgent.Builder.Infrastructure.Persistence.DatabaseSeeder.SeedAsync(dbContext);
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}