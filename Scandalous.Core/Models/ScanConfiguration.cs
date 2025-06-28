using Scandalous.Core.Validation;
using Scandalous.Core.Enums;

namespace Scandalous.Core.Models
{
    public class ScanConfiguration
    {
        public ScanConfiguration() {
            // Default constructor for deserialization.
        }
        public ScanConfiguration(string outputFolder, string baseFileName, ScannerColorMode colorMode = ScannerColorMode.Grayscale,
                                 DocumentOptions documentOptions = DocumentOptions.Combined, bool autoDeskew = true,
                                 bool excludeBlankPages = true, int scanResolutionDpi = 300, ScannerPaperSource scannerPaperSource = ScannerPaperSource.Auto,
                                 bool ocrEnabled = false, string tessdataFolder = "", string languageCode = "eng")
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
            // Only validate tessdataFolder if it's not empty
            if (!string.IsNullOrWhiteSpace(tessdataFolder))
            {
                FolderValidator.Validate(tessdataFolder);
            }
            TessdataFolder = tessdataFolder;
            TessdataLanguageCode = languageCode;
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
        public string TessdataLanguageCode { get; set; } = "eng"; // Default language code for Tesseract OCR
    }
} 