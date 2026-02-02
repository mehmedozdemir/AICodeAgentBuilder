using System.Threading.Tasks;

namespace AI.CodeAgent.Builder.UI.Services;

/// <summary>
/// Service for displaying dialogs and user confirmations.
/// Decouples ViewModels from UI-specific dialog implementations.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Shows an information message.
    /// </summary>
    Task ShowInformationAsync(string title, string message);

    /// <summary>
    /// Shows an error message.
    /// </summary>
    Task ShowErrorAsync(string title, string message);

    /// <summary>
    /// Shows a warning message.
    /// </summary>
    Task ShowWarningAsync(string title, string message);

    /// <summary>
    /// Shows a confirmation dialog with Yes/No buttons.
    /// </summary>
    /// <returns>True if user confirmed, false otherwise.</returns>
    Task<bool> ShowConfirmationAsync(string title, string message);

    /// <summary>
    /// Shows an input dialog for simple text input.
    /// </summary>
    /// <returns>User input, or null if canceled.</returns>
    Task<string?> ShowInputDialogAsync(string title, string prompt, string? defaultValue = null);
}
