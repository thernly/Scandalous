using ScanUtility;
using Xunit;

namespace Scandalous.Tests
{
    public class ScanConfigurationTests
    {
        #region Constructor Tests - Valid Inputs

        [Fact]
        public void Constructor_ValidParameters_CreatesInstance()
        {
            // Arrange & Act
            var config = new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "Document",
                colorMode: ScannerColorMode.Color,
                documentOptions: DocumentOptions.Combined,
                autoDeskew: true,
                excludeBlankPages: true,
                scanResolutionDpi: 300,
                scannerPaperSource: ScannerPaperSource.Auto,
                ocrEnabled: false,
                tessdataFolder: "C:\\Tessdata",
                languageCode: "eng"
            );

            // Assert
            Assert.NotNull(config);
            Assert.Equal("C:\\Scans", config.OutputFolder);
            Assert.Equal("Document", config.OutputBaseFileName);
            Assert.Equal(ScannerColorMode.Color, config.ColorMode);
            Assert.Equal(DocumentOptions.Combined, config.DocumentOptions);
            Assert.True(config.AutoDeskew);
            Assert.True(config.ExcludeBlankPages);
            Assert.Equal(300, config.ScanResolutionDPI);
            Assert.Equal(ScannerPaperSource.Auto, config.ScannerPaperSource);
            Assert.False(config.OcrEnabled);
            Assert.Equal("C:\\Tessdata", config.TessdataFolder);
            Assert.Equal("eng", config.TessdataLanguageCode);
        }

        [Fact]
        public void Constructor_DefaultParameters_UsesDefaults()
        {
            // Arrange & Act
            var config = new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "Document",
                tessdataFolder: "C:\\Tessdata"
            );

