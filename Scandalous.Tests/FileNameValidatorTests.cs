using ScanUtility;
using Xunit;

namespace Scandalous.Tests
{
    public class FileNameValidatorTests
    {
        #region IsValid Tests - Null and Empty

        [Fact]
        public void IsValid_NullName_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(null);

            // Assert
            Assert.False(isValid);
            Assert.Contains("cannot be null", errorMessage);
        }

        [Fact]
        public void IsValid_EmptyName_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(string.Empty);

            // Assert
            Assert.False(isValid);
            Assert.Contains("cannot be null, empty", errorMessage);
        }

        [Fact]
        public void IsValid_WhitespaceName_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid("   ");

            // Assert
            Assert.False(isValid);
            Assert.Contains("white-space", errorMessage);
        }

        #endregion

        #region IsValid Tests - Valid Names

        [Theory]
        [InlineData("document")]
        [InlineData("my_file")]
        [InlineData("file-123")]
        [InlineData("file_name_2024")]
        [InlineData("a")]
        [InlineData("ABC")]
        public void IsValid_ValidBaseFileName_ReturnsTrue(string name)
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(name, isBaseNameOnly: true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Theory]
        [InlineData("document.pdf")]
        [InlineData("my_file.txt")]
        [InlineData("archive.tar.gz")]
        [InlineData(".config")]
        [InlineData("file.123")]
        public void IsValid_ValidFullFileName_ReturnsTrue(string name)
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(name, isBaseNameOnly: false);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        #endregion

        #region IsValid Tests - Invalid Characters

        [Theory]
        [InlineData("file<name")]
        [InlineData("file>name")]
        [InlineData("file:name")]
        [InlineData("file\"name")]
        [InlineData("file/name")]
        [InlineData("file\\name")]
        [InlineData("file|name")]
        [InlineData("file?name")]
        [InlineData("file*name")]
        public void IsValid_InvalidCharacters_ReturnsFalse(string name)
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(name);

            // Assert
            Assert.False(isValid);
            Assert.Contains("invalid character", errorMessage);
        }

        #endregion

        #region IsValid Tests - Reserved Names

        [Theory]
        [InlineData("CON")]
        [InlineData("PRN")]
        [InlineData("AUX")]
        [InlineData("NUL")]
        [InlineData("COM1")]
        [InlineData("COM2")]
        [InlineData("COM9")]
        [InlineData("LPT1")]
        [InlineData("LPT9")]
        [InlineData("con")] // case insensitive
        [InlineData("Con")]
        public void IsValid_ReservedNames_ReturnsFalse(string name)
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(name, isBaseNameOnly: true);

            // Assert
            Assert.False(isValid);
            Assert.Contains("reserved", errorMessage);
        }

        [Theory]
        [InlineData("CON.txt")]
        [InlineData("PRN.pdf")]
        [InlineData("AUX.doc")]
        [InlineData("NUL.log")]
        [InlineData("COM1.dat")]
        [InlineData("LPT1.bin")]
        public void IsValid_ReservedNamesWithExtension_ReturnsFalse(string name)
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(name, isBaseNameOnly: false);

            // Assert
            Assert.False(isValid);
            Assert.Contains("reserved", errorMessage);
        }

        #endregion

        #region IsValid Tests - Ending with Space or Period

        [Theory]
        [InlineData("filename ")]
        [InlineData("filename.")]
        [InlineData("test  ")]
        [InlineData("test..")]
        public void IsValid_EndsWithSpaceOrPeriod_ReturnsFalse(string name)
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(name);

            // Assert
            Assert.False(isValid);
            Assert.Contains("cannot end with a space or a period", errorMessage);
        }

        #endregion

        #region IsValid Tests - Dot and DoubleDot

        [Fact]
        public void IsValid_SingleDot_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(".");

            // Assert
            Assert.False(isValid);
            Assert.Contains("cannot end with a space or a period", errorMessage);
        }

        [Fact]
        public void IsValid_DoubleDot_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid("..");

            // Assert
            Assert.False(isValid);
            Assert.Contains("cannot end with a space or a period", errorMessage);
        }

        #endregion

        #region IsValid Tests - Base Name with Period

        [Fact]
        public void IsValid_BaseNameWithPeriod_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid("file.name", isBaseNameOnly: true);

            // Assert
            Assert.False(isValid);
            Assert.Contains("should not contain an extension separator", errorMessage);
        }

        [Fact]
        public void IsValid_BaseNameWithoutPeriod_ReturnsTrue()
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid("filename", isBaseNameOnly: true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        #endregion

        #region IsValid Tests - Length Validation

        [Fact]
        public void IsValid_ExactlyMaxLength_ReturnsTrue()
        {
            // Arrange
            var name = new string('a', 255);

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(name, isBaseNameOnly: true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_ExceedsMaxLength_ReturnsFalse()
        {
            // Arrange
            var name = new string('a', 256);

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(name, isBaseNameOnly: true);

            // Assert
            Assert.False(isValid);
            Assert.Contains("too long", errorMessage);
            Assert.Contains("255", errorMessage);
        }

        #endregion

        #region Validate Method Tests

        [Fact]
        public void Validate_ValidName_DoesNotThrow()
        {
            // Act & Assert
            var exception = Record.Exception(() => FileNameValidator.Validate("validfile", isBaseNameOnly: true));
            Assert.Null(exception);
        }

        [Fact]
        public void Validate_InvalidName_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => FileNameValidator.Validate("invalid/file", isBaseNameOnly: true));
            Assert.Contains("invalid character", exception.Message);
        }

        [Fact]
        public void Validate_NullName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => FileNameValidator.Validate(null));
        }

        [Fact]
        public void Validate_BaseNameWithPeriod_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => FileNameValidator.Validate("file.txt", isBaseNameOnly: true));
            Assert.Contains("extension separator", exception.Message);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void IsValid_HiddenFileWithExtension_ReturnsTrue()
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(".gitignore", isBaseNameOnly: false);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_MultipleExtensions_ReturnsTrue()
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid("archive.tar.gz", isBaseNameOnly: false);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_UnicodeCharacters_ReturnsTrue()
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid("文档", isBaseNameOnly: true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Theory]
        [InlineData("CONOUT")] // Not exactly "CON"
        [InlineData("COM10")] // Not COM1-9
        [InlineData("LPT0")]  // Not LPT1-9
        [InlineData("acon")]  // Contains "con" but not exactly
        public void IsValid_SimilarToReservedButNotReserved_ReturnsTrue(string name)
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(name, isBaseNameOnly: true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        #endregion
    }
}