using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using AI.CodeAgent.Builder.UI.Services.Navigation;
using AI.CodeAgent.Builder.UI.ViewModels;
using AI.CodeAgent.Builder.UI.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ApplicationDI = AI.CodeAgent.Builder.Application.DependencyInjection;
using InfrastructureDI = AI.CodeAgent.Builder.Infrastructure.DependencyInjection;
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

            var mainWindowViewModel = ServiceProvider.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
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
            InfrastructureDI.AddInfrastructure(services, context.Configuration);

            // Register Navigation Service
            services.AddSingleton<INavigationService, NavigationService>();

            // Register ViewModels
            services.AddTransient<MainWindowViewModel>();
        });

        _host = builder.Build();
        ServiceProvider = _host.Services;
    }

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        _host?.Dispose();
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