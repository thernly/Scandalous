using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Scandalous.Core.Enums;
using Scandalous.Core.Services;
using Scandalous.Core.Validation;

namespace Scandalous.Avalonia.ViewModels;

public partial class MainWindowViewModel : ObservableValidator
{
    private readonly IDocumentScanner _scanner;
    private readonly IConfigurationManager _configManager;
    private readonly IScanConfigurationMapper _configMapper;
    private readonly IPdfService _pdfService;
    private readonly ILanguageCodeService _languageService;
    private readonly IScanExceptionHandler _exceptionHandler;

    // Folder picker / dialog callbacks set by the View
    public Func<Task<string?>>? PickOutputFolderAsync { get; set; }
    public Func<Task<string?>>? PickTessdataFolderAsync { get; set; }
    public Func<string, string, Task<bool>>? ShowYesNoDialogAsync { get; set; }
    public Func<string, string, Task>? ShowErrorDialogAsync { get; set; }

    // Observable properties
    [ObservableProperty] private string outputFolder = string.Empty;
    [ObservableProperty]
    [CustomValidation(typeof(MainWindowViewModel), nameof(ValidateBaseFilename))]
    private string baseFilename = "output";
    [ObservableProperty] private ScannerColorMode colorMode = ScannerColorMode.Grayscale;
    [ObservableProperty] private DocumentOptions documentOption = DocumentOptions.Combined;
    [ObservableProperty] private bool autoDeskew = true;
    [ObservableProperty] private bool excludeBlankPages = true;
    [ObservableProperty] private int selectedDpi = 300;
    [ObservableProperty] private ScannerPaperSource paperSource = ScannerPaperSource.FeederDuplex;
    [ObservableProperty] private bool ocrEnabled = true;
    [ObservableProperty] private string tessdataFolder = string.Empty;
    [ObservableProperty] private string selectedLanguageCode = "eng";
    [ObservableProperty] private string selectedScanner = string.Empty;
    [ObservableProperty] private string statusText = "Searching for scanners...";
    [ObservableProperty] private bool isScanning = false;
    [ObservableProperty] private string? previewImagePath = null;
    [ObservableProperty] private int pageCount = 0;

    // Static/collection properties
    public int[] DpiOptions { get; } = [150, 300, 600, 1200];
    public ObservableCollection<string> Scanners { get; } = [];
    public ObservableCollection<string> AvailableLanguageCodes { get; } = [];

    // Display-name arrays for enum ComboBoxes
    public ScannerColorMode[] ColorModeOptions { get; } = [ScannerColorMode.Grayscale, ScannerColorMode.BlackAndWhite, ScannerColorMode.Color];
    public ScannerPaperSource[] PaperSourceOptions { get; } = [ScannerPaperSource.FeederDuplex, ScannerPaperSource.FeederSimplex, ScannerPaperSource.Flatbed];
    public DocumentOptions[] DocumentOptionOptions { get; } = [DocumentOptions.Combined, DocumentOptions.Individual];

    public static string FormatColorMode(ScannerColorMode mode) => mode switch
    {
        ScannerColorMode.Grayscale => "Grayscale",
        ScannerColorMode.BlackAndWhite => "Black & White",
        ScannerColorMode.Color => "Color",
        _ => mode.ToString()
    };

    public static string FormatPaperSource(ScannerPaperSource source) => source switch
    {
        ScannerPaperSource.FeederDuplex => "Feeder (Duplex)",
        ScannerPaperSource.FeederSimplex => "Feeder (Simplex)",
        ScannerPaperSource.Flatbed => "Flatbed",
        ScannerPaperSource.Auto => "Auto",
        _ => source.ToString()
    };

    public static string FormatDocumentOption(DocumentOptions option) => option switch
    {
        DocumentOptions.Combined => "Combined PDF",
        DocumentOptions.Individual => "Individual PDFs",
        _ => option.ToString()
    };

