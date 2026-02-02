using System;
using System.Collections.Generic;
using AI.CodeAgent.Builder.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AI.CodeAgent.Builder.UI.Services;

/// <summary>
/// Implementation of navigation service using a stack-based history.
/// Thread-safe and supports wizard-style navigation.
/// </summary>
public sealed class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Stack<ViewModelBase> _navigationHistory = new();
    private ViewModelBase? _currentViewModel;
    private readonly object _lock = new();

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public ViewModelBase? CurrentViewModel
    {
        get
        {
            lock (_lock)
            {
                return _currentViewModel;
            }
        }
        private set
        {
            lock (_lock)
            {
                if (_currentViewModel == value)
                    return;

                _currentViewModel = value;
            }

            CurrentViewModelChanged?.Invoke(this, value);
        }
    }

    public bool CanGoBack
    {
        get
        {
            lock (_lock)
            {
                return _navigationHistory.Count > 0;
            }
        }
    }

    public event EventHandler<ViewModelBase?>? CurrentViewModelChanged;

    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        var viewModel = _serviceProvider.GetService(typeof(TViewModel)) as TViewModel
            ?? throw new InvalidOperationException($"ViewModel {typeof(TViewModel).Name} is not registered in DI container.");

        NavigateTo(viewModel);
    }

    public void NavigateTo(ViewModelBase viewModel)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        lock (_lock)
        {
            // Push current view model to history before navigating
            if (_currentViewModel != null)
            {
                _navigationHistory.Push(_currentViewModel);
            }
        }

        CurrentViewModel = viewModel;
    }

    public bool GoBack()
    {
        lock (_lock)
        {
            if (_navigationHistory.Count == 0)
                return false;

            var previousViewModel = _navigationHistory.Pop();
            CurrentViewModel = previousViewModel;
            return true;
        }
    }

    public void NavigateToRoot<TViewModel>() where TViewModel : ViewModelBase
    {
        lock (_lock)
        {
            _navigationHistory.Clear();
        }

        NavigateTo<TViewModel>();
    }
}
