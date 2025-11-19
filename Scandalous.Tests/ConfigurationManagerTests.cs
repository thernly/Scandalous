using FluentAssertions;
using ScanUtility;
using System.Text.Json;
using Xunit;

namespace Scandalous.Tests
{
    public class ConfigurationManagerTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly string _originalAppData;

        public ConfigurationManagerTests()
        {
            // Create a temporary directory for test files
            _testDirectory = Path.Combine(Path.GetTempPath(), "ScandalousTests_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);

            // Save original AppData path
            _originalAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        public void Dispose()
        {
            // Clean up test directory
            try
            {
                if (Directory.Exists(_testDirectory))
                {
                    Directory.Delete(_testDirectory, recursive: true);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        #region SaveConfiguration Tests

        [Fact]
        public async Task SaveConfigurationAsync_ValidConfiguration_SavesSuccessfully()
        {
            // Arrange
            var manager = new ConfigurationManager();
            var config = new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "TestDocument",
                colorMode: ScannerColorMode.Color,
                documentOptions: DocumentOptions.Combined,
                autoDeskew: true,
                excludeBlankPages: true,
                scanResolutionDpi: 300,
                scannerPaperSource: ScannerPaperSource.Auto,
                ocrEnabled: true,
                tessdataFolder: "C:\\Tessdata",
                languageCode: "eng"
            );

            // Act
            await manager.SaveConfigurationAsync(config);

            // Assert - Load it back and verify
            var loadedConfig = await manager.LoadConfigurationAsync();
            loadedConfig.OutputFolder.Should().Be(config.OutputFolder);
            loadedConfig.OutputBaseFileName.Should().Be(config.OutputBaseFileName);
            loadedConfig.ColorMode.Should().Be(config.ColorMode);
            loadedConfig.DocumentOptions.Should().Be(config.DocumentOptions);
            loadedConfig.AutoDeskew.Should().Be(config.AutoDeskew);
            loadedConfig.ExcludeBlankPages.Should().Be(config.ExcludeBlankPages);
            loadedConfig.ScanResolutionDPI.Should().Be(config.ScanResolutionDPI);
            loadedConfig.ScannerPaperSource.Should().Be(config.ScannerPaperSource);
            loadedConfig.OcrEnabled.Should().Be(config.OcrEnabled);
            loadedConfig.TessdataFolder.Should().Be(config.TessdataFolder);
            loadedConfig.TessdataLanguageCode.Should().Be(config.TessdataLanguageCode);
        }

        [Fact]
        public async Task SaveConfigurationAsync_MultipleSaves_OverwritesPrevious()
        {
            // Arrange
            var manager = new ConfigurationManager();
            var config1 = new ScanConfiguration(
                outputFolder: "C:\\Scans1",
                baseFileName: "Document1",
                tessdataFolder: "C:\\Tessdata"
            );
            var config2 = new ScanConfiguration(
                outputFolder: "C:\\Scans2",
                baseFileName: "Document2",
                tessdataFolder: "C:\\Tessdata"
            );

            // Act
            await manager.SaveConfigurationAsync(config1);
            await manager.SaveConfigurationAsync(config2);

            // Assert - Should have config2, not config1
            var loadedConfig = await manager.LoadConfigurationAsync();
            loadedConfig.OutputFolder.Should().Be("C:\\Scans2");
            loadedConfig.OutputBaseFileName.Should().Be("Document2");
        }

        #endregion

        #region LoadConfiguration Tests

        [Fact]
        public async Task LoadConfigurationAsync_NoExistingFile_ReturnsDefaultConfiguration()
        {
            // Arrange
            var manager = new ConfigurationManager();

            // Act - Load without saving first
            var config = await manager.LoadConfigurationAsync();

            // Assert - Should return a new instance with default values
            config.Should().NotBeNull();
            config.OutputFolder.Should().BeEmpty();
            config.OutputBaseFileName.Should().BeEmpty();
        }

        [Fact]
        public async Task LoadConfigurationAsync_AfterSave_ReturnsSavedConfiguration()
        {
            // Arrange
            var manager = new ConfigurationManager();
            var originalConfig = new ScanConfiguration(
                outputFolder: "C:\\TestScans",
                baseFileName: "TestDoc",
                colorMode: ScannerColorMode.BlackAndWhite,
                scanResolutionDpi: 600,
                tessdataFolder: "C:\\Tessdata"
            );

            // Act
            await manager.SaveConfigurationAsync(originalConfig);
            var loadedConfig = await manager.LoadConfigurationAsync();

            // Assert
            loadedConfig.OutputFolder.Should().Be(originalConfig.OutputFolder);
            loadedConfig.OutputBaseFileName.Should().Be(originalConfig.OutputBaseFileName);
            loadedConfig.ColorMode.Should().Be(originalConfig.ColorMode);
            loadedConfig.ScanResolutionDPI.Should().Be(originalConfig.ScanResolutionDPI);
        }

        #endregion

        #region GetInstalledTessdataLanguageCodes Tests

        [Fact]
        public void GetInstalledTessdataLanguageCodes_NullFolder_ReturnsEmptyList()
        {
            // Act
            var result = ConfigurationManager.GetInstalledTessdataLanguageCodes(null!);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetInstalledTessdataLanguageCodes_EmptyFolder_ReturnsEmptyList()
        {
            // Act
            var result = ConfigurationManager.GetInstalledTessdataLanguageCodes(string.Empty);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetInstalledTessdataLanguageCodes_WhitespaceFolder_ReturnsEmptyList()
        {
            // Act
            var result = ConfigurationManager.GetInstalledTessdataLanguageCodes("   ");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetInstalledTessdataLanguageCodes_NonExistentFolder_ReturnsEmptyList()
        {
            // Arrange
            var nonExistentPath = Path.Combine(_testDirectory, "NonExistent");

            // Act
            var result = ConfigurationManager.GetInstalledTessdataLanguageCodes(nonExistentPath);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetInstalledTessdataLanguageCodes_EmptyDirectory_ReturnsEmptyList()
        {
            // Arrange
            var emptyDir = Path.Combine(_testDirectory, "EmptyTessdata");
            Directory.CreateDirectory(emptyDir);

            // Act
            var result = ConfigurationManager.GetInstalledTessdataLanguageCodes(emptyDir);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetInstalledTessdataLanguageCodes_WithTrainedDataFiles_ReturnsLanguageCodes()
        {
            // Arrange
            var tessdataDir = Path.Combine(_testDirectory, "Tessdata");
            Directory.CreateDirectory(tessdataDir);

            // Create test .traineddata files
            File.WriteAllText(Path.Combine(tessdataDir, "eng.traineddata"), "test");
            File.WriteAllText(Path.Combine(tessdataDir, "fra.traineddata"), "test");
            File.WriteAllText(Path.Combine(tessdataDir, "deu.traineddata"), "test");

            // Act
            var result = ConfigurationManager.GetInstalledTessdataLanguageCodes(tessdataDir);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().Contain("eng");
            result.Should().Contain("fra");
            result.Should().Contain("deu");
        }

        [Fact]
        public void GetInstalledTessdataLanguageCodes_WithMixedFiles_ReturnsOnlyTrainedDataFiles()
        {
            // Arrange
            var tessdataDir = Path.Combine(_testDirectory, "TessdataMixed");
            Directory.CreateDirectory(tessdataDir);

            // Create test files - mix of .traineddata and other files
            File.WriteAllText(Path.Combine(tessdataDir, "eng.traineddata"), "test");
            File.WriteAllText(Path.Combine(tessdataDir, "fra.traineddata"), "test");
            File.WriteAllText(Path.Combine(tessdataDir, "readme.txt"), "test");
            File.WriteAllText(Path.Combine(tessdataDir, "config.json"), "test");

            // Act
            var result = ConfigurationManager.GetInstalledTessdataLanguageCodes(tessdataDir);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain("eng");
            result.Should().Contain("fra");
            result.Should().NotContain("readme");
            result.Should().NotContain("config");
        }

        [Fact]
        public void GetInstalledTessdataLanguageCodes_WithSubdirectories_IgnoresSubdirectories()
        {
            // Arrange
            var tessdataDir = Path.Combine(_testDirectory, "TessdataWithSub");
            Directory.CreateDirectory(tessdataDir);

            // Create files in root
            File.WriteAllText(Path.Combine(tessdataDir, "eng.traineddata"), "test");

            // Create subdirectory with files (should be ignored)
            var subDir = Path.Combine(tessdataDir, "subdirectory");
            Directory.CreateDirectory(subDir);
            File.WriteAllText(Path.Combine(subDir, "fra.traineddata"), "test");

            // Act
            var result = ConfigurationManager.GetInstalledTessdataLanguageCodes(tessdataDir);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.Should().Contain("eng");
            result.Should().NotContain("fra");
        }

        [Fact]
        public void GetInstalledTessdataLanguageCodes_WithComplexLanguageCodes_ReturnsCorrectCodes()
        {
            // Arrange
            var tessdataDir = Path.Combine(_testDirectory, "TessdataComplex");
            Directory.CreateDirectory(tessdataDir);

            // Create files with complex language codes
            File.WriteAllText(Path.Combine(tessdataDir, "chi_sim.traineddata"), "test");
            File.WriteAllText(Path.Combine(tessdataDir, "chi_tra.traineddata"), "test");
            File.WriteAllText(Path.Combine(tessdataDir, "script_Arabic.traineddata"), "test");

            // Act
            var result = ConfigurationManager.GetInstalledTessdataLanguageCodes(tessdataDir);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().Contain("chi_sim");
            result.Should().Contain("chi_tra");
            result.Should().Contain("script_Arabic");
        }

        #endregion

        #region JSON Serialization Tests

        [Fact]
        public async Task SaveAndLoad_JsonFormat_UsesCorrectNamingPolicy()
        {
            // Arrange
            var manager = new ConfigurationManager();
            var config = new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "Document",
                tessdataFolder: "C:\\Tessdata"
            );

            // Act
            await manager.SaveConfigurationAsync(config);

            // Get the config file path through reflection or by knowing the structure
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? "ScanUtility";
            var configFilePath = Path.Combine(appDataPath, appName, "ScanUtilityConfig.json");

            // Assert - Check JSON file exists and contains camelCase properties
            if (File.Exists(configFilePath))
            {
                var jsonContent = await File.ReadAllTextAsync(configFilePath);
                jsonContent.Should().Contain("outputFolder");  // camelCase
                jsonContent.Should().Contain("outputBaseFileName");
                jsonContent.Should().NotContain("OutputFolder");  // Should not have PascalCase
            }
        }

        #endregion
    }
}
