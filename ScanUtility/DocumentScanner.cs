using NAPS2.Scan;
using NAPS2.Images.Gdi;
using NAPS2.Images;
using NAPS2.Pdf;

namespace ScanUtility;

public enum ScannerColorMode
{
    Grayscale,
    BlackAndWhite,
    Color
}

public class DocumentScanner
{
    private readonly ScanController _scanController;
    private readonly ScanningContext _scanningContext;
    public event EventHandler<PageScannedEventArgs>? PageScanned;
    

    public DocumentScanner()
    {
        _scanningContext = new ScanningContext(new GdiImageContext());
        _scanController = new ScanController(_scanningContext);
    }
      
    public async Task ScanDocumentsFromFeeder(ScanConfiguration configuration)
    {
        var device = await GetFirstAvailableDevice() ?? throw new InvalidOperationException("No scanner device available.");
        var options = GetScanOptions(device, configuration.ColorMode, configuration.ScannerPaperSource);
        options.AutoDeskew = configuration.AutoDeskew;
        options.ExcludeBlankPages = configuration.ExcludeBlankPages;
        options.Dpi = configuration.ScanResolutionDPI;

        var images = new List<ProcessedImage>();
        var imageFiles = new List<string>();
        await foreach (var image in _scanController.Scan(options))
        {
            images.Add(image);
            Guid guid = Guid.CreateVersion7();
            var outputFile = Path.Combine(configuration.OutputFolder, $"scan-{guid}.png");
            imageFiles.Add(outputFile);
            image.Save(outputFile, ImageFileFormat.Png);
            OnPageScanned(outputFile);
        }

        var pdfExporter = new PdfExporter(_scanningContext);
        if (configuration.DocumentOptions == DocumentOptions.Combined)
        {
            var outputFile = Path.Combine(configuration.OutputFolder, $"{configuration.OutputBaseFileName}.pdf");
            await pdfExporter.Export(outputFile, images);

        }
        else
        {
            foreach (var image in images)
            {
                var outputFile = Path.Combine(configuration.OutputFolder, $"{configuration.OutputBaseFileName}-{Guid.NewGuid()}.pdf");
                await pdfExporter.Export(outputFile, [image]);
            }
        }

        foreach (var file in imageFiles)
        {
            File.Delete(file);
        }


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
                _ => PaperSource.Auto
            },
            PageSize = PageSize.Letter,
            Dpi = 300,
            BitDepth = GetBitDepth(colorMode),
        };

        return options;
    }

    private static BitDepth GetBitDepth(ScannerColorMode mode)
    {
        if (mode == ScannerColorMode.Grayscale) { return BitDepth.Grayscale; }
        if (mode == ScannerColorMode.BlackAndWhite) { return BitDepth.BlackAndWhite; }
        if (mode == ScannerColorMode.Color) { return BitDepth.Color; }
        return BitDepth.Grayscale;
    }

    private async Task<ScanDevice> GetFirstAvailableDevice()
    {
        var deviceList = await _scanController.GetDeviceList();
        return deviceList.FirstOrDefault<ScanDevice>() ?? throw new InvalidOperationException("No scanners found.");
    }

    public async Task<List<ScanDevice>> GetScanDevicesAsync()
    {
        var deviceList = await _scanController.GetDeviceList();
        return deviceList;
    }

    public List<ScanDevice> GetScanDevices()
    {
        var deviceList = _scanController.GetDeviceList().Result;
        return deviceList;
    }

    protected virtual void OnPageScanned(string imageFilePath)
    {
        PageScanned?.Invoke(this, new PageScannedEventArgs(imageFilePath));
    }

        
}