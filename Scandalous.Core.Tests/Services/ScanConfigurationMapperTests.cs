using Scandalous.Core.Enums;
using Scandalous.Core.Models;
using Scandalous.Core.Services;

namespace Scandalous.Core.Tests.Services
{
    public class ScanConfigurationMapperTests
    {
        [Fact]
        public void BuildConfigurationFromUIState_MapsPaperSize()
        {
            var mapper = new ScanConfigurationMapper();
            var uiState = new UIState
            {
                OutputFolder = "C:/output",
                BaseFileName = "doc",
                ColorModeGrayscale = true,
                DocumentCombined = true,
                FeederDuplex = true,
                TessdataFolder = "C:/tessdata",
                SelectedLanguageCode = "eng",
                PaperSize = ScannerPaperSize.A4
            };

            var configuration = mapper.BuildConfigurationFromUIState(uiState);

            Assert.Equal(ScannerPaperSize.A4, configuration.PaperSize);
        }

        [Fact]
        public void BuildUIStateFromConfiguration_MapsPaperSize()
        {
            var mapper = new ScanConfigurationMapper();
            var configuration = new ScanConfiguration
            {
                OutputFolder = "C:/output",
                OutputBaseFileName = "doc",
                PaperSize = ScannerPaperSize.Letter
            };

            var uiState = mapper.BuildUIStateFromConfiguration(configuration);

            Assert.Equal(ScannerPaperSize.Letter, uiState.PaperSize);
        }
    }
}