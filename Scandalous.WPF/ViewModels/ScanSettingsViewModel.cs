using CommunityToolkit.Mvvm.ComponentModel;
using Scandalous.Core.Enums;
using Scandalous.Core.Validation;
using System.Collections.ObjectModel;

namespace Scandalous.WPF.ViewModels
{
    public partial class ScanSettingsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string? _selectedScanner;

        [ObservableProperty]
        private string _selectedScannerStatus = string.Empty;

        [ObservableProperty]
        private ScannerColorMode _selectedColorMode;

        [ObservableProperty]
        private ScannerPaperSource _selectedPaperSource;

        private string _selectedResolution = "300 DPI";

        public string SelectedResolution
        {
            get => _selectedResolution;
            set
            {
                if (SetProperty(ref _selectedResolution, value))
                {
                    // Extract the integer part from the string (e.g., "300 DPI" -> 300)
                    if (int.TryParse(value.Replace(" DPI", ""), out int dpi))
                    {
                        CustomResolution = dpi.ToString();
                    }
                }
            }
        }

        [ObservableProperty]
        private string _customResolution = string.Empty;

        [ObservableProperty]
        private string _customResolutionError = string.Empty;

        partial void OnCustomResolutionChanged(string value)
        {
            if (int.TryParse(value, out int dpi) && dpi > 0)
            {
                CustomResolutionError = string.Empty;
            }
            else
            {
                CustomResolutionError = "Please enter a valid DPI (e.g., 300).";
            }
        }

        [ObservableProperty]
        private ObservableCollection<string> _resolutions = new()
        {
            "75 DPI",
            "150 DPI",
            "200 DPI",
            "300 DPI",
            "600 DPI"
        };

        [ObservableProperty]
        private DocumentOptions _selectedDocumentOption;

        [ObservableProperty]
        private ObservableCollection<DocumentOptions> _documentOptions = new();

        [ObservableProperty]
        private bool _autoDeskew;

        [ObservableProperty]
        private bool _excludeBlankPages;

        [ObservableProperty]
        private string? _selectedLanguageCode;

        [ObservableProperty]
        private ObservableCollection<string> _availableScanners = new();

        [ObservableProperty]
        private ObservableCollection<string> _availableLanguages = new();

        public ScanSettingsViewModel()
        {
        }

        [ObservableProperty]
        private string _selectedColorModeDescription = string.Empty;

        partial void OnSelectedColorModeChanged(ScannerColorMode value)
        {
            SelectedColorModeDescription = value switch
            {
                ScannerColorMode.Color => "Best for photos and documents with color. Produces larger file sizes.",
                ScannerColorMode.Grayscale => "Ideal for documents with text and images where color is not essential. Balances file size and detail.",
                ScannerColorMode.BlackAndWhite => "Suitable for text-only documents. Produces the smallest file sizes and clearest text.",
                _ => string.Empty
            };
        }

        [ObservableProperty]
        private bool _ocrEnabled;

        [ObservableProperty]
        private string? _tessdataFolder;

        [ObservableProperty]
        private string _tessdataFolderError = string.Empty;

        partial void OnTessdataFolderChanged(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                TessdataFolderError = string.Empty;
            }
            else
            {
                var (isValid, error) = FolderValidator.IsValid(value);
                TessdataFolderError = isValid ? string.Empty : error;
            }
        }
    }
}
