using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AI.CodeAgent.Builder.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AI.CodeAgent.Builder.UI.Services.Navigation;

/// <summary>
/// Navigation service implementation for Avalonia UI.
/// Manages navigation between views in the application.
/// </summary>
public sealed class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Stack<Type> _navigationStack = new();

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public bool CanGoBack => _navigationStack.Count > 1;

    public async Task NavigateToAsync<TViewModel>() where TViewModel : class
    {
        await NavigateToAsync<TViewModel>(null);
    }

    public async Task NavigateToAsync<TViewModel>(object? parameter) where TViewModel : class
    {
        var viewModelType = typeof(TViewModel);
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();

        if (viewModel is BaseViewModel baseViewModel)
        {
            await baseViewModel.InitializeAsync();
        }

        _navigationStack.Push(viewModelType);

        // Signal that navigation occurred
        // In a full implementation, this would update the main window's content
        OnNavigated(viewModel);
    }

    public async Task GoBackAsync()
    {
        if (!CanGoBack)
            return;

        // Remove current view
        _navigationStack.Pop();

        // Get previous view
        var previousViewModelType = _navigationStack.Peek();
        var viewModel = _serviceProvider.GetRequiredService(previousViewModelType);

        if (viewModel is BaseViewModel baseViewModel)
        {
            await baseViewModel.InitializeAsync();
        }

        OnNavigated(viewModel);
    }

    private void OnNavigated(object viewModel)
    {
        // This would typically raise an event or update a property
        // that the main window is subscribed to, allowing it to update its content
        Navigated?.Invoke(this, viewModel);
    }

    public event EventHandler<object>? Navigated;
}
