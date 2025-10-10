using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Scandalous.Core.Models;
using Scandalous.Core.Services;
using Scandalous.WPF.Services;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Threading.Tasks;
using Scandalous.Core.Enums;
using System.Threading;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows;

namespace Scandalous.WPF.ViewModels
{
    public partial class MainWindowViewModel : BaseViewModel
    {
        private readonly IDocumentScanner _documentScanner;
        private readonly IConfigurationManager _configurationManager;
        private readonly IScanConfigurationMapper _scanConfigurationMapper;
        private readonly IPdfService _pdfService;
        private readonly ILanguageCodeService _languageCodeService;
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IConfigurationDialogService _configurationDialogService;
        private readonly IThemeService _themeService;
        private readonly ILogger<MainWindowViewModel> _logger;
        private CancellationTokenSource? _cancellationTokenSource;

        [ObservableProperty]
        private ObservableCollection<Theme> _availableThemes = new()
        {
            Theme.Light,
            Theme.Dark,
            Theme.System
        };

        [ObservableProperty]
        private Theme _selectedTheme;

        partial void OnSelectedThemeChanged(Theme value)
        {
            _themeService.SwitchTheme(value);
        }

        [ObservableProperty]
        private double _windowWidth = 1200;

        [ObservableProperty]
        private double _windowHeight = 800;

        [ObservableProperty]
        private double _windowLeft = double.NaN;

        [ObservableProperty]
        private double _windowTop = double.NaN;

        [ObservableProperty]
        private System.Windows.WindowState _windowState = System.Windows.WindowState.Normal;

        [ObservableProperty]
        private double _leftPanelWidth = 500;

