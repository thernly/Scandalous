using NAPS2.Images;
using NAPS2.Ocr;
using NAPS2.Pdf;
using NAPS2.Scan;
using Scandalous.Core.Enums;
using Scandalous.Core.Models;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Scandalous.Core.Services
{
    public class DocumentScanner : IDocumentScanner
    {
        private readonly ScanController _scanController;
        private readonly ScanningContext _scanningContext;
        public event EventHandler<PageScannedEventArgs>? PageScanned;
        private bool _disposed = false;

        public DocumentScanner(ImageContext imageContext)
        {
            _scanningContext = new ScanningContext(imageContext);
            _scanController = new ScanController(_scanningContext);
        }

        public async Task<ScanResult> ScanDocuments(ScanConfiguration configuration, CancellationToken cancellationToken = default, Func<Task<bool>>? promptForMorePages = null)
        {
            ThrowIfDisposed();

            ArgumentNullException.ThrowIfNull(configuration);

            if (string.IsNullOrWhiteSpace(configuration.OutputFolder))
            {
                throw new ArgumentException("Output folder cannot be null, empty, or whitespace.", nameof(configuration));
            }

            if (string.IsNullOrWhiteSpace(configuration.OutputBaseFileName))
            {
                throw new ArgumentException("Output base file name cannot be null, empty, or whitespace.", nameof(configuration));
            }

            // Re-discover the scanner at scan time to get a fresh ScanDevice handle.
            var deviceList = await DiscoverDevicesAsync(TimeSpan.FromSeconds(10), cancellationToken);
            var device = deviceList.FirstOrDefault(d => d.Name == configuration.SelectedScannerName);

            if (device == null)
            {
                // Fall back to a longer discovery timeout.
                deviceList = await DiscoverDevicesAsync(TimeSpan.FromSeconds(20), cancellationToken);
                device = deviceList.FirstOrDefault(d => d.Name == configuration.SelectedScannerName);
            }

            if (device == null)
                throw new InvalidOperationException("The selected scanner is offline.");
            var options = PrepareScanOptions(device, configuration);
            List<ProcessedImage> processedImages = [];
            var imageFiles = new List<string>();
            var outputFiles = new List<string>();
            try
            {
                (var batch, var batchFiles) = await PerformScanning(options, cancellationToken);
                processedImages.AddRange(batch);
                imageFiles.AddRange(batchFiles);

                bool isFlatbedCombined = configuration.ScannerPaperSource == ScannerPaperSource.Flatbed
                    && configuration.DocumentOptions == DocumentOptions.Combined
                    && promptForMorePages != null;

                while (isFlatbedCombined && await promptForMorePages!())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    (var moreBatch, var moreBatchFiles) = await PerformScanning(options, cancellationToken);
                    processedImages.AddRange(moreBatch);
                    imageFiles.AddRange(moreBatchFiles);
                }

                if (processedImages.Count > 0)
                {
                    outputFiles = await ExportImagesToPdfAsync(configuration, processedImages, cancellationToken);
                }

                return new ScanResult
                {
                    CapturedPageCount = processedImages.Count,
                    OutputFiles = outputFiles
                };
            }
            finally
            {
                CleanUpImageFiles(imageFiles);
                DisposeImages(processedImages);
            }
        }

        private static ScanOptions PrepareScanOptions(ScanDevice device, ScanConfiguration configuration)
        {
            var options = GetScanOptions(device, configuration.ColorMode, configuration.ScannerPaperSource);
            options.AutoDeskew = configuration.AutoDeskew;
            options.ExcludeBlankPages = configuration.ExcludeBlankPages;
            options.Dpi = configuration.ScanResolutionDPI;
            return options;
        }

        private async Task<(List<ProcessedImage> scannedImages, List<string> tempFiles)> PerformScanning(ScanOptions scanOptions, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var images = new List<ProcessedImage>();
            var tempFiles = new List<string>();
            var tempFolder = Path.GetTempPath();

            await foreach (var image in _scanController.Scan(scanOptions, cancellationToken).WithCancellation(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                images.Add(image);
                Guid guid = Guid.CreateVersion7();
                var outputFile = Path.Combine(tempFolder, $"scan-{guid}.png");
                tempFiles.Add(outputFile);
                image.Save(outputFile, ImageFileFormat.Png);
                OnPageScanned(outputFile);
            }
            return (images, tempFiles);
        }

        private async Task<List<string>> ExportImagesToPdfAsync(ScanConfiguration configuration, IList<ProcessedImage> processedImages, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (configuration.OcrEnabled)
            {
                _scanningContext.OcrEngine = TesseractOcrEngine.Bundled(configuration.TessdataFolder);
            }
            var pdfExporter = new PdfExporter(_scanningContext);
            if (configuration.DocumentOptions == DocumentOptions.Combined)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var outputFile = GetAvailableFilePath(configuration.OutputFolder, configuration.OutputBaseFileName);
                await ExportPdfAsync(pdfExporter, outputFile, processedImages, configuration.OcrEnabled, configuration.TessdataLanguageCode);
                return [outputFile];
            }
            else
            {
                var outputFiles = new List<string>(processedImages.Count);
                foreach (var image in processedImages)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var outputFile = GetAvailableFilePath(configuration.OutputFolder, configuration.OutputBaseFileName);
                    await ExportPdfAsync(pdfExporter, outputFile, [image], configuration.OcrEnabled, configuration.TessdataLanguageCode);
                    outputFiles.Add(outputFile);
                }
                return outputFiles;
            }
        }

        private static string GetAvailableFilePath(string folder, string baseName)
        {
            var candidate = Path.Combine(folder, $"{baseName}.pdf");
            if (!File.Exists(candidate))
                return candidate;

            int counter = 2;
            while (true)
            {
                candidate = Path.Combine(folder, $"{baseName}_{counter}.pdf");
                if (!File.Exists(candidate))
                    return candidate;
                counter++;
            }
        }

        private static async Task ExportPdfAsync(PdfExporter pdfExporter, string outputFile, IList<ProcessedImage> images, bool ocrEnabled, string languageCode)
        {
            if (ocrEnabled)
            {
                var selectedLanguage = string.IsNullOrWhiteSpace(languageCode) ? "eng" : languageCode;
                await pdfExporter.Export(outputFile, images, ocrParams: new OcrParams(selectedLanguage));
            }
            else
            {
                await pdfExporter.Export(outputFile, images);
            }
        }

        private static void CleanUpImageFiles(IList<string> imageFiles)
        {
            foreach (var file in imageFiles)
            {
                try
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
                catch (IOException ex)
                {
                    // Log or handle file deletion errors if necessary
                    System.Diagnostics.Debug.WriteLine($"Error deleting temporary file {file}: {ex.Message}");
                }
            }

        }
        private static void DisposeImages(List<ProcessedImage> images)
        {
            foreach (var image in images)
            {
                (image as IDisposable)?.Dispose();
            }
            images.Clear();
        }


        private static ScanOptions GetScanOptions(ScanDevice device, ScannerColorMode colorMode, ScannerPaperSource scannerPaperSource)
        {
            var options = new ScanOptions
            {
                Device = device,
                PaperSource = scannerPaperSource switch
                {
                    ScannerPaperSource.Flatbed => PaperSource.Flatbed,
                    ScannerPaperSource.FeederSimplex => PaperSource.Feeder,
                    ScannerPaperSource.FeederDuplex => PaperSource.Duplex,
                    _ => PaperSource.Auto // Default to Auto if unspecified or for ScannerPaperSource.Auto
                },
                PageSize = PageSize.Letter, // Consider making this configurable
                Dpi = 300, // Default DPI, overridden by configuration.ScanResolutionDPI
                BitDepth = GetBitDepth(colorMode),
            };

            return options;
        }

        private static BitDepth GetBitDepth(ScannerColorMode mode) => mode switch
        {
            ScannerColorMode.Grayscale => BitDepth.Grayscale,
            ScannerColorMode.BlackAndWhite => BitDepth.BlackAndWhite,
            ScannerColorMode.Color => BitDepth.Color,
            _ => BitDepth.Grayscale // Default case
        };

        private async Task<ScanDevice?> GetFirstAvailableDevice() // Return nullable if no device is an acceptable state before throwing
        {
            var deviceList = await _scanController.GetDeviceList();
            return deviceList.FirstOrDefault(); // Simplified
        }

        public async Task<List<ScanDevice>> GetScanDevicesAsync()
        {
            ThrowIfDisposed();
            return await DiscoverDevicesAsync(TimeSpan.FromSeconds(3));
        }

        private async Task<List<ScanDevice>> DiscoverDevicesAsync(
            TimeSpan timeout, CancellationToken ct = default)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return await MacEsclDiscoveryService.DiscoverAsync(timeout, ct);

            var driver = GetPlatformDriver();
            var devices = new List<ScanDevice>();
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(timeout);
            try
            {
                await foreach (var device in _scanController.GetDevices(driver, cts.Token))
                    devices.Add(device);
            }
            catch (OperationCanceledException)
            {
                // Timeout expired — return whatever was found
            }
            return devices;
        }

        private static Driver GetPlatformDriver()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return Driver.Default;  // WIA
            return Driver.Sane;         // SANE airscan backend on Linux
        }

        protected virtual void OnPageScanned(string imageFilePath)
        {
            PageScanned?.Invoke(this, new PageScannedEventArgs(imageFilePath));
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DocumentScanner));
        }

        // IDisposable Implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing && _scanningContext is IDisposable disposableContext)
            {
                disposableContext.Dispose();
            }
            // Free unmanaged resources (unmanaged objects) and override a finalizer below.
            // Set large fields to null.
            _disposed = true;
        }

        // Override finalizer only if Dispose(bool disposing) has code to free unmanaged resources.
        ~DocumentScanner()
        {
            Dispose(false);
        }
    }
}