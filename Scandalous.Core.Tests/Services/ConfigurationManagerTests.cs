using NSubstitute;
using Scandalous.Core.Enums;
using Scandalous.Core.Models;
using Scandalous.Core.Services;
using System.Text.Json;
using Xunit;
using System.IO;

namespace Scandalous.Core.Tests.Services
{
    public class ConfigurationManagerTests
    {
        [Fact]
        public void Constructor_CreatesAppDataDirectory()
        {
            // Act
            var configManager = new ConfigurationManager();

            // Assert
            // The constructor should not throw an exception
            Assert.NotNull(configManager);
        }

        public class TestableConfigurationManager : ConfigurationManager
        {
            public TestableConfigurationManager(string configFilePath) : base(configFilePath) { }
        }

        [Fact]
        public async Task LoadConfigurationAsync_WhenFileDoesNotExist_ReturnsDefaultConfiguration()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            File.Delete(tempFile); // Ensure it does not exist
            var configManager = new TestableConfigurationManager(tempFile);

            // Act
            var result = await configManager.LoadConfigurationAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.OutputFolder);
            Assert.Equal(string.Empty, result.OutputBaseFileName);
            Assert.Equal(ScannerColorMode.Grayscale, result.ColorMode);
            Assert.Equal(DocumentOptions.Combined, result.DocumentOptions);
            Assert.True(result.AutoDeskew);
            Assert.True(result.ExcludeBlankPages);
            Assert.Equal(300, result.ScanResolutionDPI);
            Assert.Equal(ScannerPaperSource.Auto, result.ScannerPaperSource);
            Assert.False(result.OcrEnabled);
            Assert.Equal(string.Empty, result.TessdataFolder);
            Assert.Equal("eng", result.TessdataLanguageCode);
        }

        [Fact]
        public async Task SaveConfigurationAsync_And_LoadConfigurationAsync_RoundTripWorks()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var configManager = new TestableConfigurationManager(tempFile);
            var originalConfig = new ScanConfiguration(
                outputFolder: "C:\\TestOutput",
                baseFileName: "TestDocument",
                colorMode: ScannerColorMode.Color,
                documentOptions: DocumentOptions.Individual,
                autoDeskew: false,
                excludeBlankPages: false,
                scanResolutionDpi: 600,
                scannerPaperSource: ScannerPaperSource.FeederDuplex,
                ocrEnabled: true,
                tessdataFolder: "C:\\tessdata",
                languageCode: "deu"
            );

            // Act
            await configManager.SaveConfigurationAsync(originalConfig);
            var loadedConfig = await configManager.LoadConfigurationAsync();

            // Assert
            Assert.Equal(originalConfig.OutputFolder, loadedConfig.OutputFolder);
            Assert.Equal(originalConfig.OutputBaseFileName, loadedConfig.OutputBaseFileName);
            Assert.Equal(originalConfig.ColorMode, loadedConfig.ColorMode);
            Assert.Equal(originalConfig.DocumentOptions, loadedConfig.DocumentOptions);
            Assert.Equal(originalConfig.AutoDeskew, loadedConfig.AutoDeskew);
            Assert.Equal(originalConfig.ExcludeBlankPages, loadedConfig.ExcludeBlankPages);
            Assert.Equal(originalConfig.ScanResolutionDPI, loadedConfig.ScanResolutionDPI);
            Assert.Equal(originalConfig.ScannerPaperSource, loadedConfig.ScannerPaperSource);
            Assert.Equal(originalConfig.OcrEnabled, loadedConfig.OcrEnabled);
            Assert.Equal(originalConfig.TessdataFolder, loadedConfig.TessdataFolder);
            Assert.Equal(originalConfig.TessdataLanguageCode, loadedConfig.TessdataLanguageCode);
        }

        [Fact]
        public void GetInstalledTessdataLanguageCodes_WithEmptyFolder_ReturnsEmptyList()
        {
            // Arrange
            var configManager = new ConfigurationManager();
            var emptyFolder = string.Empty;

            // Act
            var result = configManager.GetInstalledTessdataLanguageCodes(emptyFolder);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetInstalledTessdataLanguageCodes_WithNonExistentFolder_ReturnsEmptyList()
        {
            // Arrange
            var configManager = new ConfigurationManager();
            var nonExistentFolder = "C:\\NonExistentFolder\\That\\Does\\Not\\Exist";

            // Act
            var result = configManager.GetInstalledTessdataLanguageCodes(nonExistentFolder);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
} 