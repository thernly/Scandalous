using System.Threading.Tasks;
using System;

namespace Scandalous.WPF.Services;

/// <summary>
/// Service for handling errors in the WPF application with user-friendly dialogs and logging.
/// </summary>
public interface IErrorHandlingService
{
    /// <summary>
    /// Shows a user-friendly error dialog.
    /// </summary>
    /// <param name="message">The error message to display</param>
    /// <param name="title">The dialog title (optional)</param>
    /// <returns>A task that completes when the dialog is closed</returns>
    Task ShowErrorAsync(string message, string title = "Error");

    /// <summary>
    /// Shows a user-friendly exception dialog with detailed information.
    /// </summary>
    /// <param name="exception">The exception to display</param>
    /// <param name="context">Additional context about where the error occurred</param>
    /// <returns>A task that completes when the dialog is closed</returns>
    Task ShowExceptionAsync(Exception exception, string context = "");

    /// <summary>
    /// Logs an error for debugging purposes.
    /// </summary>
    /// <param name="exception">The exception to log</param>
    /// <param name="context">Additional context about where the error occurred</param>
    void LogError(Exception exception, string context = "");

    /// <summary>
    /// Handles a scan-related error using the Core exception handler and shows appropriate UI.
    /// </summary>
    /// <param name="exception">The scan exception</param>
    /// <returns>A task that completes when the error is handled</returns>
    Task HandleScanErrorAsync(Exception exception);
} 