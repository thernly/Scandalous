using NAPS2.Scan;
using Scandalous.Core.Models;

namespace Scandalous.Core.Services
{
    public interface IDocumentScanner : IDisposable
    {
        event EventHandler<PageScannedEventArgs>? PageScanned;
        Task ScanDocuments(ScanConfiguration configuration);
        Task<List<ScanDevice>> GetScanDevicesAsync();
    }
} 