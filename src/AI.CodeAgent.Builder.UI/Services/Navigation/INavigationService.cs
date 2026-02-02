using System;
using System.Threading.Tasks;

namespace AI.CodeAgent.Builder.UI.Services.Navigation;

/// <summary>
/// Interface for navigation service in MVVM architecture.
/// Enables ViewModels to navigate between views without direct coupling to the UI layer.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigates to the specified ViewModel type.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type to navigate to</typeparam>
    Task NavigateToAsync<TViewModel>() where TViewModel : class;

    /// <summary>
    /// Navigates to the specified ViewModel type with a parameter.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type to navigate to</typeparam>
    /// <param name="parameter">The parameter to pass to the ViewModel</param>
    Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : class;

    /// <summary>
    /// Navigates back to the previous view.
    /// </summary>
    Task GoBackAsync();

    /// <summary>
    /// Gets whether the navigation service can go back.
    /// </summary>
    bool CanGoBack { get; }
}
