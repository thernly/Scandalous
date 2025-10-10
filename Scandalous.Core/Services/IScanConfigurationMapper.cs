using Scandalous.Core.Models;

namespace Scandalous.Core.Services
{
    public interface IScanConfigurationMapper
    {
        ScanConfiguration BuildConfigurationFromUIState(UIState uiState);
        UIState BuildUIStateFromConfiguration(ScanConfiguration configuration);
    }

    public class UIState
    {
        public string OutputFolder { get; set; } = string.Empty;
        public string BaseFileName { get; set; } = string.Empty;
        public bool AutoDeskew { get; set; } = true;
        public bool ExcludeBlankPages { get; set; } = true;
        public bool DocumentCombined { get; set; } = true;
        public bool DocumentIndividual { get; set; } = false;
        public bool ColorModeGrayscale { get; set; } = true;
        public bool ColorModeBlackWhite { get; set; } = false;
        public bool ColorModeColor { get; set; } = false;
        public bool FeederDuplex { get; set; } = false;
        public bool FeederSimplex { get; set; } = false;
        public bool Flatbed { get; set; } = false;
        public int Dpi { get; set; } = 300;
        public bool OcrEnabled { get; set; } = false;
        public string TessdataFolder { get; set; } = string.Empty;
        public string SelectedLanguageCode { get; set; } = "eng";
        public string SelectedScannerName { get; set; } = string.Empty;
    }
} 