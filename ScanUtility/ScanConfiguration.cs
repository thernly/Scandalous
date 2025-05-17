namespace ScanUtility
{
    public class ScanConfiguration
    {
        public ScanConfiguration() {
            // Default constructor for deserialization.
        }
        public ScanConfiguration(string folder, string baseFileName, ScannerColorMode colorMode = ScannerColorMode.Grayscale,
                                 DocumentOptions documentOptions = DocumentOptions.Combined, bool autoDeskew = true,
                                 bool excludeBlankPages = true, int scanResolutionDpi = 300, ScannerPaperSource scannerPaperSource = ScannerPaperSource.Auto)
        {
            // Validate folder first
            FolderValidator.Validate(folder); 
            OutputFolder = folder;

            // Validate base file name (without extension)
            FileNameValidator.Validate(baseFileName, isBaseNameOnly: true);
            OutputBaseFileName = baseFileName;
            
            ColorMode = colorMode;
            DocumentOptions = documentOptions;
            AutoDeskew = autoDeskew;
            ExcludeBlankPages = excludeBlankPages;
            ScanResolutionDPI = scanResolutionDpi;
            this.ScannerPaperSource = scannerPaperSource;
        }
        public string OutputFolder { get; set; } = string.Empty;
        public string OutputBaseFileName { get; set; } = string.Empty;
        public ScannerColorMode ColorMode { get; set; } = ScannerColorMode.Grayscale;
        public DocumentOptions DocumentOptions { get; set; } = DocumentOptions.Combined;
        public bool AutoDeskew { get; set; } = true;
        public bool ExcludeBlankPages { get; set; } = true;
        public int ScanResolutionDPI { get; set; } = 300;
        public ScannerPaperSource ScannerPaperSource { get; set; } = ScannerPaperSource.Auto;
    }
}
