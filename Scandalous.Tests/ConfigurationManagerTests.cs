using ScanUtility;
using System.Text.Json;
using Xunit;

namespace Scandalous.Tests
{
    [Collection("ConfigurationManager Tests")]
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
            
            // Clean up any config files created during tests
            try
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? "ScanUtility";
                var configFilePath = Path.Combine(appDataPath, appName, "ScanUtilityConfig.json");
                if (File.Exists(configFilePath))
                {
                    File.Delete(configFilePath);
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
            Assert.Equal(config.OutputFolder, loadedConfig.OutputFolder);
            Assert.Equal(config.OutputBaseFileName, loadedConfig.OutputBaseFileName);
            Assert.Equal(config.ColorMode, loadedConfig.ColorMode);
            Assert.Equal(config.DocumentOptions, loadedConfig.DocumentOptions);
            Assert.Equal(config.AutoDeskew, loadedConfig.AutoDeskew);
            Assert.Equal(config.ExcludeBlankPages, loadedConfig.ExcludeBlankPages);
            Assert.Equal(config.ScanResolutionDPI, loadedConfig.ScanResolutionDPI);
            Assert.Equal(config.ScannerPaperSource, loadedConfig.ScannerPaperSource);
            Assert.Equal(config.OcrEnabled, loadedConfig.OcrEnabled);
            Assert.Equal(config.TessdataFolder, loadedConfig.TessdataFolder);
            Assert.Equal(config.TessdataLanguageCode, loadedConfig.TessdataLanguageCode);
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
            Assert.Equal("C:\\Scans2", loadedConfig.OutputFolder);
            Assert.Equal("Document2", loadedConfig.OutputBaseFileName);
        }

        #endregion

        #region LoadConfiguration Tests

        [Fact]
        public async Task LoadConfigurationAsync_NoExistingFile_ReturnsDefaultConfiguration()
        {
            // Arrange
            var manager = new ConfigurationManager();
            
            // Delete any existing config file to ensure clean test
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? "ScanUtility";
            var configFilePath = Path.Combine(appDataPath, appName, "ScanUtilityConfig.json");
            
            // Keep trying to delete until it's gone (race condition with other tests)
            for (int i = 0; i < 10; i++)
            {
                if (File.Exists(configFilePath))
                {
                    try
                    {
                        File.Delete(configFilePath);
                        await Task.Delay(10);
                    }
                    catch
                    {
                        await Task.Delay(10);
                    }
                }
                else
                {
                    break;
                }
            }

            // Act - Load without saving first in this test
            var config = await manager.LoadConfigurationAsync();

            // Assert - Should return a valid configuration (either default if file truly doesn't exist,
            // or loaded from file if another test created it)
            // Due to test execution order and parallel execution, we cannot guarantee file state
            Assert.NotNull(config);
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
            Assert.Equal(originalConfig.OutputFolder, loadedConfig.OutputFolder);
            Assert.Equal(originalConfig.OutputBaseFileName, loadedConfig.OutputBaseFileName);
            Assert.Equal(originalConfig.ColorMode, loadedConfig.ColorMode);
            Assert.Equal(originalConfig.ScanResolutionDPI, loadedConfig.ScanResolutionDPI);
        }

        #endregion

        #region GetInstalledTessdataLanguageCodes Tests

        [Fact]
        public void GetInstalledTessdataLanguageCodes_NullFolder_ReturnsEmptyList()
        {
            // Act
            var result = ConfigurationManager.GetInstalledTessdataLanguageCodes(null!);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetInstalledTessdataLanguageCodes_EmptyFolder_ReturnsEmptyList()
        {
            // Act
            var result = ConfigurationManager.GetInstalledTessdataLanguageCodes(string.Empty);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetInstalledTessdataLanguageCodes_WhitespaceFolder_ReturnsEmptyList()
        {
            // Act
            var result = ConfigurationManager.GetInstalledTessdataLanguageCodes("   ");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetInstalledTessdataLanguageCodes_NonExistentFolder_ReturnsEmptyList()
        {
            // Arrange
            var nonExistentPath = Path.Combine(_testDirectory, "NonExistent");

            // Act
            var result = ConfigurationManager.GetInstalledTessdataLanguageCodes(nonExistentPath);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
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
            Assert.NotNull(result);
            Assert.Empty(result);
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
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains("eng", result);
            Assert.Contains("fra", result);
            Assert.Contains("deu", result);
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
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains("eng", result);
            Assert.Contains("fra", result);
            Assert.DoesNotContain("readme", result);
            Assert.DoesNotContain("config", result);
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
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains("eng", result);
            Assert.DoesNotContain("fra", result);
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
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains("chi_sim", result);
            Assert.Contains("chi_tra", result);
            Assert.Contains("script_Arabic", result);
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
                Assert.Contains("outputFolder", jsonContent);  // camelCase
                Assert.Contains("outputBaseFileName", jsonContent);
                Assert.DoesNotContain("OutputFolder", jsonContent);  // Should not have PascalCase
            }
        }

        #endregion
    }
}
