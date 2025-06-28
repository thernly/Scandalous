using NSubstitute;
using Scandalous.Core.Enums;
using Scandalous.Core.Models;
using Scandalous.Core.Services;
using System.Threading.Tasks;
using Xunit;
using System.IO;

namespace Scandalous.Core.Tests.Services
{
    public class DocumentScannerEnhancedTests : IDisposable
    {
        private readonly DocumentScanner _scanner;
        private readonly string _testOutputFolder;

        public DocumentScannerEnhancedTests()
        {
            _scanner = new DocumentScanner();
            _testOutputFolder = Path.Combine(Path.GetTempPath(), "ScandalousTests");
            
            // Ensure test directory exists
            if (!Directory.Exists(_testOutputFolder))
            {
                Directory.CreateDirectory(_testOutputFolder);
            }
        }

        public void Dispose()
        {
            _scanner?.Dispose();
            
            // Clean up test directory
            try
            {
                if (Directory.Exists(_testOutputFolder))
                {
                    Directory.Delete(_testOutputFolder, true);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        #region Event Handling Tests

        [Fact]
        public void PageScannedEvent_CanBeSubscribedAndUnsubscribed()
        {
            // Arrange
            var eventRaised = false;
            string? capturedFilePath = null;
            
            EventHandler<PageScannedEventArgs> handler = (sender, e) =>
            {
                eventRaised = true;
                capturedFilePath = e.ImageFilePath;
            };

            // Act - Subscribe
            _scanner.PageScanned += handler;

            // Assert - Event can be subscribed
            Assert.False(eventRaised);
            Assert.Null(capturedFilePath);

            // Act - Unsubscribe
            _scanner.PageScanned -= handler;

            // Assert - Event can be unsubscribed without issues
            Assert.False(eventRaised);
        }

        [Fact]
        public void PageScannedEvent_CanHaveMultipleSubscribers()
        {
            // Arrange
            EventHandler<PageScannedEventArgs> handler1 = (sender, e) => { };
            EventHandler<PageScannedEventArgs> handler2 = (sender, e) => { };

            // Act
            _scanner.PageScanned += handler1;
            _scanner.PageScanned += handler2;

            // Assert - No exception should be thrown

            // Cleanup
            _scanner.PageScanned -= handler1;
            _scanner.PageScanned -= handler2;
        }

        [Fact]
        public void PageScannedEvent_CanBeSubscribedToMultipleTimes()
        {
            // Arrange
            var eventCount = 0;
            
            EventHandler<PageScannedEventArgs> handler = (sender, e) => eventCount++;

            // Act
            _scanner.PageScanned += handler;
            _scanner.PageScanned += handler; // Same handler twice

            // Assert
            Assert.Equal(0, eventCount);

            // Cleanup
            _scanner.PageScanned -= handler;
            _scanner.PageScanned -= handler;
        }

        [Fact]
        public void PageScannedEvent_CanBeUnsubscribedFromMultipleTimes()
        {
            // Arrange
            EventHandler<PageScannedEventArgs> handler = (sender, e) => { };

            // Act
            _scanner.PageScanned += handler;
            _scanner.PageScanned -= handler;
            _scanner.PageScanned -= handler; // Unsubscribe again

            // Assert - No exception should be thrown
        }

        [Fact]
        public void PageScannedEvent_CanBeUnsubscribedFromWithoutBeingSubscribed()
        {
            // Arrange
            EventHandler<PageScannedEventArgs> handler = (sender, e) => { };

            // Act & Assert - Should not throw
            _scanner.PageScanned -= handler;
        }

        #endregion

        #region Dispose Pattern Tests

        [Fact]
        public void Dispose_CanBeCalledMultipleTimes_Safely()
        {
            // Arrange
            var scanner = new DocumentScanner();

            // Act & Assert
            scanner.Dispose();
            scanner.Dispose(); // Should not throw
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
        public void Dispose_CanBeCalledOnDisposedInstance_Safely()
        {
            // Arrange
            var scanner = new DocumentScanner();
            scanner.Dispose();

            // Act & Assert
            scanner.Dispose(); // Should not throw
        }

        [Fact]
        public void Dispose_CanBeCalledOnInstanceWithEventSubscribers_Safely()
        {
            // Arrange
            var scanner = new DocumentScanner();
            scanner.PageScanned += (sender, e) => { };

            // Act & Assert
            scanner.Dispose(); // Should not throw
        }

        [Fact]
        public void Dispose_CanBeCalledOnInstanceWithMultipleEventSubscribers_Safely()
        {
            // Arrange
            var scanner = new DocumentScanner();
            
            scanner.PageScanned += (sender, e) => { };
            scanner.PageScanned += (sender, e) => { };

            // Act & Assert
            scanner.Dispose(); // Should not throw
        }

        [Fact]
        public void Dispose_CanBeCalledOnInstanceWithUnsubscribedEvents_Safely()
        {
            // Arrange
            var scanner = new DocumentScanner();
            EventHandler<PageScannedEventArgs> handler = (sender, e) => { };
            
            scanner.PageScanned += handler;
            scanner.PageScanned -= handler;

            // Act & Assert
            scanner.Dispose(); // Should not throw
        }

        #endregion

        #region Error Handling Tests

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

        [Fact]
        public async Task GetScanDevicesAsync_AfterDispose_ThrowsObjectDisposedException()
        {
            // Arrange
            var scanner = new DocumentScanner();
            scanner.Dispose();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ObjectDisposedException>(() => scanner.GetScanDevicesAsync());
            Assert.Equal(nameof(DocumentScanner), exception.ObjectName);
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_CreatesInstance_Successfully()
        {
            // Arrange & Act
            var scanner = new DocumentScanner();

            // Assert
            Assert.NotNull(scanner);
        }

        [Fact]
        public void Constructor_CreatesMultipleInstances_Successfully()
        {
            // Arrange & Act
            var scanner1 = new DocumentScanner();
            var scanner2 = new DocumentScanner();

            // Assert
            Assert.NotNull(scanner1);
            Assert.NotNull(scanner2);
            Assert.NotSame(scanner1, scanner2);
        }

        [Fact]
        public void Constructor_CreatesInstanceWithEventSupport()
        {
            // Arrange & Act
            var scanner = new DocumentScanner();
            var eventRaised = false;
            scanner.PageScanned += (sender, e) => eventRaised = true;

            // Assert
            Assert.NotNull(scanner);
            Assert.False(eventRaised);
        }

        #endregion

        #region PageScannedEventArgs Tests

        [Fact]
        public void PageScannedEventArgs_Constructor_SetsImageFilePath()
        {
            // Arrange
            var expectedPath = @"C:\temp\test.png";

            // Act
            var args = new PageScannedEventArgs(expectedPath);

            // Assert
            Assert.Equal(expectedPath, args.ImageFilePath);
        }

        [Fact]
        public void PageScannedEventArgs_WithEmptyPath_SetsImageFilePath()
        {
            // Arrange
            var expectedPath = "";

            // Act
            var args = new PageScannedEventArgs(expectedPath);

            // Assert
            Assert.Equal(expectedPath, args.ImageFilePath);
        }

        [Fact]
        public void PageScannedEventArgs_WithNullPath_SetsImageFilePath()
        {
            // Arrange
            string? expectedPath = null;

            // Act
            var args = new PageScannedEventArgs(expectedPath!);

            // Assert
            Assert.Equal(expectedPath, args.ImageFilePath);
        }

        [Fact]
        public void PageScannedEventArgs_WithUnicodePath_SetsImageFilePath()
        {
            // Arrange
            var expectedPath = @"C:\temp\документ-文档.png";

            // Act
            var args = new PageScannedEventArgs(expectedPath);

            // Assert
            Assert.Equal(expectedPath, args.ImageFilePath);
        }

        [Fact]
        public void PageScannedEventArgs_WithNetworkPath_SetsImageFilePath()
        {
            // Arrange
            var expectedPath = @"\\server\share\test.png";

            // Act
            var args = new PageScannedEventArgs(expectedPath);

            // Assert
            Assert.Equal(expectedPath, args.ImageFilePath);
        }

        #endregion
    }
} 