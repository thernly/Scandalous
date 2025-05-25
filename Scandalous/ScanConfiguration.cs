namespace ScanUtility
{
    public class ScanConfiguration
    {
        public ScanConfiguration() {
            // Default constructor for deserialization.
        }
        public ScanConfiguration(string outputFolder, string baseFileName, ScannerColorMode colorMode = ScannerColorMode.Grayscale,
                                 DocumentOptions documentOptions = DocumentOptions.Combined, bool autoDeskew = true,
                                 bool excludeBlankPages = true, int scanResolutionDpi = 300, ScannerPaperSource scannerPaperSource = ScannerPaperSource.Auto,
                                 bool ocrEnabled = false, string tessdataFolder = "")
        {            
            FolderValidator.Validate(outputFolder); 
            OutputFolder = outputFolder;

            // Validate base file name (without extension)
            FileNameValidator.Validate(baseFileName, isBaseNameOnly: true);
            OutputBaseFileName = baseFileName;
            
            ColorMode = colorMode;
            DocumentOptions = documentOptions;
            AutoDeskew = autoDeskew;
            ExcludeBlankPages = excludeBlankPages;
            ScanResolutionDPI = scanResolutionDpi;
            this.ScannerPaperSource = scannerPaperSource;
            
            OcrEnabled = ocrEnabled;
            FolderValidator.Validate(tessdataFolder); 
            TessdataFolder = tessdataFolder; 
        }
        public string OutputFolder { get; set; } = string.Empty;
        public string OutputBaseFileName { get; set; } = string.Empty;
        public ScannerColorMode ColorMode { get; set; } = ScannerColorMode.Grayscale;
        public DocumentOptions DocumentOptions { get; set; } = DocumentOptions.Combined;
        public bool AutoDeskew { get; set; } = true;
        public bool ExcludeBlankPages { get; set; } = true;
        public int ScanResolutionDPI { get; set; } = 300;
        public ScannerPaperSource ScannerPaperSource { get; set; } = ScannerPaperSource.Auto;
        public bool OcrEnabled { get; set; } = false; 
        public string TessdataFolder { get; set; } = string.Empty; // Path to Tesseract's tessdata folder, if OCR is enabled
    }
}