    public static ValidationResult? ValidateBaseFilename(string? value, ValidationContext context)
    {
        var (isValid, errorMessage) = FileNameValidator.IsValid(value, isBaseNameOnly: true);
        return isValid ? ValidationResult.Success : new ValidationResult(errorMessage);
    }

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

        // Set platform-appropriate defaults
        OutputFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        TessdataFolder = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? @"C:\tessdata"
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "tessdata");
    }

    partial void OnIsScanningChanged(bool value) => ScanCommand.NotifyCanExecuteChanged();
    partial void OnSelectedScannerChanged(string value) => ScanCommand.NotifyCanExecuteChanged();

    private bool CanScan() => !IsScanning && !string.IsNullOrEmpty(SelectedScanner);

    [RelayCommand(CanExecute = nameof(CanScan))]
    private async Task ScanAsync()
    {
        IsScanning = true;
        PageCount = 0;
        StatusText = "Building configuration...";

        var uiState = BuildUIState();
        var configuration = _configMapper.BuildConfigurationFromUIState(uiState);

        void PageScannedHandler(object? sender, Core.Models.PageScannedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                PageCount++;
                PreviewImagePath = e.ImageFilePath;
                StatusText = $"Scanned {PageCount} page(s)...";
            });
        }

        _scanner.PageScanned += PageScannedHandler;

        Func<Task<bool>>? promptForMorePages = null;
        if (PaperSource == ScannerPaperSource.Flatbed && DocumentOption == DocumentOptions.Combined && ShowYesNoDialogAsync != null)
        {
            promptForMorePages = () => Dispatcher.UIThread.InvokeAsync(() =>
                ShowYesNoDialogAsync("More Pages?", "Place the next page on the flatbed and click Yes, or click No to finish."));
        }

        string outputPath = string.Empty;
        try
        {
            outputPath = await _scanner.ScanDocuments(configuration, promptForMorePages: promptForMorePages);
            StatusText = "Scanning completed.";
        }
        catch (Exception ex)
        {
            var result = _exceptionHandler.HandleScanException(ex);
            StatusText = result.UserMessage;
            if (ShowErrorDialogAsync != null)
                await ShowErrorDialogAsync("Error", result.UserMessage);
        }
        finally
        {
            _scanner.PageScanned -= PageScannedHandler;
            IsScanning = false;
        }

        if (DocumentOption == DocumentOptions.Combined && !string.IsNullOrEmpty(outputPath))
        {
            if (_pdfService.PdfFileExists(outputPath))
                _pdfService.OpenPdfFile(outputPath, configuration.OutputFolder);
        }
    }

    [RelayCommand]
    private async Task GetScannersAsync()
    {
        try
        {
            StatusText = "Searching for scanners...";
            var previousSelection = SelectedScanner;
            var devices = await _scanner.GetScanDevicesAsync();
            var names = devices.Select(d => d.Name).ToList();

            Scanners.Clear();
            SelectedScanner = string.Empty;
            foreach (var name in names)
                Scanners.Add(name);

            var target = !string.IsNullOrEmpty(previousSelection) && Scanners.Contains(previousSelection)
                ? previousSelection
                : Scanners.Count > 0 ? Scanners[0] : string.Empty;

            // Defer selection so the ComboBox has processed the new items.
            Dispatcher.UIThread.Post(() => SelectedScanner = target);

            StatusText = Scanners.Count > 0
                ? $"Found {Scanners.Count} scanner(s)."
                : "No scanners found.";
        }
        catch (Exception ex)
        {
            StatusText = $"Could not list scanners: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task BrowseOutputFolder()
    {
        if (PickOutputFolderAsync == null) return;
        var folder = await PickOutputFolderAsync();
        if (folder != null)
            OutputFolder = folder;
    }

    [RelayCommand]
    private async Task BrowseTessdataFolder()
    {
        if (PickTessdataFolderAsync == null) return;
        var folder = await PickTessdataFolderAsync();
        if (folder != null)
        {
            TessdataFolder = folder;
            RefreshLanguageCodes();
        }
    }

    public async Task InitializeAsync()
    {
        var config = await _configManager.LoadConfigurationAsync();
        var uiState = _configMapper.BuildUIStateFromConfiguration(config);
        ApplyUIState(uiState);

        RefreshLanguageCodes();
        await GetScannersAsync();
    }

    public async Task SaveConfigurationAsync()
    {
        var uiState = BuildUIState();
        var config = _configMapper.BuildConfigurationFromUIState(uiState);
        await _configManager.SaveConfigurationAsync(config);
    }

    public Task<Core.Models.WindowStateInfo?> LoadWindowStateAsync() => _configManager.LoadWindowStateAsync();
    public Task SaveWindowStateAsync(Core.Models.WindowStateInfo state) => _configManager.SaveWindowStateAsync(state);

    private void RefreshLanguageCodes()
    {
        var codes = _languageService.GetAvailableLanguageCodes(TessdataFolder, SelectedLanguageCode);
        AvailableLanguageCodes.Clear();
        foreach (var code in codes)
            AvailableLanguageCodes.Add(code);

        if (!string.IsNullOrEmpty(SelectedLanguageCode) && AvailableLanguageCodes.Contains(SelectedLanguageCode))
            return;
        if (AvailableLanguageCodes.Count > 0)
            SelectedLanguageCode = AvailableLanguageCodes[0];
    }

    private UIState BuildUIState() => new UIState
    {
        OutputFolder = OutputFolder,
        BaseFileName = BaseFilename,
        AutoDeskew = AutoDeskew,
        ExcludeBlankPages = ExcludeBlankPages,
        DocumentCombined = DocumentOption == DocumentOptions.Combined,
        DocumentIndividual = DocumentOption == DocumentOptions.Individual,
        ColorModeGrayscale = ColorMode == ScannerColorMode.Grayscale,
        ColorModeBlackWhite = ColorMode == ScannerColorMode.BlackAndWhite,
        ColorModeColor = ColorMode == ScannerColorMode.Color,
        FeederDuplex = PaperSource == ScannerPaperSource.FeederDuplex,
        FeederSimplex = PaperSource == ScannerPaperSource.FeederSimplex,
        Flatbed = PaperSource == ScannerPaperSource.Flatbed,
        Dpi = SelectedDpi,
        OcrEnabled = OcrEnabled,
        TessdataFolder = TessdataFolder,
        SelectedLanguageCode = SelectedLanguageCode,
        SelectedScannerName = SelectedScanner
    };

    private void ApplyUIState(UIState uiState)
    {
        if (!string.IsNullOrEmpty(uiState.OutputFolder)) OutputFolder = uiState.OutputFolder;
        if (!string.IsNullOrEmpty(uiState.BaseFileName)) BaseFilename = uiState.BaseFileName;
        AutoDeskew = uiState.AutoDeskew;
        ExcludeBlankPages = uiState.ExcludeBlankPages;
        DocumentOption = uiState.DocumentCombined ? DocumentOptions.Combined : DocumentOptions.Individual;
        ColorMode = uiState.ColorModeBlackWhite ? ScannerColorMode.BlackAndWhite
                  : uiState.ColorModeColor ? ScannerColorMode.Color
                  : ScannerColorMode.Grayscale;
        PaperSource = uiState.FeederSimplex ? ScannerPaperSource.FeederSimplex
                    : uiState.Flatbed ? ScannerPaperSource.Flatbed
                    : ScannerPaperSource.FeederDuplex;
        SelectedDpi = uiState.Dpi > 0 ? uiState.Dpi : 300;
        OcrEnabled = uiState.OcrEnabled;
        if (!string.IsNullOrEmpty(uiState.TessdataFolder)) TessdataFolder = uiState.TessdataFolder;
        if (!string.IsNullOrEmpty(uiState.SelectedLanguageCode)) SelectedLanguageCode = uiState.SelectedLanguageCode;
        if (!string.IsNullOrEmpty(uiState.SelectedScannerName)) SelectedScanner = uiState.SelectedScannerName;
    }
}
