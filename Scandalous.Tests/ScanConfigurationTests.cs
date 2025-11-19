using FluentAssertions;
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
            config.Should().NotBeNull();
            config.OutputFolder.Should().Be("C:\\Scans");
            config.OutputBaseFileName.Should().Be("Document");
            config.ColorMode.Should().Be(ScannerColorMode.Color);
            config.DocumentOptions.Should().Be(DocumentOptions.Combined);
            config.AutoDeskew.Should().BeTrue();
            config.ExcludeBlankPages.Should().BeTrue();
            config.ScanResolutionDPI.Should().Be(300);
            config.ScannerPaperSource.Should().Be(ScannerPaperSource.Auto);
            config.OcrEnabled.Should().BeFalse();
            config.TessdataFolder.Should().Be("C:\\Tessdata");
            config.TessdataLanguageCode.Should().Be("eng");
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
            config.ColorMode.Should().Be(ScannerColorMode.Grayscale);
            config.DocumentOptions.Should().Be(DocumentOptions.Combined);
            config.AutoDeskew.Should().BeTrue();
            config.ExcludeBlankPages.Should().BeTrue();
            config.ScanResolutionDPI.Should().Be(300);
            config.ScannerPaperSource.Should().Be(ScannerPaperSource.Auto);
            config.OcrEnabled.Should().BeFalse();
            config.TessdataLanguageCode.Should().Be("eng");
        }

        [Fact]
        public void ParameterlessConstructor_CreatesInstance()
        {
            // Act
            var config = new ScanConfiguration();

            // Assert
            config.Should().NotBeNull();
            config.OutputFolder.Should().BeEmpty();
            config.OutputBaseFileName.Should().BeEmpty();
        }

        #endregion

        #region Constructor Tests - Invalid OutputFolder

        [Fact]
        public void Constructor_NullOutputFolder_ThrowsArgumentException()
        {
            // Act
            Action act = () => new ScanConfiguration(
                outputFolder: null!,
                baseFileName: "Document",
                tessdataFolder: "C:\\Tessdata"
            );

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_EmptyOutputFolder_ThrowsArgumentException()
        {
            // Act
            Action act = () => new ScanConfiguration(
                outputFolder: string.Empty,
                baseFileName: "Document",
                tessdataFolder: "C:\\Tessdata"
            );

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_InvalidOutputFolder_ThrowsArgumentException()
        {
            // Act
            Action act = () => new ScanConfiguration(
                outputFolder: "Invalid|Folder",
                baseFileName: "Document",
                tessdataFolder: "C:\\Tessdata"
            );

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*invalid character*");
        }

        [Fact]
        public void Constructor_OutputFolderWithReservedName_ThrowsArgumentException()
        {
            // Act
            Action act = () => new ScanConfiguration(
                outputFolder: "CON",
                baseFileName: "Document",
                tessdataFolder: "C:\\Tessdata"
            );

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*reserved*");
        }

        #endregion

        #region Constructor Tests - Invalid BaseFileName

        [Fact]
        public void Constructor_NullBaseFileName_ThrowsArgumentException()
        {
            // Act
            Action act = () => new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: null!,
                tessdataFolder: "C:\\Tessdata"
            );

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_EmptyBaseFileName_ThrowsArgumentException()
        {
            // Act
            Action act = () => new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: string.Empty,
                tessdataFolder: "C:\\Tessdata"
            );

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_BaseFileNameWithExtension_ThrowsArgumentException()
        {
            // Act
            Action act = () => new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "Document.pdf",
                tessdataFolder: "C:\\Tessdata"
            );

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*extension separator*");
        }

        [Fact]
        public void Constructor_BaseFileNameWithInvalidCharacters_ThrowsArgumentException()
        {
            // Act
            Action act = () => new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "Invalid/Name",
                tessdataFolder: "C:\\Tessdata"
            );

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*invalid character*");
        }

        [Fact]
        public void Constructor_BaseFileNameIsReservedName_ThrowsArgumentException()
        {
            // Act
            Action act = () => new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "CON",
                tessdataFolder: "C:\\Tessdata"
            );

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*reserved*");
        }

        #endregion

        #region Constructor Tests - Invalid TessdataFolder

        [Fact]
        public void Constructor_NullTessdataFolder_ThrowsArgumentException()
        {
            // Act
            Action act = () => new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "Document",
                tessdataFolder: null!
            );

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_EmptyTessdataFolder_ThrowsArgumentException()
        {
            // Act
            Action act = () => new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "Document",
                tessdataFolder: string.Empty
            );

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_InvalidTessdataFolder_ThrowsArgumentException()
        {
            // Act
            Action act = () => new ScanConfiguration(
                outputFolder: "C:\\Scans",
                baseFileName: "Document",
                tessdataFolder: "Invalid|Folder"
            );

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*invalid character*");
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
            config.OutputFolder.Should().Be("C:\\NewScans");
            config.OutputBaseFileName.Should().Be("NewDocument");
            config.ColorMode.Should().Be(ScannerColorMode.BlackAndWhite);
            config.DocumentOptions.Should().Be(DocumentOptions.Individual);
            config.AutoDeskew.Should().BeFalse();
            config.ExcludeBlankPages.Should().BeFalse();
            config.ScanResolutionDPI.Should().Be(600);
            config.ScannerPaperSource.Should().Be(ScannerPaperSource.Flatbed);
            config.OcrEnabled.Should().BeTrue();
            config.TessdataFolder.Should().Be("C:\\NewTessdata");
            config.TessdataLanguageCode.Should().Be("fra");
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
            config.ColorMode.Should().Be(colorMode);
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
            config.DocumentOptions.Should().Be(documentOptions);
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
            config.ScannerPaperSource.Should().Be(paperSource);
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
            config.OutputFolder.Should().Be(longPath);
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
            config.OutputBaseFileName.Should().Be(longName);
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
            config.ScanResolutionDPI.Should().Be(dpi);
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
            config.TessdataLanguageCode.Should().Be(languageCode);
        }

        #endregion
    }
}
