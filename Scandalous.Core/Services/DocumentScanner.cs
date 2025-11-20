using NAPS2.Images;
using NAPS2.Images.Gdi;
using NAPS2.Ocr;
using NAPS2.Pdf;
using NAPS2.Scan;
using Scandalous.Core.Enums;
using Scandalous.Core.Models;
using System.Runtime.CompilerServices;

namespace Scandalous.Core.Services
{
    public class DocumentScanner : IDocumentScanner
    {
        private readonly ScanController _scanController;
        private readonly ScanningContext _scanningContext;
        public event EventHandler<PageScannedEventArgs>? PageScanned;
        private bool _disposed = false;

        public DocumentScanner()
        {
            _scanningContext = new ScanningContext(new GdiImageContext());
            _scanController = new ScanController(_scanningContext);
        }

        public async Task ScanDocuments(ScanConfiguration configuration, CancellationToken cancellationToken = default)
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

            var deviceList = await _scanController.GetDeviceList();
            var device = deviceList.FirstOrDefault(d => d.Name == configuration.SelectedScannerName) ?? throw new InvalidOperationException("The selected scanner is offline.");
            var options = PrepareScanOptions(device, configuration);
            List<ProcessedImage> processedImages = [];
            var imageFiles = new List<string>();

            try
            {
                (processedImages, imageFiles) = await PerformScanning(options, configuration.OutputFolder, cancellationToken);

                if (processedImages.Count > 0)
                {
                    await ExportImagesToPdfAsync(configuration, processedImages, cancellationToken);
                }
            }
            finally
            {
                CleanUpImageFiles(imageFiles, configuration.OutputFolder);
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

        private async Task<(List<ProcessedImage> scannedImages, List<string> tempFiles)> PerformScanning(ScanOptions scanOptions, string outputFolder, CancellationToken cancellationToken)
        {
            var images = new List<ProcessedImage>();
            var tempFiles = new List<string>();

            await foreach (var image in _scanController.Scan(scanOptions, cancellationToken).WithCancellation(cancellationToken))
            {
                images.Add(image);
                Guid guid = Guid.CreateVersion7();
                var outputFile = Path.Combine(outputFolder, $"scan-{guid}.png");
                tempFiles.Add(outputFile);
                image.Save(outputFile, ImageFileFormat.Png);
                OnPageScanned(outputFile);
            }
            return (images, tempFiles);
        }

        private async Task ExportImagesToPdfAsync(ScanConfiguration configuration, IList<ProcessedImage> processedImages, CancellationToken cancellationToken)
        {
            if (configuration.OcrEnabled)
            {
                _scanningContext.OcrEngine = TesseractOcrEngine.Bundled(configuration.TessdataFolder);
            }
            var pdfExporter = new PdfExporter(_scanningContext);
            if (configuration.DocumentOptions == DocumentOptions.Combined)
            {
                var outputFile = Path.Combine(configuration.OutputFolder, $"{configuration.OutputBaseFileName}.pdf");
                await ExportPdfAsync(pdfExporter, outputFile, processedImages, configuration.OcrEnabled);
            }
            else
            {
                foreach (var image in processedImages)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var individualPdfName = $"{configuration.OutputBaseFileName}-{Guid.NewGuid()}.pdf";
                    var outputFile = Path.Combine(configuration.OutputFolder, individualPdfName);
                    await ExportPdfAsync(pdfExporter, outputFile, [image], configuration.OcrEnabled);
                }
            }
        }

        private static async Task ExportPdfAsync(PdfExporter pdfExporter, string outputFile, IList<ProcessedImage> images, bool ocrEnabled)
        {
            if (ocrEnabled)
            {
                await pdfExporter.Export(outputFile, images, ocrParams: new OcrParams("eng"));
            }
            else
            {
                await pdfExporter.Export(outputFile, images);
            }
        }

        private static void CleanUpImageFiles(IList<string> imageFiles, string folder)
        {
            foreach (var file in imageFiles)
            {
                try
                {
                    var filePath = Path.Combine(folder, file);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
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
            
            var deviceList = await _scanController.GetDeviceList();
            return deviceList;
        }

        protected virtual void OnPageScanned(string imageFilePath)
        {
            PageScanned?.Invoke(this, new PageScannedEventArgs(imageFilePath));
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(DocumentScanner));
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