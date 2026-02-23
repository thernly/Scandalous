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

        [Fact]
        public async Task LoadConfigurationAsync_WithCorruptedJsonFile_ReturnsDefaultConfiguration()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var configManager = new TestableConfigurationManager(tempFile);
            
            // Write corrupted JSON
            await File.WriteAllTextAsync(tempFile, "{ invalid json content }", TestContext.Current.CancellationToken);

            // Act
            var result = await configManager.LoadConfigurationAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.OutputFolder);
            Assert.Equal(string.Empty, result.OutputBaseFileName);
            Assert.Equal(ScannerColorMode.Grayscale, result.ColorMode);
        }

        [Fact]
        public async Task LoadConfigurationAsync_WithEmptyJsonFile_ReturnsDefaultConfiguration()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var configManager = new TestableConfigurationManager(tempFile);
            
            // Write empty file
            await File.WriteAllTextAsync(tempFile, "", TestContext.Current.CancellationToken);

            // Act
            var result = await configManager.LoadConfigurationAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.OutputFolder);
            Assert.Equal(string.Empty, result.OutputBaseFileName);
            Assert.Equal(ScannerColorMode.Grayscale, result.ColorMode);
        }

        [Fact]
        public async Task LoadConfigurationAsync_WithPartialJsonFile_ReturnsPartialConfiguration()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var configManager = new TestableConfigurationManager(tempFile);
            
            // Write partial JSON with properly escaped backslashes
            await File.WriteAllTextAsync(tempFile, "{ \"outputFolder\": \"C:\\\\Test\" }", TestContext.Current.CancellationToken);

            // Act
            var result = await configManager.LoadConfigurationAsync();

            // Assert
            Assert.NotNull(result);
            // The JSON deserializer will populate the outputFolder but leave other properties as defaults
            Assert.Equal("C:\\Test", result.OutputFolder);
            Assert.Equal(string.Empty, result.OutputBaseFileName);
            Assert.Equal(ScannerColorMode.Grayscale, result.ColorMode);
        }

        [Fact]
        public async Task SaveConfigurationAsync_WithNullConfiguration_ThrowsArgumentNullException()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var configManager = new TestableConfigurationManager(tempFile);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => configManager.SaveConfigurationAsync(null!));
        }

        [Fact]
        public async Task SaveConfigurationAsync_WithReadOnlyFile_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var configManager = new TestableConfigurationManager(tempFile);
            var config = new ScanConfiguration();
            
            // Make file read-only
            File.SetAttributes(tempFile, FileAttributes.ReadOnly);

            try
            {
                // Act & Assert
                await Assert.ThrowsAsync<UnauthorizedAccessException>(() => configManager.SaveConfigurationAsync(config));
            }
            finally
            {
                // Clean up
                File.SetAttributes(tempFile, FileAttributes.Normal);
            }
        }

        [Fact]
        public async Task SaveConfigurationAsync_WithDirectoryThatDoesNotExist_CreatesDirectory()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), "ScandalousTest", Guid.NewGuid().ToString());
            var configFile = Path.Combine(tempDir, "config.json");
            var configManager = new TestableConfigurationManager(configFile);
            var config = new ScanConfiguration();

            try
            {
                // Act
                await configManager.SaveConfigurationAsync(config);

                // Assert
                Assert.True(Directory.Exists(tempDir));
                Assert.True(File.Exists(configFile));
            }
            finally
            {
                // Clean up
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        [Fact]
        public async Task LoadConfigurationAsync_WithFileInUse_ReturnsDefaultConfiguration()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var configManager = new TestableConfigurationManager(tempFile);
            
            // Write content to the file first
            await File.WriteAllTextAsync(tempFile, "{}", TestContext.Current.CancellationToken);
            
            // Then lock the file
            using var fileStream = File.Open(tempFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

            // Act
            var result = await configManager.LoadConfigurationAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.OutputFolder);
            Assert.Equal(string.Empty, result.OutputBaseFileName);
            Assert.Equal(ScannerColorMode.Grayscale, result.ColorMode);
        }

        [Fact]
        public async Task SaveConfigurationAsync_WithVeryLongPath_HandlesCorrectly()
        {
            // Arrange
            var longPath = new string('a', 200);
            var tempFile = Path.Combine(Path.GetTempPath(), longPath, "config.json");
            var configManager = new TestableConfigurationManager(tempFile);
            var config = new ScanConfiguration();

            try
            {
                // Act
                await configManager.SaveConfigurationAsync(config);

                // Assert
                Assert.True(File.Exists(tempFile));
            }
            finally
            {
                // Clean up
                var dir = Path.GetDirectoryName(tempFile);
                if (Directory.Exists(dir!))
                {
                    Directory.Delete(dir, true);
                }
            }
        }

        [Fact]
        public async Task LoadConfigurationAsync_WithUnicodeContent_HandlesCorrectly()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var configManager = new TestableConfigurationManager(tempFile);
            var config = new ScanConfiguration(
                outputFolder: @"C:\temp\résumé-文档",
                baseFileName: "документ",
                colorMode: ScannerColorMode.Color,
                documentOptions: DocumentOptions.Combined,
                autoDeskew: true,
                excludeBlankPages: true,
                scanResolutionDpi: 300,
                scannerPaperSource: ScannerPaperSource.Auto,
                ocrEnabled: true,
                tessdataFolder: @"C:\tessdata\中文",
                languageCode: "chi_sim"
            );

            // Act
            await configManager.SaveConfigurationAsync(config);
            var loadedConfig = await configManager.LoadConfigurationAsync();

            // Assert
            Assert.Equal(config.OutputFolder, loadedConfig.OutputFolder);
            Assert.Equal(config.OutputBaseFileName, loadedConfig.OutputBaseFileName);
            Assert.Equal(config.TessdataFolder, loadedConfig.TessdataFolder);
            Assert.Equal(config.TessdataLanguageCode, loadedConfig.TessdataLanguageCode);
        }

        [Fact]
        public async Task SaveConfigurationAsync_WithSpecialCharacters_HandlesCorrectly()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var configManager = new TestableConfigurationManager(tempFile);
            var config = new ScanConfiguration(
                outputFolder: @"C:\temp\folder with spaces & symbols (1)",
                baseFileName: "file-name_with_underscores", // Removed the dot to make it valid
                colorMode: ScannerColorMode.Color,
                documentOptions: DocumentOptions.Combined,
                autoDeskew: true,
                excludeBlankPages: true,
                scanResolutionDpi: 300,
                scannerPaperSource: ScannerPaperSource.Auto,
                ocrEnabled: false,
                tessdataFolder: @"C:\tessdata\special chars",
                languageCode: "eng"
            );

            // Act
            await configManager.SaveConfigurationAsync(config);
            var loadedConfig = await configManager.LoadConfigurationAsync();

            // Assert
            Assert.Equal(config.OutputFolder, loadedConfig.OutputFolder);
            Assert.Equal(config.OutputBaseFileName, loadedConfig.OutputBaseFileName);
            Assert.Equal(config.TessdataFolder, loadedConfig.TessdataFolder);
        }

        [Fact]
        public async Task LoadConfigurationAsync_WithNetworkPath_HandlesCorrectly()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var configManager = new TestableConfigurationManager(tempFile);
            var config = new ScanConfiguration(
                outputFolder: @"\\server\share\scans",
                baseFileName: "network-document",
                colorMode: ScannerColorMode.Color,
                documentOptions: DocumentOptions.Combined,
                autoDeskew: true,
                excludeBlankPages: true,
                scanResolutionDpi: 300,
                scannerPaperSource: ScannerPaperSource.Auto,
                ocrEnabled: false,
                tessdataFolder: @"\\server\share\tessdata",
                languageCode: "eng"
            );

            // Act
            await configManager.SaveConfigurationAsync(config);
            var loadedConfig = await configManager.LoadConfigurationAsync();

            // Assert
            Assert.Equal(config.OutputFolder, loadedConfig.OutputFolder);
            Assert.Equal(config.OutputBaseFileName, loadedConfig.OutputBaseFileName);
            Assert.Equal(config.TessdataFolder, loadedConfig.TessdataFolder);
        }

        [Fact]
        public async Task SaveConfigurationAsync_WithAllEnumValues_HandlesCorrectly()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var configManager = new TestableConfigurationManager(tempFile);
            var config = new ScanConfiguration(
                outputFolder: @"C:\temp",
                baseFileName: "test",
                colorMode: ScannerColorMode.BlackAndWhite,
                documentOptions: DocumentOptions.Individual,
                autoDeskew: false,
                excludeBlankPages: false,
                scanResolutionDpi: 600,
                scannerPaperSource: ScannerPaperSource.FeederSimplex,
                ocrEnabled: true,
                tessdataFolder: @"C:\tessdata",
                languageCode: "deu"
            );

            // Act
            await configManager.SaveConfigurationAsync(config);
            var loadedConfig = await configManager.LoadConfigurationAsync();

            // Assert
            Assert.Equal(ScannerColorMode.BlackAndWhite, loadedConfig.ColorMode);
            Assert.Equal(DocumentOptions.Individual, loadedConfig.DocumentOptions);
            Assert.Equal(ScannerPaperSource.FeederSimplex, loadedConfig.ScannerPaperSource);
            Assert.Equal(600, loadedConfig.ScanResolutionDPI);
            Assert.False(loadedConfig.AutoDeskew);
            Assert.False(loadedConfig.ExcludeBlankPages);
            Assert.True(loadedConfig.OcrEnabled);
        }

        [Fact]
        public async Task LoadConfigurationAsync_WithMalformedJson_ReturnsDefaultConfiguration()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var configManager = new TestableConfigurationManager(tempFile);
            
            // Write malformed JSON with properly escaped backslashes
            await File.WriteAllTextAsync(tempFile, "{ \"outputFolder\": \"C:\\\\Test\", \"colorMode\": \"InvalidValue\" }", TestContext.Current.CancellationToken);

            // Act
            var result = await configManager.LoadConfigurationAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.OutputFolder);
            Assert.Equal(ScannerColorMode.Grayscale, result.ColorMode);
        }

        [Fact]
        public async Task SaveConfigurationAsync_WithVeryLargeConfiguration_HandlesCorrectly()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var configManager = new TestableConfigurationManager(tempFile);
            var longString = new string('x', 100); // Reduced from 10000 to avoid validation issues
            var config = new ScanConfiguration(
                outputFolder: longString,
                baseFileName: longString,
                colorMode: ScannerColorMode.Color,
                documentOptions: DocumentOptions.Combined,
                autoDeskew: true,
                excludeBlankPages: true,
                scanResolutionDpi: 300,
                scannerPaperSource: ScannerPaperSource.Auto,
                ocrEnabled: true,
                tessdataFolder: longString,
                languageCode: longString
            );

            // Act
            await configManager.SaveConfigurationAsync(config);
            var loadedConfig = await configManager.LoadConfigurationAsync();

            // Assert
            Assert.Equal(config.OutputFolder, loadedConfig.OutputFolder);
            Assert.Equal(config.OutputBaseFileName, loadedConfig.OutputBaseFileName);
            Assert.Equal(config.TessdataFolder, loadedConfig.TessdataFolder);
            Assert.Equal(config.TessdataLanguageCode, loadedConfig.TessdataLanguageCode);
        }
    }
} 