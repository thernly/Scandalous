using NAPS2.Scan;
using Scandalous.Core.Models;

namespace Scandalous.Core.Services
{
    public interface IDocumentScanner : IDisposable
    {
        event EventHandler<PageScannedEventArgs>? PageScanned;
        Task<string> ScanDocuments(ScanConfiguration configuration, CancellationToken cancellationToken = default, Func<Task<bool>>? promptForMorePages = null);
        Task<List<ScanDevice>> GetScanDevicesAsync();
    }
} 