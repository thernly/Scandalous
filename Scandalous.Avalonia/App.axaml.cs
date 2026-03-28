using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using NAPS2.Images.ImageSharp;
using Scandalous.Avalonia.ViewModels;
using Scandalous.Avalonia.Views;
using Scandalous.Core.Services;

namespace Scandalous.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IDocumentScanner>(sp => new DocumentScanner(new ImageSharpImageContext()));
        services.AddSingleton<IConfigurationManager, ConfigurationManager>();
        services.AddSingleton<IScanConfigurationMapper, ScanConfigurationMapper>();
        services.AddSingleton<IPdfService, PdfService>();
        services.AddSingleton<ILanguageCodeService, LanguageCodeService>();
        services.AddSingleton<IScanExceptionHandler, ScanExceptionHandler>();
        var provider = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var vm = new MainWindowViewModel(
                provider.GetRequiredService<IDocumentScanner>(),
                provider.GetRequiredService<IConfigurationManager>(),
                provider.GetRequiredService<IScanConfigurationMapper>(),
                provider.GetRequiredService<IPdfService>(),
                provider.GetRequiredService<ILanguageCodeService>(),
                provider.GetRequiredService<IScanExceptionHandler>()
            );
            desktop.MainWindow = new MainWindow { DataContext = vm };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
