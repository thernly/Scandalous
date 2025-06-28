using Scandalous.Core.Enums;
using Scandalous.Core.Models;
using Xunit;

namespace Scandalous.Core.Tests.Models
{
    public class ScanConfigurationTests
    {
        [Fact]
        public void DefaultConstructor_SetsExpectedDefaults()
        {
            var config = new ScanConfiguration();
            Assert.Equal(string.Empty, config.OutputFolder);
            Assert.Equal(string.Empty, config.OutputBaseFileName);
            Assert.Equal(ScannerColorMode.Grayscale, config.ColorMode);
            Assert.Equal(DocumentOptions.Combined, config.DocumentOptions);
            Assert.True(config.AutoDeskew);
            Assert.True(config.ExcludeBlankPages);
            Assert.Equal(300, config.ScanResolutionDPI);
            Assert.Equal(ScannerPaperSource.Auto, config.ScannerPaperSource);
            Assert.False(config.OcrEnabled);
            Assert.Equal(string.Empty, config.TessdataFolder);
            Assert.Equal("eng", config.TessdataLanguageCode);
        }

        [Fact]
        public void ParameterizedConstructor_SetsPropertiesCorrectly()
        {
            var config = new ScanConfiguration(
                outputFolder: "C:/output",
                baseFileName: "doc",
                colorMode: ScannerColorMode.Color,
                documentOptions: DocumentOptions.Individual,
                autoDeskew: false,
                excludeBlankPages: false,
                scanResolutionDpi: 600,
                scannerPaperSource: ScannerPaperSource.FeederDuplex,
                ocrEnabled: true,
                tessdataFolder: "C:/tessdata",
                languageCode: "deu"
            );
            Assert.Equal("C:/output", config.OutputFolder);
            Assert.Equal("doc", config.OutputBaseFileName);
            Assert.Equal(ScannerColorMode.Color, config.ColorMode);
            Assert.Equal(DocumentOptions.Individual, config.DocumentOptions);
            Assert.False(config.AutoDeskew);
            Assert.False(config.ExcludeBlankPages);
            Assert.Equal(600, config.ScanResolutionDPI);
            Assert.Equal(ScannerPaperSource.FeederDuplex, config.ScannerPaperSource);
            Assert.True(config.OcrEnabled);
            Assert.Equal("C:/tessdata", config.TessdataFolder);
            Assert.Equal("deu", config.TessdataLanguageCode);
        }
    }
} 