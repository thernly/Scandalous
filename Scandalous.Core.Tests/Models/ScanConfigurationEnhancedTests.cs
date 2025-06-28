using Scandalous.Core.Enums;
using Scandalous.Core.Models;
using System.Text.Json;
using Xunit;

namespace Scandalous.Core.Tests.Models
{
    public class ScanConfigurationEnhancedTests
    {
        #region Serialization Tests

        [Fact]
        public void ScanConfiguration_CanBeSerializedToJson()
        {
            // Arrange
            var config = new ScanConfiguration(
                outputFolder: "C:/test/output",
                baseFileName: "test-document",
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

            // Act
            var json = JsonSerializer.Serialize(config);

            // Assert
            Assert.NotNull(json);
            Assert.Contains("test-document", json);
            Assert.Contains("Color", json);
            Assert.Contains("DocumentOptions", json);
            Assert.Contains("600", json);
            Assert.Contains("ScannerPaperSource", json);
            Assert.Contains("deu", json);
        }

        [Fact]
        public void ScanConfiguration_CanBeDeserializedFromJson()
        {
            // Arrange
            var originalConfig = new ScanConfiguration(
                outputFolder: "C:/test/output",
                baseFileName: "test-document",
                colorMode: ScannerColorMode.BlackAndWhite,
                documentOptions: DocumentOptions.Combined,
                autoDeskew: true,
                excludeBlankPages: true,
                scanResolutionDpi: 300,
                scannerPaperSource: ScannerPaperSource.Flatbed,
                ocrEnabled: false,
                tessdataFolder: "C:/tessdata",
                languageCode: "eng"
            );
            var json = JsonSerializer.Serialize(originalConfig);

            // Act
            var deserializedConfig = JsonSerializer.Deserialize<ScanConfiguration>(json);

            // Assert
            Assert.NotNull(deserializedConfig);
            Assert.Equal(originalConfig.OutputFolder, deserializedConfig.OutputFolder);
            Assert.Equal(originalConfig.OutputBaseFileName, deserializedConfig.OutputBaseFileName);
            Assert.Equal(originalConfig.ColorMode, deserializedConfig.ColorMode);
            Assert.Equal(originalConfig.DocumentOptions, deserializedConfig.DocumentOptions);
            Assert.Equal(originalConfig.AutoDeskew, deserializedConfig.AutoDeskew);
            Assert.Equal(originalConfig.ExcludeBlankPages, deserializedConfig.ExcludeBlankPages);
            Assert.Equal(originalConfig.ScanResolutionDPI, deserializedConfig.ScanResolutionDPI);
            Assert.Equal(originalConfig.ScannerPaperSource, deserializedConfig.ScannerPaperSource);
            Assert.Equal(originalConfig.OcrEnabled, deserializedConfig.OcrEnabled);
            Assert.Equal(originalConfig.TessdataFolder, deserializedConfig.TessdataFolder);
            Assert.Equal(originalConfig.TessdataLanguageCode, deserializedConfig.TessdataLanguageCode);
        }

        [Fact]
        public void ScanConfiguration_SerializationPreservesAllProperties()
        {
            // Arrange
            var config = new ScanConfiguration(
                outputFolder: "C:/test/output",
                baseFileName: "test-document",
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

            // Act
            var json = JsonSerializer.Serialize(config);
            var deserializedConfig = JsonSerializer.Deserialize<ScanConfiguration>(json);

            // Assert
            Assert.NotNull(deserializedConfig);
            Assert.Equal(config.OutputFolder, deserializedConfig.OutputFolder);
            Assert.Equal(config.OutputBaseFileName, deserializedConfig.OutputBaseFileName);
            Assert.Equal(config.ColorMode, deserializedConfig.ColorMode);
            Assert.Equal(config.DocumentOptions, deserializedConfig.DocumentOptions);
            Assert.Equal(config.AutoDeskew, deserializedConfig.AutoDeskew);
            Assert.Equal(config.ExcludeBlankPages, deserializedConfig.ExcludeBlankPages);
            Assert.Equal(config.ScanResolutionDPI, deserializedConfig.ScanResolutionDPI);
            Assert.Equal(config.ScannerPaperSource, deserializedConfig.ScannerPaperSource);
            Assert.Equal(config.OcrEnabled, deserializedConfig.OcrEnabled);
            Assert.Equal(config.TessdataFolder, deserializedConfig.TessdataFolder);
            Assert.Equal(config.TessdataLanguageCode, deserializedConfig.TessdataLanguageCode);
        }

        [Fact]
        public void ScanConfiguration_WithUnicodeContent_SerializesCorrectly()
        {
            // Arrange
            var config = new ScanConfiguration(
                outputFolder: "C:/test/résumé-文档",
                baseFileName: "документ",
                colorMode: ScannerColorMode.Color,
                documentOptions: DocumentOptions.Combined,
                autoDeskew: true,
                excludeBlankPages: true,
                scanResolutionDpi: 300,
                scannerPaperSource: ScannerPaperSource.Auto,
                ocrEnabled: true,
                tessdataFolder: "C:/tessdata/中文",
                languageCode: "chi_sim"
            );

            // Act
            var json = JsonSerializer.Serialize(config);
            var deserializedConfig = JsonSerializer.Deserialize<ScanConfiguration>(json);

            // Assert
            Assert.NotNull(deserializedConfig);
            Assert.Equal(config.OutputFolder, deserializedConfig.OutputFolder);
            Assert.Equal(config.OutputBaseFileName, deserializedConfig.OutputBaseFileName);
            Assert.Equal(config.TessdataFolder, deserializedConfig.TessdataFolder);
            Assert.Equal(config.TessdataLanguageCode, deserializedConfig.TessdataLanguageCode);
        }

        #endregion

        #region Validation Integration Tests

        [Fact]
        public void ScanConfiguration_WithInvalidOutputFolder_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: "C:/invalid<folder>",
                baseFileName: "test"
            ));
            Assert.Contains("invalid character", exception.Message);
        }

