using NAPS2.Images;
using NAPS2.Images.Gdi;
using NAPS2.Pdf;
using NAPS2.Scan;
using System.Runtime.CompilerServices;

namespace ScanUtility;

public enum ScannerColorMode
{
    Grayscale,
    BlackAndWhite,
    Color
}

public class DocumentScanner : IDisposable // Consider IDisposable
{
    private readonly ScanController _scanController;
    private readonly ScanningContext _scanningContext;
    public event EventHandler<PageScannedEventArgs>? PageScanned;
    private bool _disposed = false; // For IDisposable pattern

    public DocumentScanner()
    {
        _scanningContext = new ScanningContext(new GdiImageContext());
        _scanController = new ScanController(_scanningContext);
    }


    public async Task ScanDocuments(ScanConfiguration configuration)
    {
        var device = await GetFirstAvailableDevice() ?? throw new InvalidOperationException("No scanner device available.");
        var options = PrepareScanOptions(device, configuration);
        List<ProcessedImage> processedImages = []; 
        var imageFiles = new List<string>();

        try
        {
            (processedImages, imageFiles) = await PerformScanning(options, configuration.OutputFolder);

            if (processedImages.Count > 0) // Only export if there are images
            {
                await ExportImagesToPdfAsync(configuration, processedImages);
            }
        }
        finally
        {
            CleanUpImageFiles(imageFiles, configuration.OutputFolder);
            DisposeImages(processedImages); // Dispose images after exporting to PDF
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

    private async Task<(List<ProcessedImage> scannedImages, List<string> tempFiles)> PerformScanning(ScanOptions scanOptions, string outputFolder)
    {
        var images = new List<ProcessedImage>();
        var tempFiles = new List<string>();

        await foreach (var image in _scanController.Scan(scanOptions))
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

    private async Task ExportImagesToPdfAsync(ScanConfiguration configuration, IList<ProcessedImage> processedImages)
    {
        var pdfExporter = new PdfExporter(_scanningContext);
        if (configuration.DocumentOptions == DocumentOptions.Combined)
        {
            var outputFile = Path.Combine(configuration.OutputFolder, $"{configuration.OutputBaseFileName}.pdf");
            await pdfExporter.Export(outputFile, processedImages);
        }
        else
        {
            foreach (var image in processedImages)
            {
                // Ensure unique PDF filenames for individual files if base name is the same
                var individualPdfName = $"{configuration.OutputBaseFileName}-{Guid.NewGuid()}.pdf";
                var outputFile = Path.Combine(configuration.OutputFolder, individualPdfName);
                await pdfExporter.Export(outputFile, [image]);
            }
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
        var deviceList = await _scanController.GetDeviceList();
        return deviceList;
    }

    // Consider removing this synchronous version due to blocking .Result
    // public List<ScanDevice> GetScanDevices()
    // {
    //     var deviceList = _scanController.GetDeviceList().Result;
    //     return deviceList;
    // }

    protected virtual void OnPageScanned(string imageFilePath)
    {
        PageScanned?.Invoke(this, new PageScannedEventArgs(imageFilePath));
    }

    // IDisposable Implementation (if ScanningContext/ScanController are IDisposable)
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Dispose managed state (managed objects).
            // Example:
            // (_scanController as IDisposable)?.Dispose();
            // (_scanningContext as IDisposable)?.Dispose();
        }

        // Free unmanaged resources (unmanaged objects) and override a finalizer below.
        // Set large fields to null.
        _disposed = true;
    }

    // Override finalizer only if Dispose(bool disposing) has code to free unmanaged resources.
    // ~DocumentScanner()
    // {
    //     Dispose(false);
    // }
}