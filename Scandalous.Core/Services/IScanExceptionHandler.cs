using NAPS2.Scan.Exceptions;

namespace Scandalous.Core.Services
{
    public interface IScanExceptionHandler
    {
        ScanExceptionResult HandleScanException(Exception ex);
        string GetUserFriendlyMessage(Exception ex);
        bool IsRecoverableException(Exception ex);
    }

    public class ScanExceptionResult
    {
        public bool IsHandled { get; set; }
        public string UserMessage { get; set; } = string.Empty;
        public bool ShouldRetry { get; set; }
        public Exception? OriginalException { get; set; }
    }
} 