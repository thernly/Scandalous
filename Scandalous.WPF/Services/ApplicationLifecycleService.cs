using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scandalous.Core.Services;

namespace Scandalous.WPF.Services;

/// <summary>
/// Service for managing application lifecycle events.
/// </summary>
public class ApplicationLifecycleService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfigurationManager _configurationManager;
    private readonly IErrorHandlingService _errorHandlingService;

    public ApplicationLifecycleService(
        IServiceProvider serviceProvider,
        IConfigurationManager configurationManager,
        IErrorHandlingService errorHandlingService)
    {
        _serviceProvider = serviceProvider;
        _configurationManager = configurationManager;
        _errorHandlingService = errorHandlingService;
    }

    /// <summary>
    /// Initializes the application on startup.
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            // Load initial configuration
            await _configurationManager.LoadConfigurationAsync();
            
            // Additional initialization can be added here
            // For example: logging setup, theme loading, etc.
        }
        catch (Exception ex)
        {
            await _errorHandlingService.ShowExceptionAsync(ex, "initializing application");
        }
    }

    /// <summary>
    /// Performs cleanup operations on application shutdown.
    /// </summary>
    public async Task ShutdownAsync()
    {
        try
        {
            // Get current configuration and save it
            var configuration = await _configurationManager.LoadConfigurationAsync();
            await _configurationManager.SaveConfigurationAsync(configuration);
            
            // Additional cleanup can be added here
            // For example: disposing resources, closing connections, etc.
        }
        catch (Exception ex)
        {
            // Log the error but don't show UI during shutdown
            _errorHandlingService.LogError(ex, "application shutdown");
        }
    }

    /// <summary>
    /// Handles application exit events.
    /// </summary>
    public void OnApplicationExit()
    {
        // Perform any synchronous cleanup
        // Note: This method should not be async as it's called during shutdown
    }
} 