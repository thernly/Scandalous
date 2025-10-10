using Scandalous.Core.Enums;
using Scandalous.Core.Models;

namespace Scandalous.Core.Services
{
    public class ScanConfigurationMapper : IScanConfigurationMapper
    {
        public ScanConfiguration BuildConfigurationFromUIState(UIState uiState)
        {
            var documentOptions = uiState.DocumentCombined ? DocumentOptions.Combined : DocumentOptions.Individual;
            var colorMode = GetScannerColorMode(uiState);
            var scannerPaperSource = GetScannerPaperSource(uiState);
            
            return new ScanConfiguration(
                uiState.OutputFolder,
                uiState.BaseFileName,
                colorMode,
                documentOptions,
                uiState.AutoDeskew,
                uiState.ExcludeBlankPages,
                uiState.Dpi,
                scannerPaperSource,
                uiState.OcrEnabled,
                uiState.TessdataFolder,
                uiState.SelectedLanguageCode,
                uiState.SelectedScannerName
            );
        }

        public UIState BuildUIStateFromConfiguration(ScanConfiguration configuration)
        {
            return new UIState
            {
                OutputFolder = configuration.OutputFolder,
                BaseFileName = configuration.OutputBaseFileName,
                AutoDeskew = configuration.AutoDeskew,
                ExcludeBlankPages = configuration.ExcludeBlankPages,
                DocumentCombined = configuration.DocumentOptions == DocumentOptions.Combined,
                DocumentIndividual = configuration.DocumentOptions == DocumentOptions.Individual,
                ColorModeGrayscale = configuration.ColorMode == ScannerColorMode.Grayscale,
                ColorModeBlackWhite = configuration.ColorMode == ScannerColorMode.BlackAndWhite,
                ColorModeColor = configuration.ColorMode == ScannerColorMode.Color,
                FeederDuplex = configuration.ScannerPaperSource == ScannerPaperSource.FeederDuplex,
                FeederSimplex = configuration.ScannerPaperSource == ScannerPaperSource.FeederSimplex,
                Flatbed = configuration.ScannerPaperSource == ScannerPaperSource.Flatbed,
                Dpi = configuration.ScanResolutionDPI,
                OcrEnabled = configuration.OcrEnabled,
                TessdataFolder = configuration.TessdataFolder,
                SelectedLanguageCode = configuration.TessdataLanguageCode,
                SelectedScannerName = configuration.SelectedScannerName
            };
        }

        private ScannerColorMode GetScannerColorMode(UIState uiState)
        {
            if (uiState.ColorModeGrayscale) return ScannerColorMode.Grayscale;
            if (uiState.ColorModeBlackWhite) return ScannerColorMode.BlackAndWhite;
            if (uiState.ColorModeColor) return ScannerColorMode.Color;
            return ScannerColorMode.Grayscale;
        }

        private ScannerPaperSource GetScannerPaperSource(UIState uiState)
        {
            if (uiState.FeederDuplex) return ScannerPaperSource.FeederDuplex;
            if (uiState.FeederSimplex) return ScannerPaperSource.FeederSimplex;
            if (uiState.Flatbed) return ScannerPaperSource.Flatbed;
            return ScannerPaperSource.Auto;
        }
    }
} 