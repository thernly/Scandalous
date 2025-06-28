using Scandalous.Core.Validation;
using Xunit;

namespace Scandalous.Core.Tests.Validation
{
    public class FolderValidatorTests
    {
        [Fact]
        public void IsValid_WithValidFolderName_ReturnsTrue()
        {
            var folderName = "ValidFolder";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithNullFolderName_ReturnsFalse()
        {
            string? folderName = null;
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.False(isValid);
            Assert.Contains("cannot be null", errorMessage);
        }

        [Fact]
        public void IsValid_WithEmptyFolderName_ReturnsFalse()
        {
            var folderName = "";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.False(isValid);
            Assert.Contains("cannot be null", errorMessage);
        }

        [Fact]
        public void IsValid_WithInvalidCharacters_ReturnsFalse()
        {
            var folderName = "Invalid|Name";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.False(isValid);
            Assert.Contains("invalid character", errorMessage);
        }

        [Fact]
        public void IsValid_WithReservedName_ReturnsFalse()
        {
            var folderName = "CON";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.False(isValid);
            Assert.Contains("reserved system name", errorMessage);
        }

        [Fact]
        public void IsValid_WithSegmentEndingWithSpace_ReturnsFalse()
        {
            var folderName = "Folder ";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.False(isValid);
            Assert.Contains("cannot end with a space or a period", errorMessage);
        }

        [Fact]
        public void IsValid_WithDotSegment_ReturnsFalse()
        {
            var folderName = ".";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.False(isValid);
            Assert.Contains("cannot end with a space or a period", errorMessage);
        }

        [Fact]
        public void IsValid_WithDotDotSegment_ReturnsFalse()
        {
            var folderName = "..";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.False(isValid);
            Assert.Contains("cannot end with a space or a period", errorMessage);
        }

        [Fact]
        public void IsValid_WithLongSegment_ReturnsFalse()
        {
            var longSegment = new string('a', 256);
            var (isValid, errorMessage) = FolderValidator.IsValid(longSegment);
            Assert.False(isValid);
            Assert.Contains("too long", errorMessage);
        }

        [Fact]
        public void IsValid_WithRootPath_ReturnsTrue()
        {
            var folderName = "/";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }
    }
} 