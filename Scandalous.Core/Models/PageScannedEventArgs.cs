namespace Scandalous.Core.Models
{
    public class PageScannedEventArgs(string imageFilePath) : EventArgs
    {
        public string ImageFilePath { get; } = imageFilePath;
    }
} 