        public new bool IsLoading
        {
            get => base.IsLoading;
            set
            {
                if (base.IsLoading != value)
                {
                    base.IsLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                    ScanDocumentsCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private bool _isScanning;

        public bool IsScanning
        {
            get => _isScanning;
            set
            {
                if (SetProperty(ref _isScanning, value))
                {
                    ScanDocumentsCommand.NotifyCanExecuteChanged();
                    CancelScanCommand.NotifyCanExecuteChanged();
                }
            }
        }

        [ObservableProperty]
        private ObservableCollection<string> _scannedPages = new();

        [ObservableProperty]
        private ScanSettingsViewModel _scanSettingsViewModel;

        [ObservableProperty]
        private OutputSettingsViewModel _outputSettingsViewModel;

        [ObservableProperty]
        private PreviewViewModel _previewViewModel;

        [ObservableProperty]
        private int _scanProgress;

        [ObservableProperty]
        private string _scanStatusMessage = string.Empty;

        public bool CanStartScan
        {
            get
            {
                var canStart = !IsLoading && !IsScanning && !string.IsNullOrEmpty(ScanSettingsViewModel.SelectedScanner) && OutputSettingsViewModel.IsOutputPathValid && OutputSettingsViewModel.IsBaseFileNameValid;
                _logger.LogTrace("CanStartScan evaluated: {CanStart}. IsLoading: {IsLoading}, IsScanning: {IsScanning}, SelectedScanner: {SelectedScanner}, OutputPathValid: {OutputPathValid}, BaseFileNameValid: {BaseFileNameValid}",
                    canStart, IsLoading, IsScanning, ScanSettingsViewModel.SelectedScanner, OutputSettingsViewModel.IsOutputPathValid, OutputSettingsViewModel.IsBaseFileNameValid);
                return canStart;
            }
        }

        public bool CanStopScan
        {
            get
            {
                var canStop = IsScanning;
                _logger.LogTrace("CanStopScan evaluated: {CanStop}. IsScanning: {IsScanning}", canStop, IsScanning);
                return canStop;
            }
        }

        public MainWindowViewModel(
            IDocumentScanner documentScanner,
            IConfigurationManager configurationManager,
            IScanConfigurationMapper scanConfigurationMapper,
            IPdfService pdfService,
            ILanguageCodeService languageCodeService,
            IErrorHandlingService errorHandlingService,
            IConfigurationDialogService configurationDialogService,
            IThemeService themeService,
            ILogger<MainWindowViewModel> logger)
        {
            _documentScanner = documentScanner;
            _configurationManager = configurationManager;
            _scanConfigurationMapper = scanConfigurationMapper;
            _pdfService = pdfService;
            _languageCodeService = languageCodeService;
            _errorHandlingService = errorHandlingService;
            _configurationDialogService = configurationDialogService;
            _themeService = themeService;
            _logger = logger;

            _scanSettingsViewModel = new ScanSettingsViewModel();
            _outputSettingsViewModel = new OutputSettingsViewModel();
            _previewViewModel = new PreviewViewModel();

            ScanDocumentsCommand.NotifyCanExecuteChanged();

            // Subscribe to PropertyChanged events of nested ViewModels
            _scanSettingsViewModel.PropertyChanged += (s, e) =>
            {
                _logger.LogTrace("ScanSettingsViewModel.PropertyChanged: {PropertyName}", e.PropertyName);
                if (e.PropertyName == nameof(ScanSettingsViewModel.SelectedScanner))
                {
                    ScanDocumentsCommand.NotifyCanExecuteChanged();
                }
            };

            _outputSettingsViewModel.PropertyChanged += (s, e) =>
            {
                _logger.LogTrace("OutputSettingsViewModel.PropertyChanged: {PropertyName}", e.PropertyName);
                if (e.PropertyName == nameof(OutputSettingsViewModel.OutputPath) ||
                    e.PropertyName == nameof(OutputSettingsViewModel.BaseFileName))
                {
                    ScanDocumentsCommand.NotifyCanExecuteChanged();
                }
            };

            // Set initial states
            IsLoading = false;
            IsScanning = false;
            _logger.LogTrace("MainWindowViewModel initialized. IsLoading: {IsLoading}, IsScanning: {IsScanning}", IsLoading, IsScanning);

            _documentScanner.PageScanned += OnPageScanned;

            _ = LoadConfigurationAsync();

            SelectedTheme = _themeService.CurrentTheme; // Initialize with current theme
        }

        private void OnPageScanned(object? sender, PageScannedEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ScannedPages.Add(e.ImageFilePath);
                PreviewViewModel.PreviewImage = new BitmapImage(new Uri(e.ImageFilePath));
                ScanStatusMessage = $"Scanning page {ScannedPages.Count}...";
                UpdateProgress();
            });
        }

        private void UpdateProgress()
        {
            var progress = ScannedPages.Count * 10;
            ScanProgress = Math.Min(progress, 100);
        }

        [RelayCommand]
        private async Task LoadConfigurationAsync()
        {
            try
            {
                SetBusyState(true, "Loading configuration...");
                _logger.LogTrace("LoadConfigurationAsync started. IsLoading: {IsLoading}", IsLoading);
                var configuration = await _configurationManager.LoadConfigurationAsync();

                ScanSettingsViewModel.SelectedScanner = configuration.SelectedScannerName;
                ScanSettingsViewModel.SelectedColorMode = configuration.ColorMode;
                ScanSettingsViewModel.SelectedPaperSource = configuration.ScannerPaperSource;
                ScanSettingsViewModel.SelectedResolution = $"{configuration.ScanResolutionDPI} DPI";
                ScanSettingsViewModel.CustomResolution = configuration.ScanResolutionDPI.ToString();
                ScanSettingsViewModel.AutoDeskew = configuration.AutoDeskew;
                ScanSettingsViewModel.ExcludeBlankPages = configuration.ExcludeBlankPages;
                ScanSettingsViewModel.OcrEnabled = configuration.OcrEnabled;
                ScanSettingsViewModel.TessdataFolder = configuration.TessdataFolder;
                ScanSettingsViewModel.SelectedLanguageCode = configuration.TessdataLanguageCode;

                OutputSettingsViewModel.OutputPath = configuration.OutputFolder;
                OutputSettingsViewModel.BaseFileName = configuration.OutputBaseFileName;

                ScanSettingsViewModel.SelectedDocumentOption = configuration.DocumentOptions;

                OutputSettingsViewModel.RecentFolders.Clear();
                foreach (var folder in configuration.RecentFolders)
                {
                    OutputSettingsViewModel.RecentFolders.Add(folder);
                }

                OutputSettingsViewModel.RecentFiles.Clear();
                foreach (var file in configuration.RecentFiles)
                {
                    OutputSettingsViewModel.RecentFiles.Add(file);
                }

                await LoadAvailableScannersAsync();
                // Set selected scanner after loading available scanners
                if (!string.IsNullOrEmpty(ScanSettingsViewModel.SelectedScanner) && ScanSettingsViewModel.AvailableScanners.Contains(ScanSettingsViewModel.SelectedScanner))
                {
                    // Keep the previously selected scanner if it's still available
                }
                else if (ScanSettingsViewModel.AvailableScanners.Any())
                {
                    // Otherwise, select the first available scanner
                    ScanSettingsViewModel.SelectedScanner = ScanSettingsViewModel.AvailableScanners.First();
                }
                else
                {
                    ScanSettingsViewModel.SelectedScanner = null; // No scanners available
                }

                LoadAvailableLanguages();
                await LoadWindowStateAsync();
                await _themeService.LoadThemeAsync();

                StatusMessage = "Configuration loaded successfully";
                _logger.LogTrace("Configuration loaded. SelectedScanner: {SelectedScanner}, OutputPath: {OutputPath}, BaseFileName: {BaseFileName}",
                    ScanSettingsViewModel.SelectedScanner, OutputSettingsViewModel.OutputPath, OutputSettingsViewModel.BaseFileName);
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleScanErrorAsync(ex);
                _logger.LogError(ex, "Failed to load configuration");
                StatusMessage = $"Startup error: {ex.Message}";
            }
            finally
            {
                ClearBusyState();
                _logger.LogTrace("LoadConfigurationAsync finished. IsLoading: {IsLoading}", IsLoading);
                ScanDocumentsCommand.NotifyCanExecuteChanged();
            }
        }

        [RelayCommand]
        private async Task SaveConfigurationAsync()
        {
            try
            {
                SetBusyState(true, "Saving configuration...");
                var scanConfiguration = new ScanConfiguration(
                    OutputSettingsViewModel.OutputPath,
                    OutputSettingsViewModel.BaseFileName,
                    ScanSettingsViewModel.SelectedColorMode,
                    ScanSettingsViewModel.SelectedDocumentOption,
                    ScanSettingsViewModel.AutoDeskew,
                    ScanSettingsViewModel.ExcludeBlankPages,
                    int.Parse(ScanSettingsViewModel.SelectedResolution.Replace(" DPI", "")),
                    ScanSettingsViewModel.SelectedPaperSource,
                    ScanSettingsViewModel.OcrEnabled,
                    ScanSettingsViewModel.TessdataFolder ?? string.Empty,
                    ScanSettingsViewModel.SelectedLanguageCode ?? string.Empty,
                    ScanSettingsViewModel.SelectedScanner ?? string.Empty
                ) { RecentFolders = OutputSettingsViewModel.RecentFolders.ToList(), RecentFiles = OutputSettingsViewModel.RecentFiles.ToList() };

                await _configurationManager.SaveConfigurationAsync(scanConfiguration);
                StatusMessage = "Configuration saved successfully";
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleScanErrorAsync(ex);
                _logger.LogError(ex, "Failed to save configuration");
            }
            finally
            {
                ClearBusyState();
            }
        }

        [RelayCommand]
        private async Task ScanDocumentsAsync()
        {
            if (IsScanning)
                return;

            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                IsScanning = true;
                SetBusyState(true, "Scanning documents...");
                ScanProgress = 0;
                ScanDocumentsCommand.NotifyCanExecuteChanged();
                _logger.LogTrace("ScanDocumentsAsync started. IsScanning: {IsScanning}, IsLoading: {IsLoading}", IsScanning, IsLoading);

                var scanConfiguration = new ScanConfiguration(
                    OutputSettingsViewModel.OutputPath,
                    OutputSettingsViewModel.BaseFileName,
                    ScanSettingsViewModel.SelectedColorMode,
                    ScanSettingsViewModel.SelectedDocumentOption,
                    ScanSettingsViewModel.AutoDeskew,
                    ScanSettingsViewModel.ExcludeBlankPages,
                    int.Parse(ScanSettingsViewModel.SelectedResolution.Replace(" DPI", "")),
                    ScanSettingsViewModel.SelectedPaperSource,
                    ScanSettingsViewModel.OcrEnabled,
                    ScanSettingsViewModel.TessdataFolder ?? string.Empty,
                    ScanSettingsViewModel.SelectedLanguageCode ?? string.Empty,
                    ScanSettingsViewModel.SelectedScanner ?? string.Empty
                ) { RecentFolders = OutputSettingsViewModel.RecentFolders.ToList() };

                await _documentScanner.ScanDocuments(scanConfiguration, _cancellationTokenSource.Token);

                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    StatusMessage = "Scan canceled.";
                }
                else
                {
                    StatusMessage = $"Scanning completed successfully. Scanned {ScannedPages.Count} page(s).";
                    ScanProgress = 100;
                    if (scanConfiguration.DocumentOptions == DocumentOptions.Combined)
                    {
                        OpenGeneratedPdf(scanConfiguration);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                StatusMessage = "Scan canceled.";
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleScanErrorAsync(ex);
                _logger.LogError(ex, "Scan operation failed");
                StatusMessage = $"Scan failed: {ex.Message}";
            }
            finally
            {
                IsScanning = false;
                ClearBusyState();
                _logger.LogTrace("ScanDocumentsAsync finished. IsScanning: {IsScanning}, IsLoading: {IsLoading}", IsScanning, IsLoading);
                ScanDocumentsCommand.NotifyCanExecuteChanged();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        [RelayCommand]
        private void CancelScan()
        {
            _cancellationTokenSource?.Cancel();
            IsScanning = false;
            ClearBusyState();
            _logger.LogTrace("CancelScan executed. IsScanning: {IsScanning}, IsLoading: {IsLoading}", IsScanning, IsLoading);
            ScanDocumentsCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private async Task LoadAvailableScannersAsync()
        {
            try
            {
                var scanners = await _documentScanner.GetScanDevicesAsync();
                ScanSettingsViewModel.AvailableScanners.Clear();
                foreach (var scanner in scanners)
                {
                    ScanSettingsViewModel.AvailableScanners.Add(scanner.Name);
                }
                StatusMessage = $"Found {scanners.Count} scanner(s)";

                if (ScanSettingsViewModel.AvailableScanners.Any())
                {
                    ScanSettingsViewModel.SelectedScannerStatus = "Ready";
                }
                else
                {
                    ScanSettingsViewModel.SelectedScannerStatus = "No scanners found";
                }
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleScanErrorAsync(ex);
                _logger.LogError(ex, "Failed to load scanners");
            }
        }

        private void LoadAvailableLanguages()
        {
            try
            {
                var languages = _languageCodeService.GetAvailableLanguageCodes(ScanSettingsViewModel.TessdataFolder ?? string.Empty, ScanSettingsViewModel.SelectedLanguageCode ?? string.Empty);
                ScanSettingsViewModel.AvailableLanguages.Clear();
                foreach (var language in languages)
                {
                    ScanSettingsViewModel.AvailableLanguages.Add(language);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load available languages");
            }
        }

        [RelayCommand]
        private async Task SelectOutputFolderAsync()
        {
            try
            {
                var selectedFolder = await _configurationDialogService.ShowFolderPickerAsync(
                    OutputSettingsViewModel.OutputPath,
                    "Select Output Folder");

                if (!string.IsNullOrEmpty(selectedFolder))
                {
                    OutputSettingsViewModel.OutputPath = selectedFolder;
                }
            }
            catch (Exception ex)
            {
                await _errorHandlingService.ShowExceptionAsync(ex, "selecting output folder");
            }
        }

        [RelayCommand]
        private async Task SelectTessdataFolderAsync()
        {
            try
            {
                var selectedFolder = await _configurationDialogService.ShowTessdataFolderPickerAsync(
                    ScanSettingsViewModel.TessdataFolder ?? string.Empty,
                    "Select Tessdata Folder");

                if (!string.IsNullOrEmpty(selectedFolder))
                {
                    ScanSettingsViewModel.TessdataFolder = selectedFolder;
                    LoadAvailableLanguages();
                }
            }
            catch (Exception ex)
            {
                await _errorHandlingService.ShowExceptionAsync(ex, "selecting tessdata folder");
            }
        }

        [RelayCommand]
        private async Task SelectLanguageAsync()
        {
            try
            {
                var languages = ScanSettingsViewModel.AvailableLanguages.ToList();
                var selectedLanguage = await _configurationDialogService.ShowLanguageSelectorAsync(
                    languages,
                    ScanSettingsViewModel.SelectedLanguageCode ?? string.Empty,
                    "Select OCR Language");

                if (!string.IsNullOrEmpty(selectedLanguage))
                {
                    ScanSettingsViewModel.SelectedLanguageCode = selectedLanguage;
                }
            }
            catch (Exception ex)
            {
                await _errorHandlingService.ShowExceptionAsync(ex, "selecting language");
            }
        }

        private void OpenGeneratedPdf(ScanConfiguration scanConfiguration)
        {
            try
            {
                var pdfFilePath = _pdfService.GetPdfFilePath(scanConfiguration);
                if (_pdfService.PdfFileExists(pdfFilePath))
                {
                    _pdfService.OpenPdfFile(pdfFilePath);
                    StatusMessage = "PDF opened successfully";
                    OutputSettingsViewModel.RecentFiles.Insert(0, pdfFilePath);
                    if (OutputSettingsViewModel.RecentFiles.Count > 5)
                    {
                        OutputSettingsViewModel.RecentFiles.RemoveAt(5);
                    }
                }
                else
                {
                    StatusMessage = "PDF file not found";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to open PDF file");
                StatusMessage = "Failed to open PDF file";
            }
        }

        public async Task OnWindowClosingAsync()
        {
            try
            {
                _documentScanner.PageScanned -= OnPageScanned;
                await SaveConfigurationAsync();
                await SaveWindowStateAsync();
                await _themeService.SaveThemeAsync();
                _logger.LogInformation("MainWindow closing - configuration and window state saved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during window closing");
                await _errorHandlingService.ShowErrorAsync("Error saving configuration", ex.Message);
            }
        }

        [RelayCommand]
        private async Task SaveWindowStateAsync()
        {
            try
            {
                var windowState = new WindowStateInfo
                {
                    Width = WindowWidth,
                    Height = WindowHeight,
                    Left = double.IsNaN(WindowLeft) ? 0 : WindowLeft,
                    Top = double.IsNaN(WindowTop) ? 0 : WindowTop,
                    State = (Core.Models.WindowState)WindowState,
                    LeftPanelWidth = LeftPanelWidth
                };

                await _configurationManager.SaveWindowStateAsync(windowState);
                _logger.LogTrace("Window state saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving window state");
            }
        }

        [RelayCommand]
        private async Task LoadWindowStateAsync()
        {
            try
            {
                var windowState = await _configurationManager.LoadWindowStateAsync();
                if (windowState != null)
                {
                    WindowWidth = windowState.Width;
                    WindowHeight = windowState.Height;
                    WindowLeft = windowState.Left;
                    WindowTop = windowState.Top;
                    WindowState = (System.Windows.WindowState)windowState.State;
                    LeftPanelWidth = windowState.LeftPanelWidth;

                    _logger.LogTrace("Window state loaded successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading window state");
            }
        }

        [RelayCommand]
        private void ClearPreview()
        {
            PreviewViewModel.PreviewImage = null;
            StatusMessage = "Preview cleared";
        }

        [RelayCommand]
        private async Task SavePreviewAsync()
        {
            if (PreviewViewModel.PreviewImage == null)
                return;

            try
            {
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PNG files (*.png)|*.png|JPEG files (*.jpg)|*.jpg|All files (*.*)|*.*",
                    DefaultExt = "png"
                };

                if (dialog.ShowDialog() == true)
                {
                    // Implementation would save the PreviewImage to the selected file
                    StatusMessage = "Preview saved successfully";
                }
            }
            catch (Exception ex)
            {
                await _errorHandlingService.ShowExceptionAsync(ex, "saving preview");
            }
        }

        [RelayCommand]
        private void ToggleTheme()
        {
            try
            {
                _themeService.ToggleTheme();
                var themeName = _themeService.CurrentTheme switch
                {
                    Theme.Light => "Light",
                    Theme.Dark => "Dark",
                    Theme.System => "System",
                    _ => "Unknown"
                };
                StatusMessage = $"Switched to {themeName} theme";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling theme");
                StatusMessage = "Error switching theme";
            }
        }

        [RelayCommand]
        private async Task ResetSettingsAsync()
        {
            try
            {
                SetBusyState(true, "Resetting settings...");
                var defaultConfiguration = new ScanConfiguration();
                await _configurationManager.SaveConfigurationAsync(defaultConfiguration);
                await LoadConfigurationAsync(); // Reload settings from the default configuration
                StatusMessage = "Settings reset to defaults.";
            }
            catch (Exception ex)
            {
                await _errorHandlingService.HandleScanErrorAsync(ex);
                _logger.LogError(ex, "Failed to reset settings");
                StatusMessage = $"Failed to reset settings: {ex.Message}";
            }
            finally
            {
                ClearBusyState();
            }
        }
    }
}