        [Fact]
        public void ScanConfiguration_WithInvalidBaseFileName_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: "C:/test",
                baseFileName: "invalid<file>"
            ));
            Assert.Contains("invalid character", exception.Message);
        }

        [Fact]
        public void ScanConfiguration_WithValidPaths_DoesNotThrow()
        {
            // Arrange & Act & Assert
            var config = new ScanConfiguration(
                outputFolder: "C:/valid-folder",
                baseFileName: "valid-file"
            );

            Assert.Equal("C:/valid-folder", config.OutputFolder);
            Assert.Equal("valid-file", config.OutputBaseFileName);
        }

        [Fact]
        public void ScanConfiguration_WithReservedFolderName_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: "C:/CON",
                baseFileName: "test"
            ));
            Assert.Contains("reserved system name", exception.Message);
        }

        [Fact]
        public void ScanConfiguration_WithReservedFileName_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: "C:/test",
                baseFileName: "CON"
            ));
            Assert.Contains("reserved system name", exception.Message);
        }

        [Fact]
        public void ScanConfiguration_WithEmptyOutputFolder_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: "",
                baseFileName: "test"
            ));
            Assert.Contains("null, empty, or consist only of white-space characters", exception.Message);
        }

        [Fact]
        public void ScanConfiguration_WithEmptyBaseFileName_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: "C:/test",
                baseFileName: ""
            ));
            Assert.Contains("null, empty, or consist only of white-space characters", exception.Message);
        }

        [Fact]
        public void ScanConfiguration_WithWhitespaceOutputFolder_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: "   ",
                baseFileName: "test"
            ));
            Assert.Contains("null, empty, or consist only of white-space characters", exception.Message);
        }

        [Fact]
        public void ScanConfiguration_WithWhitespaceBaseFileName_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: "C:/test",
                baseFileName: "   "
            ));
            Assert.Contains("null, empty, or consist only of white-space characters", exception.Message);
        }

        [Fact]
        public void ScanConfiguration_WithInvalidTessdataFolder_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: "C:/test",
                baseFileName: "test",
                tessdataFolder: "C:/invalid<folder>"
            ));
            Assert.Contains("invalid character", exception.Message);
        }

        #endregion

        #region Edge Case Tests

        [Theory]
        [InlineData(72)]   // Minimum common DPI
        [InlineData(150)]  // Common DPI
        [InlineData(300)]  // Standard DPI
        [InlineData(600)]  // High DPI
        [InlineData(1200)] // Very high DPI
        [InlineData(2400)] // Ultra high DPI
        public void ScanConfiguration_WithVariousDpiValues_HandlesCorrectly(int dpi)
        {
            // Arrange & Act
            var config = new ScanConfiguration(
                outputFolder: "C:/test",
                baseFileName: "test",
                scanResolutionDpi: dpi
            );

            // Assert
            Assert.Equal(dpi, config.ScanResolutionDPI);
        }

        [Fact]
        public void ScanConfiguration_WithMaximumLengthStrings_HandlesCorrectly()
        {
            // Arrange
            var longString = new string('a', 200); // Reasonable length that should work

            // Act
            var config = new ScanConfiguration(
                outputFolder: longString,
                baseFileName: longString,
                tessdataFolder: longString,
                languageCode: longString
            );

            // Assert
            Assert.Equal(longString, config.OutputFolder);
            Assert.Equal(longString, config.OutputBaseFileName);
            Assert.Equal(longString, config.TessdataFolder);
            Assert.Equal(longString, config.TessdataLanguageCode);
        }

        [Fact]
        public void ScanConfiguration_WithUnicodeStrings_HandlesCorrectly()
        {
            // Arrange
            var unicodeOutputFolder = @"C:\temp\résumé-文档-документ";
            var unicodeBaseFileName = "документ-文档";
            var unicodeTessdataFolder = @"C:\tessdata\中文-русский";
            var unicodeLanguageCode = "chi_sim";

            // Act
            var config = new ScanConfiguration(
                outputFolder: unicodeOutputFolder,
                baseFileName: unicodeBaseFileName,
                tessdataFolder: unicodeTessdataFolder,
                languageCode: unicodeLanguageCode
            );

            // Assert
            Assert.Equal(unicodeOutputFolder, config.OutputFolder);
            Assert.Equal(unicodeBaseFileName, config.OutputBaseFileName);
            Assert.Equal(unicodeTessdataFolder, config.TessdataFolder);
            Assert.Equal(unicodeLanguageCode, config.TessdataLanguageCode);
        }

        [Fact]
        public void ScanConfiguration_WithSpecialCharacters_HandlesCorrectly()
        {
            // Arrange
            var specialOutputFolder = @"C:\temp\folder with spaces & symbols (1)";
            var specialBaseFileName = "file-name_with_underscores";
            var specialTessdataFolder = @"C:\tessdata\special chars";
            var specialLanguageCode = "eng-uk";

            // Act
            var config = new ScanConfiguration(
                outputFolder: specialOutputFolder,
                baseFileName: specialBaseFileName,
                tessdataFolder: specialTessdataFolder,
                languageCode: specialLanguageCode
            );

            // Assert
            Assert.Equal(specialOutputFolder, config.OutputFolder);
            Assert.Equal(specialBaseFileName, config.OutputBaseFileName);
            Assert.Equal(specialTessdataFolder, config.TessdataFolder);
            Assert.Equal(specialLanguageCode, config.TessdataLanguageCode);
        }

        [Fact]
        public void ScanConfiguration_WithNetworkPaths_HandlesCorrectly()
        {
            // Arrange
            var networkOutputFolder = @"\\server\share\scans";
            var networkTessdataFolder = @"\\server\share\tessdata";

            // Act
            var config = new ScanConfiguration(
                outputFolder: networkOutputFolder,
                baseFileName: "network-document",
                tessdataFolder: networkTessdataFolder
            );

            // Assert
            Assert.Equal(networkOutputFolder, config.OutputFolder);
            Assert.Equal(networkTessdataFolder, config.TessdataFolder);
        }

        [Fact]
        public void ScanConfiguration_WithAllEnumValues_HandlesCorrectly()
        {
            // Test all color modes
            foreach (var colorMode in Enum.GetValues<ScannerColorMode>())
            {
                var config = new ScanConfiguration(
                    outputFolder: "C:/test",
                    baseFileName: "test",
                    colorMode: colorMode
                );
                Assert.Equal(colorMode, config.ColorMode);
            }

            // Test all document options
            foreach (var documentOption in Enum.GetValues<DocumentOptions>())
            {
                var config = new ScanConfiguration(
                    outputFolder: "C:/test",
                    baseFileName: "test",
                    documentOptions: documentOption
                );
                Assert.Equal(documentOption, config.DocumentOptions);
            }

            // Test all paper sources
            foreach (var paperSource in Enum.GetValues<ScannerPaperSource>())
            {
                var config = new ScanConfiguration(
                    outputFolder: "C:/test",
                    baseFileName: "test",
                    scannerPaperSource: paperSource
                );
                Assert.Equal(paperSource, config.ScannerPaperSource);
            }
        }

        #endregion

        #region Equality Tests

        [Fact]
        public void ScanConfiguration_EqualityComparison_WorksCorrectly()
        {
            // Arrange
            var config1 = new ScanConfiguration(
                outputFolder: "C:/test",
                baseFileName: "test",
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

            var config2 = new ScanConfiguration(
                outputFolder: "C:/test",
                baseFileName: "test",
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

            var config3 = new ScanConfiguration(
                outputFolder: "C:/different",
                baseFileName: "different",
                colorMode: ScannerColorMode.BlackAndWhite,
                documentOptions: DocumentOptions.Combined,
                autoDeskew: true,
                excludeBlankPages: true,
                scanResolutionDpi: 300,
                scannerPaperSource: ScannerPaperSource.Flatbed,
                ocrEnabled: false,
                tessdataFolder: "C:/different",
                languageCode: "eng"
            );

            // Act & Assert
            Assert.Equal(config1.OutputFolder, config2.OutputFolder);
            Assert.Equal(config1.OutputBaseFileName, config2.OutputBaseFileName);
            Assert.Equal(config1.ColorMode, config2.ColorMode);
            Assert.Equal(config1.DocumentOptions, config2.DocumentOptions);
            Assert.Equal(config1.AutoDeskew, config2.AutoDeskew);
            Assert.Equal(config1.ExcludeBlankPages, config2.ExcludeBlankPages);
            Assert.Equal(config1.ScanResolutionDPI, config2.ScanResolutionDPI);
            Assert.Equal(config1.ScannerPaperSource, config2.ScannerPaperSource);
            Assert.Equal(config1.OcrEnabled, config2.OcrEnabled);
            Assert.Equal(config1.TessdataFolder, config2.TessdataFolder);
            Assert.Equal(config1.TessdataLanguageCode, config2.TessdataLanguageCode);

            // Different configurations should have different values
            Assert.NotEqual(config1.OutputFolder, config3.OutputFolder);
            Assert.NotEqual(config1.OutputBaseFileName, config3.OutputBaseFileName);
            Assert.NotEqual(config1.ColorMode, config3.ColorMode);
            Assert.NotEqual(config1.DocumentOptions, config3.DocumentOptions);
            Assert.NotEqual(config1.AutoDeskew, config3.AutoDeskew);
            Assert.NotEqual(config1.ExcludeBlankPages, config3.ExcludeBlankPages);
            Assert.NotEqual(config1.ScanResolutionDPI, config3.ScanResolutionDPI);
            Assert.NotEqual(config1.ScannerPaperSource, config3.ScannerPaperSource);
            Assert.NotEqual(config1.OcrEnabled, config3.OcrEnabled);
            Assert.NotEqual(config1.TessdataFolder, config3.TessdataFolder);
            Assert.NotEqual(config1.TessdataLanguageCode, config3.TessdataLanguageCode);
        }

        [Fact]
        public void ScanConfiguration_PropertiesAreMutable()
        {
            // Arrange
            var config = new ScanConfiguration(
                outputFolder: "C:/test",
                baseFileName: "test"
            );

            // Act
            config.OutputFolder = "C:/changed";
            config.OutputBaseFileName = "changed";
            config.ColorMode = ScannerColorMode.Color;
            config.DocumentOptions = DocumentOptions.Individual;
            config.AutoDeskew = false;
            config.ExcludeBlankPages = false;
            config.ScanResolutionDPI = 600;
            config.ScannerPaperSource = ScannerPaperSource.FeederDuplex;
            config.OcrEnabled = true;
            config.TessdataFolder = "C:/changed";
            config.TessdataLanguageCode = "deu";

            // Assert
            Assert.Equal("C:/changed", config.OutputFolder);
            Assert.Equal("changed", config.OutputBaseFileName);
            Assert.Equal(ScannerColorMode.Color, config.ColorMode);
            Assert.Equal(DocumentOptions.Individual, config.DocumentOptions);
            Assert.False(config.AutoDeskew);
            Assert.False(config.ExcludeBlankPages);
            Assert.Equal(600, config.ScanResolutionDPI);
            Assert.Equal(ScannerPaperSource.FeederDuplex, config.ScannerPaperSource);
            Assert.True(config.OcrEnabled);
            Assert.Equal("C:/changed", config.TessdataFolder);
            Assert.Equal("deu", config.TessdataLanguageCode);
        }

        #endregion
    }
} 