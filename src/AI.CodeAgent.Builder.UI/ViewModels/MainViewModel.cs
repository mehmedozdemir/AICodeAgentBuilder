using System;
using AI.CodeAgent.Builder.UI.Commands;
using AI.CodeAgent.Builder.UI.Services;
using System.Windows.Input;

namespace AI.CodeAgent.Builder.UI.ViewModels;

/// <summary>
/// Main shell view model managing application layout and navigation.
/// Provides sidebar navigation and content region for child views.
/// </summary>
public sealed class MainViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private ViewModelBase? _currentView;

    public MainViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        // Subscribe to navigation changes
        _navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;

        // Navigation commands
        NavigateToHomeCommand = new RelayCommand(() => _navigationService.NavigateTo<HomeViewModel>());
        NavigateToCategoriesCommand = new RelayCommand(() => _navigationService.NavigateTo<CategoryManagementViewModel>());
        NavigateToProjectBuilderCommand = new RelayCommand(() => _navigationService.NavigateTo<ProjectBuilderViewModel>());
        NavigateToHistoryCommand = new RelayCommand(() => _navigationService.NavigateTo<HistoryViewModel>());

        // Navigate to home on startup
        _navigationService.NavigateToRoot<HomeViewModel>();
    }

    /// <summary>
    /// The currently displayed view model in the content region.
    /// </summary>
    public ViewModelBase? CurrentView
    {
        get => _currentView;
        private set => SetProperty(ref _currentView, value);
    }

    public ICommand NavigateToHomeCommand { get; }
    public ICommand NavigateToCategoriesCommand { get; }
    public ICommand NavigateToProjectBuilderCommand { get; }
    public ICommand NavigateToHistoryCommand { get; }

    private void OnCurrentViewModelChanged(object? sender, ViewModelBase? viewModel)
    {
        CurrentView = viewModel;
    }
}
