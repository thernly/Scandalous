namespace ScanUtility
{
    public class PageScannedEventArgs : EventArgs
    {
        public string ImageFilePath { get; }

        public PageScannedEventArgs(string imageFilePath)
        {
            ImageFilePath = imageFilePath;
        }
    }
}
