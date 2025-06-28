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
        public void IsValid_WithBaseNameOnly_AcceptsValidName()
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
    }
} 