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
        public void IsValid_WithWhitespaceOnlyFolderName_ReturnsFalse()
        {
            var folderName = "   ";
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
        public void IsValid_WithMultipleInvalidCharacters_ReturnsFalse()
        {
            var folderName = "Invalid<Name>With*Multiple?Chars";
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
        public void IsValid_WithReservedNameInDifferentCase_ReturnsFalse()
        {
            var folderName = "con";
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
        public void IsValid_WithSegmentEndingWithPeriod_ReturnsFalse()
        {
            var folderName = "Folder.";
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
        public void IsValid_WithMaxLengthSegment_ReturnsTrue()
        {
            var maxLengthSegment = new string('a', 255);
            var (isValid, errorMessage) = FolderValidator.IsValid(maxLengthSegment);
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithRootPath_ReturnsTrue()
        {
            var folderName = "/";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithWindowsRootPath_ReturnsTrue()
        {
            var folderName = "\\";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithMultipleBackslashes_ReturnsTrue()
        {
            var folderName = "\\\\\\";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithValidMultiSegmentPath_ReturnsTrue()
        {
            var folderName = "ValidFolder/SubFolder/AnotherFolder";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithValidMultiSegmentPathWithBackslashes_ReturnsTrue()
        {
            var folderName = "ValidFolder\\SubFolder\\AnotherFolder";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithMixedSeparators_ReturnsTrue()
        {
            var folderName = "ValidFolder/SubFolder\\AnotherFolder";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithMultiSegmentPathWithReservedName_ReturnsFalse()
        {
            var folderName = "ValidFolder/CON/AnotherFolder";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.False(isValid);
            Assert.Contains("reserved system name", errorMessage);
        }

        [Fact]
        public void IsValid_WithMultiSegmentPathWithInvalidSegment_ReturnsFalse()
        {
            var folderName = "ValidFolder/Invalid|Name/AnotherFolder";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.False(isValid);
            Assert.Contains("invalid character", errorMessage);
        }

        [Fact]
        public void IsValid_WithMultiSegmentPathWithSegmentEndingWithSpace_ReturnsFalse()
        {
            var folderName = "ValidFolder/SubFolder /AnotherFolder";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.False(isValid);
            Assert.Contains("cannot end with a space or a period", errorMessage);
        }

        [Fact]
        public void IsValid_WithMultiSegmentPathWithDotSegment_ReturnsFalse()
        {
            var folderName = "ValidFolder/./AnotherFolder";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.False(isValid);
            Assert.Contains("cannot end with a space or a period", errorMessage);
        }

        [Fact]
        public void IsValid_WithMultiSegmentPathWithDotDotSegment_ReturnsFalse()
        {
            var folderName = "ValidFolder/../AnotherFolder";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.False(isValid);
            Assert.Contains("cannot end with a space or a period", errorMessage);
        }

        [Fact]
        public void IsValid_WithDriveLetter_ReturnsTrue()
        {
            var folderName = "C:";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithDriveLetterAndPath_ReturnsTrue()
        {
            var folderName = "C:/Program Files/MyApp";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithDriveLetterAndBackslashPath_ReturnsTrue()
        {
            var folderName = "C:\\Program Files\\MyApp";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithInvalidDriveLetter_ReturnsFalse()
        {
            var folderName = "1:";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.False(isValid);
            Assert.Contains("invalid character", errorMessage);
        }

        [Fact]
        public void IsValid_WithDriveLetterEndingWithSpace_ReturnsFalse()
        {
            var folderName = "C: ";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.False(isValid);
            Assert.Contains("contains an invalid character", errorMessage);
        }

        [Fact]
        public void IsValid_WithDriveLetterEndingWithPeriod_ReturnsFalse()
        {
            var folderName = "C:.";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.False(isValid);
            Assert.Contains("contains an invalid character", errorMessage);
        }

        [Fact]
        public void IsValid_WithLongPath_ReturnsTrue()
        {
            var longPath = "Folder1/Folder2/Folder3/Folder4/Folder5/Folder6/Folder7/Folder8/Folder9/Folder10";
            var (isValid, errorMessage) = FolderValidator.IsValid(longPath);
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithVeryLongPath_ReturnsTrue()
        {
            var veryLongPath = new string('a', 200) + "/" + new string('b', 200);
            var (isValid, errorMessage) = FolderValidator.IsValid(veryLongPath);
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithUnicodeCharacters_ReturnsTrue()
        {
            var folderName = "résumé-文档-документ";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithUnicodeMultiSegmentPath_ReturnsTrue()
        {
            var folderName = "résumé/文档/документ";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithEmptySegments_ReturnsTrue()
        {
            var folderName = "Folder1//Folder2";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithMultipleEmptySegments_ReturnsTrue()
        {
            var folderName = "Folder1///Folder2";
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);
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
            var (isValid, errorMessage) = FolderValidator.IsValid(reservedName);
            Assert.False(isValid);
            Assert.Contains("reserved system name", errorMessage);
        }

        [Theory]
        [InlineData("ValidFolder/CON/AnotherFolder")]
        [InlineData("CON/ValidFolder/AnotherFolder")]
        [InlineData("ValidFolder/AnotherFolder/CON")]
        public void IsValid_WithReservedNameInMultiSegmentPath_ReturnsFalse(string pathWithReservedName)
        {
            var (isValid, errorMessage) = FolderValidator.IsValid(pathWithReservedName);
            Assert.False(isValid);
            Assert.Contains("reserved system name", errorMessage);
        }
    }
} 