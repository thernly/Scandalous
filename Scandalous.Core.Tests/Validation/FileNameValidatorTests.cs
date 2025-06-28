using Scandalous.Core.Validation;
using Xunit;

namespace Scandalous.Core.Tests.Validation
{
    public class FileNameValidatorTests
    {
        [Fact]
        public void IsValid_WithValidFileName_ReturnsTrue()
        {
            // Arrange
            var fileName = "valid-file-name.txt";

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(fileName);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithNullFileName_ReturnsFalse()
        {
            // Arrange
            string? fileName = null;

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(fileName);

            // Assert
            Assert.False(isValid);
            Assert.Contains("cannot be null", errorMessage);
        }

        [Fact]
        public void IsValid_WithEmptyFileName_ReturnsFalse()
        {
            // Arrange
            var fileName = "";

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(fileName);

            // Assert
            Assert.False(isValid);
            Assert.Contains("cannot be null", errorMessage);
        }

        [Fact]
        public void IsValid_WithWhitespaceOnlyFileName_ReturnsFalse()
        {
            // Arrange
            var fileName = "   ";

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(fileName);

            // Assert
            Assert.False(isValid);
            Assert.Contains("cannot be null", errorMessage);
        }

        [Fact]
        public void IsValid_WithInvalidCharacters_ReturnsFalse()
        {
            // Arrange
            var fileName = "file<name>.txt";

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(fileName);

            // Assert
            Assert.False(isValid);
            Assert.Contains("invalid character", errorMessage);
        }

        [Fact]
        public void IsValid_WithMultipleInvalidCharacters_ReturnsFalse()
        {
            // Arrange
            var fileName = "file<name>|with*invalid?chars.txt";

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(fileName);

            // Assert
            Assert.False(isValid);
            Assert.Contains("invalid character", errorMessage);
        }

        [Fact]
        public void IsValid_WithReservedName_ReturnsFalse()
        {
            // Arrange
            var fileName = "CON.txt";

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(fileName);

            // Assert
            Assert.False(isValid);
            Assert.Contains("reserved system name", errorMessage);
        }

        [Fact]
        public void IsValid_WithReservedNameInDifferentCase_ReturnsFalse()
        {
            // Arrange
            var fileName = "con.txt";

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(fileName);

            // Assert
            Assert.False(isValid);
            Assert.Contains("reserved system name", errorMessage);
        }

        [Fact]
        public void IsValid_WithReservedNameWithExtension_ReturnsFalse()
        {
            // Arrange
            var fileName = "COM1.txt";

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(fileName);

            // Assert
            Assert.False(isValid);
            Assert.Contains("reserved system name", errorMessage);
        }

        [Fact]
        public void IsValid_WithFileNameEndingWithSpace_ReturnsFalse()
        {
            // Arrange
            var fileName = "filename ";

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(fileName);

            // Assert
            Assert.False(isValid);
            Assert.Contains("cannot end with a space or a period", errorMessage);
        }

        [Fact]
        public void IsValid_WithFileNameEndingWithPeriod_ReturnsFalse()
        {
            // Arrange
            var fileName = "filename.";

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(fileName);

            // Assert
            Assert.False(isValid);
            Assert.Contains("cannot end with a space or a period", errorMessage);
        }

        [Fact]
        public void IsValid_WithDotFileName_ReturnsFalse()
        {
            // Arrange
            var fileName = ".";

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(fileName);

            // Assert
            Assert.False(isValid);
            Assert.Contains("cannot end with a space or a period", errorMessage);
        }

        [Fact]
        public void IsValid_WithDotDotFileName_ReturnsFalse()
        {
            // Arrange
            var fileName = "..";

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(fileName);

            // Assert
            Assert.False(isValid);
            Assert.Contains("cannot end with a space or a period", errorMessage);
        }

        [Fact]
        public void IsValid_WithMaxLengthFileName_ReturnsTrue()
        {
            // Arrange
            var fileName = new string('a', 255);

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(fileName);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithExceedingMaxLengthFileName_ReturnsFalse()
        {
            // Arrange
            var fileName = new string('a', 256);

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(fileName);

            // Assert
            Assert.False(isValid);
            Assert.Contains("too long", errorMessage);
        }

        [Fact]
        public void IsValid_WithUnicodeCharacters_ReturnsTrue()
        {
            // Arrange
            var fileName = "résumé-文档-документ.txt";

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(fileName);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithValidBaseNameOnly_AcceptsValidName()
        {
            // Arrange
            var baseName = "validbasename";

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(baseName, isBaseNameOnly: true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithBaseNameOnly_RejectsNameWithExtension()
        {
            // Arrange
            var baseName = "filename.txt";

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(baseName, isBaseNameOnly: true);

            // Assert
            Assert.False(isValid);
            Assert.Contains("should not contain an extension separator", errorMessage);
        }

        [Fact]
        public void IsValid_WithBaseNameOnly_RejectsNameWithMultipleExtensions()
        {
            // Arrange
            var baseName = "archive.tar.gz";

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(baseName, isBaseNameOnly: true);

            // Assert
            Assert.False(isValid);
            Assert.Contains("should not contain an extension separator", errorMessage);
        }

        [Fact]
        public void IsValid_WithBaseNameOnly_AcceptsNameWithDotInMiddle()
        {
            // Arrange
            var baseName = "my.file.name";

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(baseName, isBaseNameOnly: true);

            // Assert
            Assert.False(isValid);
            Assert.Contains("should not contain an extension separator", errorMessage);
        }

        [Fact]
        public void IsValid_WithFullFileName_AcceptsNameWithMultipleExtensions()
        {
            // Arrange
            var fileName = "archive.tar.gz";

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(fileName, isBaseNameOnly: false);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithFileNameStartingWithDot_ReturnsTrue()
        {
            // Arrange
            var fileName = ".config";

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(fileName);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithFileNameWithOnlyExtension_ReturnsTrue()
        {
            // Arrange
            var fileName = ".txt";

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(fileName);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Theory]
        [InlineData("CON")]
        [InlineData("PRN")]
        [InlineData("AUX")]
        [InlineData("NUL")]
        [InlineData("COM1")]
        [InlineData("COM2")]
        [InlineData("COM3")]
        [InlineData("COM4")]
        [InlineData("COM5")]
        [InlineData("COM6")]
        [InlineData("COM7")]
        [InlineData("COM8")]
        [InlineData("COM9")]
        [InlineData("LPT1")]
        [InlineData("LPT2")]
        [InlineData("LPT3")]
        [InlineData("LPT4")]
        [InlineData("LPT5")]
        [InlineData("LPT6")]
        [InlineData("LPT7")]
        [InlineData("LPT8")]
        [InlineData("LPT9")]
        public void IsValid_WithAllReservedNames_ReturnsFalse(string reservedName)
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(reservedName);

            // Assert
            Assert.False(isValid);
            Assert.Contains("reserved system name", errorMessage);
        }

        [Theory]
        [InlineData("CON.txt")]
        [InlineData("PRN.txt")]
        [InlineData("AUX.txt")]
        [InlineData("NUL.txt")]
        [InlineData("COM1.txt")]
        [InlineData("LPT1.txt")]
        public void IsValid_WithReservedNamesWithExtensions_ReturnsFalse(string reservedNameWithExt)
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(reservedNameWithExt);

            // Assert
            Assert.False(isValid);
            Assert.Contains("reserved system name", errorMessage);
        }
    }
} 