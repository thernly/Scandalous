using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace Scandalous.WPF.ViewModels;

/// <summary>
/// Base ViewModel class that provides common functionality for all ViewModels.
/// Inherits from ObservableObject for automatic INotifyPropertyChanged implementation.
/// </summary>
public abstract class BaseViewModel : ObservableObject
{
    private bool _isLoading;
    private string _statusMessage = string.Empty;

    /// <summary>
    /// Gets or sets whether the ViewModel is currently performing a long-running operation.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    /// <summary>
    /// Gets or sets the current status message to display to the user.
    /// </summary>
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    /// <summary>
    /// Sets the busy state and status message atomically.
    /// </summary>
    /// <param name="isBusy">Whether the operation is busy</param>
    /// <param name="statusMessage">The status message to display</param>
    protected void SetBusyState(bool isLoading, string statusMessage = "")
    {
        IsLoading = isLoading;
        StatusMessage = statusMessage;
    }

    /// <summary>
    /// Clears the busy state and status message.
    /// </summary>
    protected void ClearBusyState()
    {
        SetBusyState(false, string.Empty);
    }
} 