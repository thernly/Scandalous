using NSubstitute;
using Scandalous.Core.Enums;
using Scandalous.Core.Models;
using Scandalous.Core.Services;
using System.Threading.Tasks;
using Xunit;
using System.IO;

namespace Scandalous.Core.Tests.Services
{
    public class DocumentScannerTests
    {
        [Fact]
        public void Constructor_CreatesInstance_Successfully()
        {
            // Arrange & Act
            var scanner = new DocumentScanner();

            // Assert
            Assert.NotNull(scanner);
        }

        [Fact]
        public void PageScannedEvent_CanBeSubscribedTo()
        {
            // Arrange
            var scanner = new DocumentScanner();
            bool eventRaised = false;
            scanner.PageScanned += (s, e) => eventRaised = true;

            // Act
            // We can't easily test the event without mocking the scan controller
            // This test just verifies the event can be subscribed to

            // Assert
            Assert.False(eventRaised); // Should be false since no scan occurs
        }

        [Fact]
        public void Dispose_CanBeCalledMultipleTimes_Safely()
        {
            // Arrange
            var scanner = new DocumentScanner();

            // Act & Assert
            scanner.Dispose();
            scanner.Dispose(); // Should not throw
        }

        [Fact]
        public void Dispose_CanBeCalledOnNewInstance_Safely()
        {
            // Arrange
            var scanner = new DocumentScanner();

            // Act & Assert
            scanner.Dispose(); // Should not throw
        }

        [Fact]
        public async Task ScanDocuments_WithNullConfiguration_ThrowsArgumentNullException()
        {
            // Arrange
            var scanner = new DocumentScanner();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => scanner.ScanDocuments(null!));
            Assert.Equal("configuration", exception.ParamName);
        }

        [Fact]
        public async Task ScanDocuments_WithEmptyOutputFolder_ThrowsArgumentException()
        {
            // Arrange
            var scanner = new DocumentScanner();
            var invalidConfig = new ScanConfiguration
            {
                OutputFolder = "", // Invalid empty folder
                OutputBaseFileName = "test"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => scanner.ScanDocuments(invalidConfig));
            Assert.Contains("Output folder cannot be null, empty, or whitespace", exception.Message);
        }

        [Fact]
        public async Task ScanDocuments_WithWhitespaceOutputFolder_ThrowsArgumentException()
        {
            // Arrange
            var scanner = new DocumentScanner();
            var invalidConfig = new ScanConfiguration
            {
                OutputFolder = "   ", // Invalid whitespace folder
                OutputBaseFileName = "test"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => scanner.ScanDocuments(invalidConfig));
            Assert.Contains("Output folder cannot be null, empty, or whitespace", exception.Message);
        }

        [Fact]
        public async Task ScanDocuments_WithEmptyBaseFileName_ThrowsArgumentException()
        {
            // Arrange
            var scanner = new DocumentScanner();
            var invalidConfig = new ScanConfiguration
            {
                OutputFolder = @"C:\temp",
                OutputBaseFileName = "" // Invalid empty filename
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => scanner.ScanDocuments(invalidConfig));
            Assert.Contains("Output base file name cannot be null, empty, or whitespace", exception.Message);
        }

        [Fact]
        public async Task ScanDocuments_WithWhitespaceBaseFileName_ThrowsArgumentException()
        {
            // Arrange
            var scanner = new DocumentScanner();
            var invalidConfig = new ScanConfiguration
            {
                OutputFolder = @"C:\temp",
                OutputBaseFileName = "   " // Invalid whitespace filename
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => scanner.ScanDocuments(invalidConfig));
            Assert.Contains("Output base file name cannot be null, empty, or whitespace", exception.Message);
        }

        [Fact]
        public async Task ScanDocuments_WithNullOutputFolder_ThrowsArgumentException()
        {
            // Arrange
            var scanner = new DocumentScanner();
            var invalidConfig = new ScanConfiguration
            {
                OutputFolder = null!, // Invalid null folder
                OutputBaseFileName = "test"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => scanner.ScanDocuments(invalidConfig));
            Assert.Contains("Output folder cannot be null, empty, or whitespace", exception.Message);
        }

        [Fact]
        public async Task ScanDocuments_WithNullBaseFileName_ThrowsArgumentException()
        {
            // Arrange
            var scanner = new DocumentScanner();
            var invalidConfig = new ScanConfiguration
            {
                OutputFolder = @"C:\temp",
                OutputBaseFileName = null! // Invalid null filename
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => scanner.ScanDocuments(invalidConfig));
            Assert.Contains("Output base file name cannot be null, empty, or whitespace", exception.Message);
        }

        [Fact]
        public async Task ScanDocuments_AfterDispose_ThrowsObjectDisposedException()
        {
            // Arrange
            var scanner = new DocumentScanner();
            scanner.Dispose();
            var config = new ScanConfiguration
            {
                OutputFolder = @"C:\temp",
                OutputBaseFileName = "test"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ObjectDisposedException>(() => scanner.ScanDocuments(config));
            Assert.Equal(nameof(DocumentScanner), exception.ObjectName);
        }
    }
} 