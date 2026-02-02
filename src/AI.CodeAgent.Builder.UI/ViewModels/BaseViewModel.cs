using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AI.CodeAgent.Builder.UI.ViewModels;

/// <summary>
/// Base class for all ViewModels in the application.
/// Implements INotifyPropertyChanged for data binding support.
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Called when the ViewModel is being initialized.
    /// Override this method to perform any initialization logic.
    /// </summary>
    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the ViewModel is being disposed.
    /// Override this method to perform cleanup operations.
    /// </summary>
    public virtual void Cleanup()
    {
        // Override in derived classes if cleanup is needed
    }
}
