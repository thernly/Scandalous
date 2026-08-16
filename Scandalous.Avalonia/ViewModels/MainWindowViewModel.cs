using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Scandalous.Core.Enums;
using Scandalous.Core.Models;
using Scandalous.Core.Services;
using Scandalous.Core.Validation;

namespace Scandalous.Avalonia.ViewModels;

/// <summary>Text for a mid-scan confirmation dialog, including its explicit button labels.</summary>
public sealed record ContinuationPrompt(string Title, string Message, string PrimaryLabel, string SecondaryLabel);

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
    /// <summary>
    /// Shows a confirmation dialog: (title, message, primaryButtonLabel, secondaryButtonLabel).
    /// Returns true when the primary action was chosen.
    /// </summary>
    public Func<string, string, string, string, Task<bool>>? ShowConfirmationDialogAsync { get; set; }
    public Func<string, string, Task>? ShowErrorDialogAsync { get; set; }

    // Observable properties
    [ObservableProperty]
    [CustomValidation(typeof(MainWindowViewModel), nameof(ValidateOutputFolder))]
    private string outputFolder = string.Empty;

    [ObservableProperty]
    [CustomValidation(typeof(MainWindowViewModel), nameof(ValidateBaseFilename))]
    private string baseFilename = "output";

    [ObservableProperty] private ScannerColorMode colorMode = ScannerColorMode.Grayscale;
    [ObservableProperty] private DocumentOptions documentOption = DocumentOptions.Combined;
    [ObservableProperty] private bool autoDeskew = true;
    [ObservableProperty] private bool excludeBlankPages = true;
    [ObservableProperty] private int selectedDpi = 300;
    [ObservableProperty] private ScannerPaperSource paperSource = ScannerPaperSource.FeederDuplex;
    [ObservableProperty] private ScannerPaperSize paperSize = ScannerPaperSize.Letter;
    [ObservableProperty] private bool ocrEnabled = true;
    [ObservableProperty] private bool isOcrSettingsExpanded = false;

    [ObservableProperty]
    [CustomValidation(typeof(MainWindowViewModel), nameof(ValidateTessdataFolder))]
    private string tessdataFolder = string.Empty;

    [ObservableProperty]
    [CustomValidation(typeof(MainWindowViewModel), nameof(ValidateSelectedLanguageCode))]
    private string selectedLanguageCode = "eng";

    [ObservableProperty]
    [CustomValidation(typeof(MainWindowViewModel), nameof(ValidateSelectedScanner))]
    private string selectedScanner = string.Empty;

    private string _selectedScannerUrl = string.Empty;
    [ObservableProperty] private string statusText = "Searching for scanners...";
    [ObservableProperty] private StatusKind statusKind = StatusKind.Working;
    [ObservableProperty] private string completionText = string.Empty;
    [ObservableProperty] private string completionToolTip = string.Empty;
    [ObservableProperty] private string completionWarningText = string.Empty;
    [ObservableProperty] private bool canOpenPdf = false;
    [ObservableProperty] private bool canOpenOutputFolder = false;
    [ObservableProperty] private bool isScanning = false;
    [ObservableProperty] private bool isCancelRequested = false;
    [ObservableProperty] private string? previewImagePath = null;
    [ObservableProperty] private int pageCount = 0;
    private CancellationTokenSource? _scanCancellationSource;
    private TaskCompletionSource<object?>? _activeScanCompletion;
    private ScanResult? _lastSuccessfulScanResult;

    public bool CanCancelScan => IsScanning && !IsCancelRequested;

    // Static/collection properties
    public int[] DpiOptions { get; } = [150, 300, 600, 1200];
    public ObservableCollection<string> Scanners { get; } = [];
    public ObservableCollection<string> AvailableLanguageCodes { get; } = [];

    // Display-name arrays for enum ComboBoxes
    public ScannerColorMode[] ColorModeOptions { get; } = [ScannerColorMode.Grayscale, ScannerColorMode.BlackAndWhite, ScannerColorMode.Color];
    public ScannerPaperSource[] PaperSourceOptions { get; } = [ScannerPaperSource.FeederDuplex, ScannerPaperSource.FeederSimplex, ScannerPaperSource.Flatbed];
    public ScannerPaperSize[] PaperSizeOptions { get; } = [ScannerPaperSize.Letter, ScannerPaperSize.A4, ScannerPaperSize.Legal];
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

    public static string FormatPaperSize(ScannerPaperSize size) => size switch
    {
        ScannerPaperSize.Letter => "Letter",
        ScannerPaperSize.A4 => "A4",
        ScannerPaperSize.Legal => "Legal",
        _ => size.ToString()
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

    public static ValidationResult? ValidateSelectedScanner(string? value, ValidationContext context)
    {
        if (string.IsNullOrEmpty(value))
            return new ValidationResult("A scanner must be selected.");
        return ValidationResult.Success;
    }

    public static ValidationResult? ValidateOutputFolder(string? value, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new ValidationResult("Output folder is required.");
        if (!Directory.Exists(value))
            return new ValidationResult("Output folder does not exist.");
        return ValidationResult.Success;
    }

    public static ValidationResult? ValidateTessdataFolder(string? value, ValidationContext context)
    {
        var vm = (MainWindowViewModel)context.ObjectInstance;
        if (!vm.OcrEnabled) return ValidationResult.Success;
        if (string.IsNullOrWhiteSpace(value))
            return new ValidationResult("Tessdata folder is required when OCR is enabled.");
        if (!Directory.Exists(value))
            return new ValidationResult("Tessdata folder does not exist.");
        return ValidationResult.Success;
    }

    public static ValidationResult? ValidateSelectedLanguageCode(string? value, ValidationContext context)
    {
        var vm = (MainWindowViewModel)context.ObjectInstance;
        if (!vm.OcrEnabled) return ValidationResult.Success;
        if (string.IsNullOrWhiteSpace(value))
            return new ValidationResult("An OCR language must be selected.");
        if (!string.IsNullOrWhiteSpace(vm.TessdataFolder))
        {
            var path = Path.Combine(vm.TessdataFolder, $"{value}.traineddata");
            if (!File.Exists(path))
                return new ValidationResult($"{value}.traineddata not found in tessdata folder.");
        }
        return ValidationResult.Success;
    }

    private string GetFirstErrorMessage(string propertyName) =>
        GetErrors(propertyName).OfType<ValidationResult>().FirstOrDefault()?.ErrorMessage ?? string.Empty;

    public string OutputFolderError => GetFirstErrorMessage(nameof(OutputFolder));
    public string SelectedScannerError => GetFirstErrorMessage(nameof(SelectedScanner));
    public string TessdataFolderError => GetFirstErrorMessage(nameof(TessdataFolder));
    public string SelectedLanguageCodeError => GetFirstErrorMessage(nameof(SelectedLanguageCode));
    public string NoOcrLanguagesText => AvailableLanguageCodes.Count == 0 ? "No OCR languages found" : string.Empty;
    public bool AreScanSettingsEnabled => !IsScanning;

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

        ErrorsChanged += (_, e) =>
        {
            ScanCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(OutputFolderError));
            OnPropertyChanged(nameof(SelectedScannerError));
            OnPropertyChanged(nameof(TessdataFolderError));
            OnPropertyChanged(nameof(SelectedLanguageCodeError));
        };

        // Set platform-appropriate defaults
        OutputFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        TessdataFolder = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? @"C:\tessdata"
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "tessdata");

        ValidateAllProperties();
    }

    partial void OnIsScanningChanged(bool value)
    {
        ScanCommand.NotifyCanExecuteChanged();
        CancelCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(AreScanSettingsEnabled));
        OnPropertyChanged(nameof(CanCancelScan));
    }

    partial void OnIsCancelRequestedChanged(bool value)
    {
        CancelCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(CanCancelScan));
    }

    partial void OnCanOpenPdfChanged(bool value) => OpenPdfCommand.NotifyCanExecuteChanged();

    partial void OnCanOpenOutputFolderChanged(bool value) => OpenOutputFolderCommand.NotifyCanExecuteChanged();

    partial void OnOutputFolderChanged(string value) => ValidateProperty(value, nameof(OutputFolder));
    partial void OnBaseFilenameChanged(string value) => ValidateProperty(value, nameof(BaseFilename));
    partial void OnSelectedScannerChanged(string value) => ValidateProperty(value, nameof(SelectedScanner));

    partial void OnOcrEnabledChanged(bool value)
    {
        ValidateProperty(TessdataFolder, nameof(TessdataFolder));
        ValidateProperty(SelectedLanguageCode, nameof(SelectedLanguageCode));
    }

    partial void OnTessdataFolderChanged(string value)
    {
        ValidateProperty(value, nameof(TessdataFolder));
        if (OcrEnabled)
            ValidateProperty(SelectedLanguageCode, nameof(SelectedLanguageCode));
    }

    partial void OnSelectedLanguageCodeChanged(string value)
    {
        if (OcrEnabled)
            ValidateProperty(value, nameof(SelectedLanguageCode));
    }

    private bool CanScan() => !IsScanning && !HasErrors;

    public static readonly ContinuationPrompt FlatbedContinuationPrompt = new(
        "More Pages?",
        "Place the next page on the flatbed, then choose Scan Next Page. Choose Finish to create the PDF.",
        "Scan Next Page",
        "Finish");

    /// <summary>
    /// Returns the mid-scan continuation prompt for a configuration, or null when the scan
    /// does not need one.
    /// </summary>
    public static ContinuationPrompt? GetContinuationPrompt(ScanConfiguration configuration) =>
        configuration.ScannerPaperSource == ScannerPaperSource.Flatbed
        && configuration.DocumentOptions == DocumentOptions.Combined
            ? FlatbedContinuationPrompt
            : null;

    private void SetStatus(StatusKind kind, string text)
    {
        this.StatusKind = kind;
        StatusText = text;
    }

    private void UpdateCompletionState(ScanResult? scanResult)
    {
        _lastSuccessfulScanResult = scanResult;
        CompletionWarningText = string.Empty;

        var outputFiles = scanResult?.OutputFiles
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .ToList() ?? [];

        if (scanResult == null || scanResult.CapturedPageCount == 0 || outputFiles.Count == 0)
        {
            CompletionText = "No pages were captured. No PDF was created.";
            CompletionToolTip = string.Empty;
            CanOpenPdf = false;
            CanOpenOutputFolder = false;
            SetStatus(StatusKind.Warning, CompletionText);
            return;
        }

        if (outputFiles.Count == 1)
        {
            var outputPath = outputFiles[0];
            CompletionText = $"{Path.GetFileName(outputPath)} was created.";
            CompletionToolTip = outputPath;
            CanOpenPdf = true;
            CanOpenOutputFolder = true;
            SetStatus(StatusKind.Success, CompletionText);
            return;
        }

        CompletionText = $"{outputFiles.Count} PDF files were created.";
        CompletionToolTip = string.Join(Environment.NewLine, outputFiles);
        CanOpenPdf = false;
        CanOpenOutputFolder = true;
        SetStatus(StatusKind.Success, CompletionText);
    }

    private void TryOpenPdf(string pdfFilePath)
    {
        try
        {
            _pdfService.OpenPdfFile(pdfFilePath, OutputFolder);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MainWindowViewModel] OpenPdf failed. Path: '{pdfFilePath}', OutputFolder: '{OutputFolder}'. Exception: {ex}");
            Console.Error.WriteLine($"[MainWindowViewModel] OpenPdf failed. Path: '{pdfFilePath}', OutputFolder: '{OutputFolder}'. Exception: {ex.Message}");
            CompletionWarningText = "Warning: Could not open the created PDF.";
        }
    }

    private void TryOpenOutputFolder(string outputFolderPath)
    {
        try
        {
            _pdfService.OpenOutputFolder(outputFolderPath, OutputFolder);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MainWindowViewModel] OpenOutputFolder failed. Path: '{outputFolderPath}', OutputFolder: '{OutputFolder}'. Exception: {ex}");
            Console.Error.WriteLine($"[MainWindowViewModel] OpenOutputFolder failed. Path: '{outputFolderPath}', OutputFolder: '{OutputFolder}'. Exception: {ex.Message}");
            CompletionWarningText = "Warning: Could not open the output folder.";
        }
    }

    [RelayCommand(CanExecute = nameof(CanScan))]
    private async Task ScanAsync()
    {
        var scanCompletion = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        _activeScanCompletion = scanCompletion;
        IsScanning = true;
        IsCancelRequested = false;
        PageCount = 0;
        CompletionText = string.Empty;
        CompletionToolTip = string.Empty;
        CompletionWarningText = string.Empty;
        CanOpenPdf = false;
        CanOpenOutputFolder = false;
        _lastSuccessfulScanResult = null;
        SetStatus(StatusKind.Working, "Connecting to scanner...");

        var scanCts = new CancellationTokenSource();
        _scanCancellationSource = scanCts;

        void PageScannedHandler(object? sender, Core.Models.PageScannedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                PageCount++;
                PreviewImagePath = e.ImageFilePath;
                SetStatus(StatusKind.Working, $"Scanned {PageCount} page(s)...");
            });
        }

        var pageHandlerSubscribed = false;
        try
        {
            var uiState = BuildUIState();
            var configuration = _configMapper.BuildConfigurationFromUIState(uiState);

            _scanner.PageScanned += PageScannedHandler;
            pageHandlerSubscribed = true;

            Func<Task<bool>>? promptForMorePages = null;
            var continuationPrompt = GetContinuationPrompt(configuration);
            if (continuationPrompt != null && ShowConfirmationDialogAsync != null)
            {
                var showDialog = ShowConfirmationDialogAsync;
                promptForMorePages = () => Dispatcher.UIThread.InvokeAsync(() =>
                    showDialog(
                        continuationPrompt.Title,
                        continuationPrompt.Message,
                        continuationPrompt.PrimaryLabel,
                        continuationPrompt.SecondaryLabel));
            }

            var scanResult = await _scanner.ScanDocuments(configuration, scanCts.Token, promptForMorePages: promptForMorePages);
            UpdateCompletionState(scanResult);

            if (configuration.DocumentOptions == DocumentOptions.Combined
                && scanResult.OutputFiles.Count == 1)
            {
                var outputPath = scanResult.OutputFiles[0];
                if (!string.IsNullOrWhiteSpace(outputPath)
                    && _pdfService.PdfFileExists(outputPath))
                {
                    TryOpenPdf(outputPath);
                }
            }
        }
        catch (OperationCanceledException)
        {
            SetStatus(StatusKind.Warning, "Scan canceled.");
        }
        catch (Exception ex)
        {
            var result = _exceptionHandler.HandleScanException(ex);
            var userMessage = string.IsNullOrWhiteSpace(result.UserMessage)
                ? "Scanning could not be completed. Please try again."
                : result.UserMessage;
            SetStatus(StatusKind.Error, userMessage);
            if (ShowErrorDialogAsync != null)
                await ShowErrorDialogAsync("Error", userMessage);
        }
        finally
        {
            try
            {
                if (pageHandlerSubscribed)
                    _scanner.PageScanned -= PageScannedHandler;
            }
            finally
            {
                scanCompletion.TrySetResult(null);
                if (ReferenceEquals(_activeScanCompletion, scanCompletion))
                    _activeScanCompletion = null;

                _scanCancellationSource?.Dispose();
                _scanCancellationSource = null;
                IsCancelRequested = false;
                IsScanning = false;
            }
        }
    }

    public async Task CancelScanAndWaitAsync()
    {
        if (!IsScanning)
            return;

        await CancelAsync();

        var activeScanTask = _activeScanCompletion?.Task;
        if (activeScanTask != null)
            await activeScanTask;
    }

    [RelayCommand(CanExecute = nameof(CanCancelScan))]
    private Task CancelAsync()
    {
        if (!IsScanning || _scanCancellationSource == null || IsCancelRequested)
            return Task.CompletedTask;

        IsCancelRequested = true;
        SetStatus(StatusKind.Working, "Canceling scan...");
        _scanCancellationSource.Cancel();
        return Task.CompletedTask;
    }

    [RelayCommand(CanExecute = nameof(CanOpenPdf))]
    private void OpenPdf()
    {
        var outputPath = _lastSuccessfulScanResult?.OutputFiles
            .FirstOrDefault(path => !string.IsNullOrWhiteSpace(path));

        if (string.IsNullOrWhiteSpace(outputPath))
            return;

        TryOpenPdf(outputPath);
    }

    [RelayCommand(CanExecute = nameof(CanOpenOutputFolder))]
    private void OpenOutputFolder()
    {
        var outputFiles = _lastSuccessfulScanResult?.OutputFiles
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .ToList() ?? [];

        if (outputFiles.Count == 0)
            return;

        var outputFolderPath = Path.GetDirectoryName(outputFiles[0]) ?? OutputFolder;
        TryOpenOutputFolder(outputFolderPath);
    }

    [RelayCommand]
    private async Task GetScannersAsync()
    {
        try
        {
            SetStatus(StatusKind.Working, "Searching for scanners...");
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

            var selectedDevice = devices.FirstOrDefault(d => d.Name == target);
            if (selectedDevice != null)
                _selectedScannerUrl = selectedDevice.ID;

            // Defer selection so the ComboBox has processed the new items.
            Dispatcher.UIThread.Post(() => SelectedScanner = target);

            if (Scanners.Count > 0)
                SetStatus(StatusKind.Success, $"Found {Scanners.Count} scanner(s).");
            else
                SetStatus(StatusKind.Warning, "No scanners found. Check that the scanner is powered on and connected, then Refresh.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MainWindowViewModel] Scanner discovery failed. Exception: {ex}");
            Console.Error.WriteLine($"[MainWindowViewModel] Scanner discovery failed. Exception: {ex.Message}");

            var handled = _exceptionHandler.HandleScanException(ex);
            var userMessage = handled.IsHandled && !string.IsNullOrWhiteSpace(handled.UserMessage)
                ? handled.UserMessage
                : "Could not search for scanners. Check that the scanner is connected, then try again.";
            SetStatus(StatusKind.Error, userMessage);
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
        var uiState = _configMapper.BuildUIStateFromConfiguration(config) ?? new UIState();
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
        var previousCode = SelectedLanguageCode;
        var codes = _languageService.GetAvailableLanguageCodes(TessdataFolder, previousCode);

        AvailableLanguageCodes.Clear();
        SelectedLanguageCode = string.Empty; // ensure a real property change when we re-assign below
        foreach (var code in codes)
            AvailableLanguageCodes.Add(code);

        OnPropertyChanged(nameof(NoOcrLanguagesText));

        if (AvailableLanguageCodes.Count == 0)
        {
            if (OcrEnabled)
                OcrEnabled = false;
            return;
        }

        var exactMatch = AvailableLanguageCodes.FirstOrDefault(code =>
            string.Equals(code, previousCode, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(exactMatch))
        {
            SelectedLanguageCode = exactMatch;
            return;
        }

        var bestLanguageCode = _languageService.GetBestLanguageCode(TessdataFolder, previousCode);
        if (!string.IsNullOrWhiteSpace(bestLanguageCode))
            SelectedLanguageCode = bestLanguageCode;
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
        PaperSize = PaperSize,
        Dpi = SelectedDpi,
        OcrEnabled = OcrEnabled,
        TessdataFolder = TessdataFolder,
        SelectedLanguageCode = SelectedLanguageCode,
        SelectedScannerName = SelectedScanner,
        SelectedScannerUrl = _selectedScannerUrl
    };

    private void ApplyUIState(UIState uiState)
    {
        if (uiState == null)
            return;

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
        PaperSize = uiState.PaperSize;
        SelectedDpi = uiState.Dpi > 0 ? uiState.Dpi : 300;
        OcrEnabled = uiState.OcrEnabled;
        if (!string.IsNullOrEmpty(uiState.TessdataFolder)) TessdataFolder = uiState.TessdataFolder;
        if (!string.IsNullOrEmpty(uiState.SelectedLanguageCode)) SelectedLanguageCode = uiState.SelectedLanguageCode;
        if (!string.IsNullOrEmpty(uiState.SelectedScannerName)) SelectedScanner = uiState.SelectedScannerName;
        if (!string.IsNullOrEmpty(uiState.SelectedScannerUrl)) _selectedScannerUrl = uiState.SelectedScannerUrl;
    }
}
