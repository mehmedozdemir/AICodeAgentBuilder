using System;
using AI.CodeAgent.Builder.UI.ViewModels;

namespace AI.CodeAgent.Builder.UI.Services;

/// <summary>
/// Centralized navigation service for decoupled view navigation.
/// Manages navigation history and supports forward/backward navigation for wizards.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Gets the current view model being displayed.
    /// </summary>
    ViewModelBase? CurrentViewModel { get; }

    /// <summary>
    /// Occurs when the current view model changes.
    /// </summary>
    event EventHandler<ViewModelBase?>? CurrentViewModelChanged;

    /// <summary>
    /// Navigates to the specified view model.
    /// </summary>
    void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;

    /// <summary>
    /// Navigates to a specific view model instance.
    /// Used when passing data between views.
    /// </summary>
    void NavigateTo(ViewModelBase viewModel);

    /// <summary>
    /// Navigates back to the previous view model.
    /// </summary>
    /// <returns>True if navigation was successful, false if at root.</returns>
    bool GoBack();

    /// <summary>
    /// Clears navigation history and navigates to specified view model.
    /// </summary>
    void NavigateToRoot<TViewModel>() where TViewModel : ViewModelBase;

    /// <summary>
    /// Checks if backward navigation is possible.
    /// </summary>
    bool CanGoBack { get; }
}
