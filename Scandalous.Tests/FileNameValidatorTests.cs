using FluentAssertions;
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
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("cannot be null");
        }

        [Fact]
        public void IsValid_EmptyName_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(string.Empty);

            // Assert
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("cannot be null, empty");
        }

        [Fact]
        public void IsValid_WhitespaceName_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid("   ");

            // Assert
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("white-space");
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
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
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
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
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
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("invalid character");
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
        [InlineData("CONOUT")]
        public void IsValid_ReservedNames_ReturnsFalse(string name)
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(name, isBaseNameOnly: true);

            // Assert
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("reserved");
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
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("reserved");
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
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("cannot end with a space or a period");
        }

        #endregion

        #region IsValid Tests - Dot and DoubleDot

        [Fact]
        public void IsValid_SingleDot_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(".");

            // Assert
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("cannot be '.' or '..'");
        }

        [Fact]
        public void IsValid_DoubleDot_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid("..");

            // Assert
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("cannot be '.' or '..'");
        }

        #endregion

        #region IsValid Tests - Base Name with Period

        [Fact]
        public void IsValid_BaseNameWithPeriod_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid("file.name", isBaseNameOnly: true);

            // Assert
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("should not contain an extension separator");
        }

        [Fact]
        public void IsValid_BaseNameWithoutPeriod_ReturnsTrue()
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid("filename", isBaseNameOnly: true);

            // Assert
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
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
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
        }

        [Fact]
        public void IsValid_ExceedsMaxLength_ReturnsFalse()
        {
            // Arrange
            var name = new string('a', 256);

            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(name, isBaseNameOnly: true);

            // Assert
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("too long");
            errorMessage.Should().Contain("255");
        }

        #endregion

        #region Validate Method Tests

        [Fact]
        public void Validate_ValidName_DoesNotThrow()
        {
            // Act
            Action act = () => FileNameValidator.Validate("validfile", isBaseNameOnly: true);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void Validate_InvalidName_ThrowsArgumentException()
        {
            // Act
            Action act = () => FileNameValidator.Validate("invalid/file", isBaseNameOnly: true);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*invalid character*");
        }

        [Fact]
        public void Validate_NullName_ThrowsArgumentException()
        {
            // Act
            Action act = () => FileNameValidator.Validate(null);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Validate_BaseNameWithPeriod_ThrowsArgumentException()
        {
            // Act
            Action act = () => FileNameValidator.Validate("file.txt", isBaseNameOnly: true);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*extension separator*");
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void IsValid_HiddenFileWithExtension_ReturnsTrue()
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid(".gitignore", isBaseNameOnly: false);

            // Assert
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
        }

        [Fact]
        public void IsValid_MultipleExtensions_ReturnsTrue()
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid("archive.tar.gz", isBaseNameOnly: false);

            // Assert
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
        }

        [Fact]
        public void IsValid_UnicodeCharacters_ReturnsTrue()
        {
            // Act
            var (isValid, errorMessage) = FileNameValidator.IsValid("文档", isBaseNameOnly: true);

            // Assert
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
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
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
        }

        #endregion
    }
}
