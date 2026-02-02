using System.Threading.Tasks;

namespace AI.CodeAgent.Builder.UI.ViewModels;

/// <summary>
/// ViewModel for the main application window.
/// This is the composition root for the UI layer.
/// </summary>
public sealed class MainWindowViewModel : BaseViewModel
{
    private string _greeting = "AI Code Agent Builder";

    public string Greeting
    {
        get => _greeting;
        set => SetProperty(ref _greeting, value);
    }

    public override Task InitializeAsync()
    {
        // Future: Initialize main window state, load user preferences, etc.
        return Task.CompletedTask;
    }
}
