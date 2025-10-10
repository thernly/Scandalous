using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scandalous.WPF.Services;
using Scandalous.WPF.ViewModels;
using Scandalous.WPF.Views;

namespace Scandalous.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    private ServiceProvider? _serviceProvider;
    private ApplicationLifecycleService? _lifecycleService;

    public App()
    {
        this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    protected override async void OnStartup(System.Windows.StartupEventArgs e)
    {
        base.OnStartup(e);
        try
        {
            ConfigureServices();
            
            // Initialize application
            _lifecycleService = _serviceProvider?.GetService<ApplicationLifecycleService>();
            if (_lifecycleService != null)
            {
                await _lifecycleService.InitializeAsync();
            }
            
            var mainWindow = _serviceProvider?.GetService<MainWindow>();
            mainWindow?.Show();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Startup error: {ex}");
            try { System.Windows.MessageBox.Show($"Startup error: {ex.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error); } catch { }
            Shutdown(-1);
        }
    }

    protected override async void OnExit(System.Windows.ExitEventArgs e)
    {
        // Perform cleanup before exit
        if (_lifecycleService != null)
        {
            await _lifecycleService.ShutdownAsync();
            _lifecycleService.OnApplicationExit();
        }
        
        // Dispose service provider
        _serviceProvider?.Dispose();
        
        base.OnExit(e);
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();
        
        // Use the centralized service registration
        ServiceRegistration.ConfigureServices(services);
        
        // Register Views
        services.AddTransient<MainWindow>();
        
        // Register lifecycle service
        services.AddSingleton<ApplicationLifecycleService>();
        
        _serviceProvider = services.BuildServiceProvider();
    }

    private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        Console.Error.WriteLine($"Unhandled UI exception: {e.Exception}");
        try { System.Windows.MessageBox.Show($"Unhandled UI exception: {e.Exception.Message}", "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error); } catch { }
        e.Handled = true;
        Shutdown(-1);
    }

    private void CurrentDomain_UnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            Console.Error.WriteLine($"Unhandled domain exception: {ex}");
            try { System.Windows.MessageBox.Show($"Unhandled domain exception: {ex.Message}", "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error); } catch { }
        }
        else
        {
            Console.Error.WriteLine($"Unhandled domain exception: {e.ExceptionObject}");
        }
        Shutdown(-1);
    }
}

