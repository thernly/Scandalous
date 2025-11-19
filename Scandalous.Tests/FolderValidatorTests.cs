using FluentAssertions;
using ScanUtility;
using Xunit;

namespace Scandalous.Tests
{
    public class FolderValidatorTests
    {
        #region IsValid Tests - Null and Empty

        [Fact]
        public void IsValid_NullPath_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid(null);

            // Assert
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("cannot be null");
        }

        [Fact]
        public void IsValid_EmptyPath_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid(string.Empty);

            // Assert
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("cannot be null, empty");
        }

        [Fact]
        public void IsValid_WhitespacePath_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid("   ");

            // Assert
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("white-space");
        }

        #endregion

        #region IsValid Tests - Valid Paths

        [Theory]
        [InlineData("Documents")]
        [InlineData("MyFolder")]
        [InlineData("folder_name")]
        [InlineData("folder-123")]
        [InlineData("a")]
        public void IsValid_ValidSimpleFolderName_ReturnsTrue(string folderName)
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);

            // Assert
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
        }

        [Theory]
        [InlineData("C:\\Users\\Documents")]
        [InlineData("C:\\Program Files\\MyApp")]
        [InlineData("D:\\Data\\Scans\\2024")]
        public void IsValid_ValidAbsoluteWindowsPath_ReturnsTrue(string folderName)
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);

            // Assert
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
        }

        [Theory]
        [InlineData("parent/child")]
        [InlineData("folder/subfolder/subsubfolder")]
        [InlineData("a/b/c/d")]
        public void IsValid_ValidRelativePath_ReturnsTrue(string folderName)
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);

            // Assert
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
        }

        [Theory]
        [InlineData("/")]
        [InlineData("\\")]
        public void IsValid_RootPath_ReturnsTrue(string folderName)
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);

            // Assert
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
        }

        [Theory]
        [InlineData("C:")]
        [InlineData("D:")]
        [InlineData("Z:")]
        public void IsValid_DriveLetterOnly_ReturnsTrue(string folderName)
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);

            // Assert
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
        }

        #endregion

        #region IsValid Tests - Invalid Characters in Path

        [Theory]
        [InlineData("folder<name")]
        [InlineData("folder>name")]
        [InlineData("folder\"name")]
        [InlineData("folder|name")]
        public void IsValid_InvalidPathCharacters_ReturnsFalse(string folderName)
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);

            // Assert
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("invalid character");
        }

        #endregion

        #region IsValid Tests - Invalid Characters in Segments

        [Theory]
        [InlineData("parent/child:name")]
        [InlineData("parent/child*name")]
        [InlineData("parent/child?name")]
        public void IsValid_InvalidSegmentCharacters_ReturnsFalse(string folderName)
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);

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
        [InlineData("COM9")]
        [InlineData("LPT1")]
        [InlineData("LPT9")]
        [InlineData("con")] // case insensitive
        [InlineData("Con")]
        public void IsValid_ReservedNames_ReturnsFalse(string folderName)
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);

            // Assert
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("reserved");
        }

        [Theory]
        [InlineData("C:\\Users\\CON\\Documents")]
        [InlineData("parent/PRN/child")]
        [InlineData("folder/AUX")]
        [InlineData("COM1/subfolder")]
        public void IsValid_PathWithReservedSegment_ReturnsFalse(string folderName)
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);

            // Assert
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("reserved");
        }

        #endregion

        #region IsValid Tests - Segments Ending with Space or Period

        [Theory]
        [InlineData("folder ")]
        [InlineData("folder.")]
        [InlineData("parent/child ")]
        [InlineData("parent/child.")]
        public void IsValid_SegmentEndsWithSpaceOrPeriod_ReturnsFalse(string folderName)
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);

            // Assert
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("cannot end with a space or a period");
        }

        #endregion

        #region IsValid Tests - Dot and DoubleDot Segments

        [Fact]
        public void IsValid_SingleDotSegment_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid("parent/./child");

            // Assert
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("'.' or '..' are not allowed");
        }

        [Fact]
        public void IsValid_DoubleDotSegment_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid("parent/../child");

            // Assert
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("'.' or '..' are not allowed");
        }

        [Fact]
        public void IsValid_OnlyDot_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid(".");

            // Assert
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("'.' or '..' are not allowed");
        }

        [Fact]
        public void IsValid_OnlyDoubleDot_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid("..");

            // Assert
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("'.' or '..' are not allowed");
        }

        #endregion

        #region IsValid Tests - Segment Length

        [Fact]
        public void IsValid_SegmentExactlyMaxLength_ReturnsTrue()
        {
            // Arrange
            var segment = new string('a', 255);
            var folderName = $"parent/{segment}";

            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);

            // Assert
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
        }

        [Fact]
        public void IsValid_SegmentExceedsMaxLength_ReturnsFalse()
        {
            // Arrange
            var segment = new string('a', 256);
            var folderName = $"parent/{segment}";

            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);

            // Assert
            isValid.Should().BeFalse();
            errorMessage.Should().Contain("too long");
            errorMessage.Should().Contain("255");
        }

        #endregion

        #region IsValid Tests - Drive Letter Special Cases

        [Theory]
        [InlineData("C:\\Documents")]
        [InlineData("D:\\Data")]
        [InlineData("Z:\\Backup")]
        public void IsValid_ValidDriveLetterPath_ReturnsTrue(string folderName)
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);

            // Assert
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
        }

        [Theory]
        [InlineData("C:\\")]
        [InlineData("D:\\")]
        public void IsValid_DriveLetterWithBackslash_ReturnsTrue(string folderName)
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);

            // Assert
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
        }

        #endregion

        #region Validate Method Tests

        [Fact]
        public void Validate_ValidPath_DoesNotThrow()
        {
            // Act
            Action act = () => FolderValidator.Validate("ValidFolder");

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void Validate_InvalidPath_ThrowsArgumentException()
        {
            // Act
            Action act = () => FolderValidator.Validate("Invalid|Folder");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*invalid character*");
        }

        [Fact]
        public void Validate_NullPath_ThrowsArgumentException()
        {
            // Act
            Action act = () => FolderValidator.Validate(null);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Validate_ReservedName_ThrowsArgumentException()
        {
            // Act
            Action act = () => FolderValidator.Validate("CON");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*reserved*");
        }

        #endregion

        #region Edge Cases

        [Theory]
        [InlineData("folder//subfolder")]  // Double separator
        [InlineData("folder\\\\subfolder")] // Double backslash
        public void IsValid_DoubleSeparators_ReturnsTrue(string folderName)
        {
            // Act - Double separators are removed by split, so remaining segments should be valid
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);

            // Assert
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
        }

        [Fact]
        public void IsValid_MixedSeparators_ReturnsTrue()
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid("parent/child\\grandchild");

            // Assert
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
        }

        [Fact]
        public void IsValid_UnicodeCharacters_ReturnsTrue()
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid("文档/数据");

            // Assert
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
        }

        [Theory]
        [InlineData("CONOUT")] // Not exactly "CON"
        [InlineData("COM10")]  // Not COM1-9
        [InlineData("LPT0")]   // Not LPT1-9
        public void IsValid_SimilarToReservedButNotReserved_ReturnsTrue(string folderName)
        {
            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);

            // Assert
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
        }

        [Fact]
        public void IsValid_LongValidPath_ReturnsTrue()
        {
            // Arrange - Path with total length around 200 (under 240 threshold)
            var folderName = "C:\\Users\\Documents\\Projects\\MyApp\\Data\\Scans\\2024\\January\\Week1\\Day1\\Morning\\Session1\\Documents";

            // Act
            var (isValid, errorMessage) = FolderValidator.IsValid(folderName);

            // Assert
            isValid.Should().BeTrue();
            errorMessage.Should().BeEmpty();
        }

        #endregion
    }
}
