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
                DeviceBusyException => new ScanExceptionResult
                {
                    IsHandled = true,
                    UserMessage = "The device is currently busy. Please wait and try again.",
                    ShouldRetry = true,
                    OriginalException = ex
                },
                DeviceCommunicationException => new ScanExceptionResult 
                {
                    IsHandled = true,
                    UserMessage = "There was a communication error with the device. Please check the scanner connection and try again.",
                    ShouldRetry = true,
                    OriginalException = ex
                },
                DeviceCoverOpenException => new ScanExceptionResult 
                {
                    IsHandled = true,
                    UserMessage = "The device cover is open. Please close the cover and try again.",
                    ShouldRetry = true,
                    OriginalException = ex
                },
                DeviceNotFoundException => new ScanExceptionResult 
                {
                    IsHandled = true,
                    UserMessage = "The device was not found. Please check the scanner connection and try again.",
                    ShouldRetry = true,
                    OriginalException = ex
                },
                DevicePaperJamException => new ScanExceptionResult 
                {
                    IsHandled = true,
                    UserMessage = "There is a paper jam in the device. Please clear the jam and try again.",
                    ShouldRetry = true,
                    OriginalException = ex
                },
                DeviceWarmingUpException => new ScanExceptionResult 
                {
                    IsHandled = true,
                    UserMessage = "The device is warming up. Please wait and try again.",
                    ShouldRetry = true,
                    OriginalException = ex
                },
                DriverNotSupportedException => new ScanExceptionResult 
                {
                    IsHandled = true,
                    UserMessage = "The scan driver is not supported. Please check the scanner drivers and try again.",
                    ShouldRetry = false,
                    OriginalException = ex
                },
                NoDuplexSupportException => new ScanExceptionResult 
                {
                    IsHandled = true,
                    UserMessage = "The device does not support duplex scanning. Please disable duplex mode and try again.",
                    ShouldRetry = true,
                    OriginalException = ex
                },
                NoFeederSupportException => new ScanExceptionResult 
                {
                    IsHandled = true,
                    UserMessage = "The device does not support feeder scanning. Please disable feeder mode and try again.",
                    ShouldRetry = true,
                    OriginalException = ex
                },
                _ => new ScanExceptionResult
                {
                    IsHandled = false,
                    UserMessage = $"An error occurred. Please check the log file.",
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