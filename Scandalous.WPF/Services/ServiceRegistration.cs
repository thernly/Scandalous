using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scandalous.Core.Services;
using Scandalous.Core.Validation;
using Scandalous.WPF.ViewModels;

namespace Scandalous.WPF.Services;

/// <summary>
/// Helper class for registering all services in the dependency injection container.
/// </summary>
public static class ServiceRegistration
{
    /// <summary>
    /// Configures all services for the WPF application.
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    public static void ConfigureServices(IServiceCollection services)
    {
        // Add logging
        services.AddLogging(configure =>
        {
            configure.SetMinimumLevel(LogLevel.Trace);
            configure.AddConsole();
            configure.AddDebug();
        });

        // Register Core services
        RegisterCoreServices(services);
        
        // Register WPF-specific services
        RegisterWpfServices(services);
        
        // Register ViewModels
        RegisterViewModels(services);
    }

    /// <summary>
    /// Registers all Scandalous.Core services.
    /// </summary>
    private static void RegisterCoreServices(IServiceCollection services)
    {
        // Core business logic services
        services.AddSingleton<IDocumentScanner, DocumentScanner>();
        services.AddSingleton<IConfigurationManager, ConfigurationManager>();
        services.AddSingleton<IScanConfigurationMapper, ScanConfigurationMapper>();
        services.AddSingleton<IPdfService, PdfService>();
        services.AddSingleton<ILanguageCodeService, LanguageCodeService>();
        services.AddSingleton<IScanExceptionHandler, ScanExceptionHandler>();
        
        // Note: FileNameValidator and FolderValidator are static classes
        // and cannot be registered as services. They should be used directly.
    }

    /// <summary>
    /// Registers WPF-specific services.
    /// </summary>
    private static void RegisterWpfServices(IServiceCollection services)
    {
        // WPF-specific services can be added here
        // For example: dialog services, theme services, etc.
        
        // Error handling service for WPF
        services.AddSingleton<IErrorHandlingService, ErrorHandlingService>();
        
        // Configuration dialog service
        services.AddSingleton<IConfigurationDialogService, ConfigurationDialogService>();
        
        // Theme service
        services.AddSingleton<IThemeService, ThemeService>();
    }

    /// <summary>
    /// Registers all ViewModels.
    /// </summary>
    private static void RegisterViewModels(IServiceCollection services)
    {
        // Main ViewModel
        services.AddTransient<MainWindowViewModel>();
        
        // Future ViewModels can be added here
        // services.AddTransient<ScanConfigurationViewModel>();
        // services.AddTransient<ImagePreviewViewModel>();
        // services.AddTransient<ProgressViewModel>();
    }
} 