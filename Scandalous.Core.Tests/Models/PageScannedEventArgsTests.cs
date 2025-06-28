using Scandalous.Core.Models;
using Xunit;

namespace Scandalous.Core.Tests.Models
{
    public class PageScannedEventArgsTests
    {
        [Fact]
        public void Constructor_WithValidPath_SetsImageFilePath()
        {
            // Arrange
            var expectedPath = @"C:\temp\scan-123.png";

            // Act
            var args = new PageScannedEventArgs(expectedPath);

            // Assert
            Assert.Equal(expectedPath, args.ImageFilePath);
        }

        [Fact]
        public void Constructor_WithNullPath_SetsImageFilePathToNull()
        {
            // Arrange
            string? nullPath = null;

            // Act
            var args = new PageScannedEventArgs(nullPath!);

            // Assert
            Assert.Null(args.ImageFilePath);
        }

        [Fact]
        public void Constructor_WithEmptyPath_SetsImageFilePathToEmpty()
        {
            // Arrange
            var emptyPath = "";

            // Act
            var args = new PageScannedEventArgs(emptyPath);

            // Assert
            Assert.Equal(emptyPath, args.ImageFilePath);
        }

        [Fact]
        public void Constructor_WithWhitespacePath_SetsImageFilePathToWhitespace()
        {
            // Arrange
            var whitespacePath = "   ";

            // Act
            var args = new PageScannedEventArgs(whitespacePath);

            // Assert
            Assert.Equal(whitespacePath, args.ImageFilePath);
        }

        [Fact]
        public void Constructor_WithLongPath_SetsImageFilePath()
        {
            // Arrange
            var longPath = new string('a', 1000) + ".png";

            // Act
            var args = new PageScannedEventArgs(longPath);

            // Assert
            Assert.Equal(longPath, args.ImageFilePath);
        }

        [Fact]
        public void Constructor_WithUnicodePath_SetsImageFilePath()
        {
            // Arrange
            var unicodePath = @"C:\temp\résumé-文档-документ.png";

            // Act
            var args = new PageScannedEventArgs(unicodePath);

            // Assert
            Assert.Equal(unicodePath, args.ImageFilePath);
        }

        [Fact]
        public void Constructor_WithRelativePath_SetsImageFilePath()
        {
            // Arrange
            var relativePath = "scans\\page1.png";

            // Act
            var args = new PageScannedEventArgs(relativePath);

            // Assert
            Assert.Equal(relativePath, args.ImageFilePath);
        }

        [Fact]
        public void Constructor_WithNetworkPath_SetsImageFilePath()
        {
            // Arrange
            var networkPath = @"\\server\share\scan.png";

            // Act
            var args = new PageScannedEventArgs(networkPath);

            // Assert
            Assert.Equal(networkPath, args.ImageFilePath);
        }

        [Fact]
        public void ImageFilePath_IsReadOnly()
        {
            // Arrange
            var args = new PageScannedEventArgs("test.png");

            // Act & Assert
            // The property should be read-only (no setter)
            var propertyInfo = typeof(PageScannedEventArgs).GetProperty("ImageFilePath");
            Assert.False(propertyInfo?.CanWrite ?? false);
        }

        [Fact]
        public void Constructor_WithSpecialCharacters_SetsImageFilePath()
        {
            // Arrange
            var specialPath = @"C:\temp\scan with spaces & symbols (1).png";

            // Act
            var args = new PageScannedEventArgs(specialPath);

            // Assert
            Assert.Equal(specialPath, args.ImageFilePath);
        }

        [Fact]
        public void Constructor_WithDifferentFileExtensions_SetsImageFilePath()
        {
            // Arrange
            var extensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".tiff", ".gif" };

            foreach (var extension in extensions)
            {
                var path = $"scan{extension}";

                // Act
                var args = new PageScannedEventArgs(path);

                // Assert
                Assert.Equal(path, args.ImageFilePath);
            }
        }

        [Fact]
        public void Constructor_WithGuidBasedFilename_SetsImageFilePath()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var path = $"scan-{guid}.png";

            // Act
            var args = new PageScannedEventArgs(path);

            // Assert
            Assert.Equal(path, args.ImageFilePath);
        }

        [Fact]
        public void Constructor_WithMultipleInstances_EachHasCorrectPath()
        {
            // Arrange
            var paths = new[] { "scan1.png", "scan2.png", "scan3.png" };

            // Act
            var argsList = paths.Select(path => new PageScannedEventArgs(path)).ToList();

            // Assert
            for (int i = 0; i < paths.Length; i++)
            {
                Assert.Equal(paths[i], argsList[i].ImageFilePath);
            }
        }

        [Fact]
        public void Constructor_WithPathContainingBackslashes_SetsImageFilePath()
        {
            // Arrange
            var pathWithBackslashes = @"C:\temp\subfolder\scan.png";

            // Act
            var args = new PageScannedEventArgs(pathWithBackslashes);

            // Assert
            Assert.Equal(pathWithBackslashes, args.ImageFilePath);
        }

        [Fact]
        public void Constructor_WithPathContainingForwardSlashes_SetsImageFilePath()
        {
            // Arrange
            var pathWithForwardSlashes = "C:/temp/subfolder/scan.png";

            // Act
            var args = new PageScannedEventArgs(pathWithForwardSlashes);

            // Assert
            Assert.Equal(pathWithForwardSlashes, args.ImageFilePath);
        }
    }
} 