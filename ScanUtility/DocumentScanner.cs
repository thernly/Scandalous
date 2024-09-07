using NAPS2.Scan;
using NAPS2.Images.Gdi;
using NAPS2.Images;


namespace WinFormsApp1;

public enum ScannerColorMode
{
    Grayscale,
    BlackAndWhite,
    Color
}

public class DocumentScanner
{
    private readonly ScanController _scanController;
    public event EventHandler<PageScannedEventArgs>? PageScanned;

    public DocumentScanner()
    {
        var _scanningContext = new ScanningContext(new GdiImageContext());
        _scanController = new ScanController(_scanningContext);
    }
        
    public async Task ScanDocumentsFromFeeder(string outputFolder, ScannerColorMode mode)
    {

        var device = await GetFirstAvailableDevice() ?? throw new InvalidOperationException("No scanner device available.");
        var options = GetScanOptions(device, mode);
        await foreach (var image in _scanController.Scan(options))
        {
            Guid guid = Guid.NewGuid();
            var outputFile = Path.Combine(outputFolder, $"scan-{guid}.png");
            image.Save(outputFile, ImageFileFormat.Png);
            OnPageScanned(outputFile);
        }


    }

    private static ScanOptions GetScanOptions(ScanDevice device, ScannerColorMode mode)
    {
        var options = new ScanOptions
        {
            Device = device,
            PaperSource = PaperSource.Feeder,
            PageSize = PageSize.Letter,
            Dpi = 300,
            BitDepth = GetBitDepth(mode)
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

    protected virtual void OnPageScanned(string imageFilePath)
    {
        PageScanned?.Invoke(this, new PageScannedEventArgs(imageFilePath));
    }

        
}