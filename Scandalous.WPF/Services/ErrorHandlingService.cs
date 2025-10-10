using System.Windows;
using Scandalous.Core.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace Scandalous.WPF.Services;

/// <summary>
/// Implementation of the error handling service for WPF.
/// </summary>
public class ErrorHandlingService : IErrorHandlingService
{
    private readonly IScanExceptionHandler _scanExceptionHandler;
    private readonly ILogger<ErrorHandlingService> _logger;

    public ErrorHandlingService(IScanExceptionHandler scanExceptionHandler, ILogger<ErrorHandlingService> logger)
    {
        _scanExceptionHandler = scanExceptionHandler;
        _logger = logger;
    }

    /// <summary>
    /// Shows a user-friendly error dialog.
    /// </summary>
    public async Task ShowErrorAsync(string message, string title = "Error")
    {
        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            System.Windows.MessageBox.Show(
                message,
                title,
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        });
    }

    /// <summary>
    /// Shows a user-friendly exception dialog with detailed information.
    /// </summary>
    public async Task ShowExceptionAsync(Exception exception, string context = "")
    {
        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var message = $"An error occurred{(string.IsNullOrEmpty(context) ? "" : $" while {context}")}:\n\n{exception.Message}";
            
            if (exception.InnerException != null)
            {
                message += $"\n\nDetails: {exception.InnerException.Message}";
            }

            System.Windows.MessageBox.Show(
                message,
                "Error",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        });

        // Log the error for debugging
        LogError(exception, context);
    }

    /// <summary>
    /// Logs an error for debugging purposes.
    /// </summary>
    public void LogError(Exception exception, string context = "")
    {
        _logger.LogError(exception, "Error{Context}: {Message}", 
            string.IsNullOrEmpty(context) ? "" : $" in {context}", 
            exception.Message);
    }

    /// <summary>
    /// Handles a scan-related error using the Core exception handler and shows appropriate UI.
    /// </summary>
    public async Task HandleScanErrorAsync(Exception exception)
    {
        try
        {
            // Use the Core exception handler to get user-friendly message
            var result = _scanExceptionHandler.HandleScanException(exception);
            
            // Show the user-friendly message
            await ShowErrorAsync(result.UserMessage, "Scan Error");
            
            // Log the technical details
            LogError(exception, "Scan operation");
        }
        catch (Exception ex)
        {
            // Fallback if the Core exception handler fails
            await ShowExceptionAsync(ex, "handling scan error");
        }
    }
}