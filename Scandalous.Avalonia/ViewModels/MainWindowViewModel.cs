using CommunityToolkit.Mvvm.ComponentModel;
using Scandalous.Core.Services;

namespace Scandalous.Avalonia.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IDocumentScanner _scanner;
    private readonly IConfigurationManager _configManager;
    private readonly IScanConfigurationMapper _configMapper;
    private readonly IPdfService _pdfService;
    private readonly ILanguageCodeService _languageService;
    private readonly IScanExceptionHandler _exceptionHandler;

    public MainWindowViewModel(
        IDocumentScanner scanner,
        IConfigurationManager configManager,
        IScanConfigurationMapper configMapper,
        IPdfService pdfService,
        ILanguageCodeService languageService,
        IScanExceptionHandler exceptionHandler)
    {
        _scanner = scanner;
        _configManager = configManager;
        _configMapper = configMapper;
        _pdfService = pdfService;
        _languageService = languageService;
        _exceptionHandler = exceptionHandler;
    }
}
