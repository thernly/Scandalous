using NAPS2.Scan.Exceptions;

namespace Scandalous.Core.Services
{
    public class ScanExceptionHandler : IScanExceptionHandler
    {
        public ScanExceptionResult HandleScanException(Exception ex)
        {
            return ex switch
            {
                DeviceFeederEmptyException => new ScanExceptionResult
                {
                    IsHandled = true,
                    UserMessage = "The device feeder is empty.",
                    ShouldRetry = true,
                    OriginalException = ex
                },
                ScanDriverUnknownException => new ScanExceptionResult
                {
                    IsHandled = true,
                    UserMessage = "The scan driver is unknown. Please check the scanner connection and drivers.",
                    ShouldRetry = false,
                    OriginalException = ex
                },
                _ => new ScanExceptionResult
                {
                    IsHandled = false,
                    UserMessage = $"An error occurred: {ex.Message}",
                    ShouldRetry = false,
                    OriginalException = ex
                }
            };
        }

        public string GetUserFriendlyMessage(Exception ex)
        {
            return HandleScanException(ex).UserMessage;
        }

        public bool IsRecoverableException(Exception ex)
        {
            return HandleScanException(ex).ShouldRetry;
        }
    }
} 