            // Assert
            Assert.Equal(ScannerColorMode.Grayscale, config.ColorMode);
            Assert.Equal(DocumentOptions.Combined, config.DocumentOptions);
            Assert.True(config.AutoDeskew);
            Assert.True(config.ExcludeBlankPages);
            Assert.Equal(300, config.ScanResolutionDPI);
            Assert.Equal(ScannerPaperSource.Auto, config.ScannerPaperSource);
            Assert.False(config.OcrEnabled);
            Assert.Equal("eng", config.TessdataLanguageCode);
        }

        [Fact]
        public void ParameterlessConstructor_CreatesInstance()
        {
            // Act
            var config = new ScanConfiguration();

            // Assert
            Assert.NotNull(config);
            Assert.Empty(config.OutputFolder);
            Assert.Empty(config.OutputBaseFileName);
        }

        #endregion

        #region Constructor Tests - Invalid OutputFolder

        [Fact]
        public void Constructor_NullOutputFolder_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: null!,
                baseFileName: "Document",
                tessdataFolder: "C:\\Tessdata"
            ));
        }

        [Fact]
        public void Constructor_EmptyOutputFolder_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: string.Empty,
                baseFileName: "Document",
                tessdataFolder: "C:\\Tessdata"
            ));
        }

        [Fact]
        public void Constructor_InvalidOutputFolder_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: "Invalid|Folder",
                baseFileName: "Document",
                tessdataFolder: "C:\\Tessdata"
            ));
            Assert.Contains("invalid character", exception.Message);
        }

        [Fact]
        public void Constructor_OutputFolderWithReservedName_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: "CON",
                baseFileName: "Document",
                tessdataFolder: "C:\\Tessdata"
            ));
            Assert.Contains("reserved", exception.Message);
        }

        #endregion

        #region Constructor Tests - Invalid BaseFileName

        [Fact]
        public void Constructor_NullBaseFileName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: null!,
                tessdataFolder: "C:\\Tessdata"
            ));
        }

        [Fact]
        public void Constructor_EmptyBaseFileName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: string.Empty,
                tessdataFolder: "C:\\Tessdata"
            ));
        }

        [Fact]
        public void Constructor_BaseFileNameWithExtension_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "Document.pdf",
                tessdataFolder: "C:\\Tessdata"
            ));
            Assert.Contains("extension separator", exception.Message);
        }

        [Fact]
        public void Constructor_BaseFileNameWithInvalidCharacters_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "Invalid/Name",
                tessdataFolder: "C:\\Tessdata"
            ));
            Assert.Contains("invalid character", exception.Message);
        }

        [Fact]
        public void Constructor_BaseFileNameIsReservedName_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "CON",
                tessdataFolder: "C:\\Tessdata"
            ));
            Assert.Contains("reserved", exception.Message);
        }

        #endregion

        #region Constructor Tests - Invalid TessdataFolder

        [Fact]
        public void Constructor_NullTessdataFolder_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "Document",
                tessdataFolder: null!
            ));
        }

        [Fact]
        public void Constructor_EmptyTessdataFolder_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "Document",
                tessdataFolder: string.Empty
            ));
        }

        [Fact]
        public void Constructor_InvalidTessdataFolder_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "Document",
                tessdataFolder: "Invalid|Folder"
            ));
            Assert.Contains("invalid character", exception.Message);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void Properties_CanBeSetAndRetrieved()
        {
            // Arrange
            var config = new ScanConfiguration();

            // Act
            config.OutputFolder = "C:\\NewScans";
            config.OutputBaseFileName = "NewDocument";
            config.ColorMode = ScannerColorMode.BlackAndWhite;
            config.DocumentOptions = DocumentOptions.Individual;
            config.AutoDeskew = false;
            config.ExcludeBlankPages = false;
            config.ScanResolutionDPI = 600;
            config.ScannerPaperSource = ScannerPaperSource.Flatbed;
            config.OcrEnabled = true;
            config.TessdataFolder = "C:\\NewTessdata";
            config.TessdataLanguageCode = "fra";

            // Assert
            Assert.Equal("C:\\NewScans", config.OutputFolder);
            Assert.Equal("NewDocument", config.OutputBaseFileName);
            Assert.Equal(ScannerColorMode.BlackAndWhite, config.ColorMode);
            Assert.Equal(DocumentOptions.Individual, config.DocumentOptions);
            Assert.False(config.AutoDeskew);
            Assert.False(config.ExcludeBlankPages);
            Assert.Equal(600, config.ScanResolutionDPI);
            Assert.Equal(ScannerPaperSource.Flatbed, config.ScannerPaperSource);
            Assert.True(config.OcrEnabled);
            Assert.Equal("C:\\NewTessdata", config.TessdataFolder);
            Assert.Equal("fra", config.TessdataLanguageCode);
        }

        #endregion

        #region Enum Value Tests

        [Theory]
        [InlineData(ScannerColorMode.Grayscale)]
        [InlineData(ScannerColorMode.BlackAndWhite)]
        [InlineData(ScannerColorMode.Color)]
        public void Constructor_AllColorModeValues_Accepted(ScannerColorMode colorMode)
        {
            // Act
            var config = new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "Document",
                colorMode: colorMode,
                tessdataFolder: "C:\\Tessdata"
            );

            // Assert
            Assert.Equal(colorMode, config.ColorMode);
        }

        [Theory]
        [InlineData(DocumentOptions.Individual)]
        [InlineData(DocumentOptions.Combined)]
        public void Constructor_AllDocumentOptionsValues_Accepted(DocumentOptions documentOptions)
        {
            // Act
            var config = new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "Document",
                documentOptions: documentOptions,
                tessdataFolder: "C:\\Tessdata"
            );

            // Assert
            Assert.Equal(documentOptions, config.DocumentOptions);
        }

        [Theory]
        [InlineData(ScannerPaperSource.Auto)]
        [InlineData(ScannerPaperSource.Flatbed)]
        [InlineData(ScannerPaperSource.FeederDuplex)]
        [InlineData(ScannerPaperSource.FeederSimplex)]
        public void Constructor_AllPaperSourceValues_Accepted(ScannerPaperSource paperSource)
        {
            // Act
            var config = new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "Document",
                scannerPaperSource: paperSource,
                tessdataFolder: "C:\\Tessdata"
            );

            // Assert
            Assert.Equal(paperSource, config.ScannerPaperSource);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void Constructor_VeryLongValidFolderPath_Accepted()
        {
            // Arrange
            var longPath = "C:\\" + new string('a', 200);

            // Act
            var config = new ScanConfiguration(
                outputFolder: longPath,
                baseFileName: "Document",
                tessdataFolder: "C:\\Tessdata"
            );

            // Assert
            Assert.Equal(longPath, config.OutputFolder);
        }

        [Fact]
        public void Constructor_VeryLongValidBaseFileName_Accepted()
        {
            // Arrange
            var longName = new string('a', 255);

            // Act
            var config = new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: longName,
                tessdataFolder: "C:\\Tessdata"
            );

            // Assert
            Assert.Equal(longName, config.OutputBaseFileName);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(150)]
        [InlineData(300)]
        [InlineData(600)]
        [InlineData(1200)]
        public void Constructor_VariousDpiValues_Accepted(int dpi)
        {
            // Act
            var config = new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "Document",
                scanResolutionDpi: dpi,
                tessdataFolder: "C:\\Tessdata"
            );

            // Assert
            Assert.Equal(dpi, config.ScanResolutionDPI);
        }

        [Theory]
        [InlineData("eng")]
        [InlineData("fra")]
        [InlineData("deu")]
        [InlineData("spa")]
        [InlineData("chi_sim")]
        public void Constructor_VariousLanguageCodes_Accepted(string languageCode)
        {
            // Act
            var config = new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "Document",
                languageCode: languageCode,
                tessdataFolder: "C:\\Tessdata"
            );

            // Assert
            Assert.Equal(languageCode, config.TessdataLanguageCode);
        }

        #endregion
    }
}
