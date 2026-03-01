using Scandalous.Core.Models;

namespace Scandalous.Core.Services
{
    public class PdfService : IPdfService
    {
        public string GetPdfFilePath(ScanConfiguration configuration)
        {
            return Path.Combine(configuration.OutputFolder, $"{configuration.OutputBaseFileName}.pdf");
        }

        public void OpenPdfFile(string pdfFilePath, string expectedOutputFolder)
        {
            var fullPdfPathParam = Path.GetFullPath(pdfFilePath);
            var fullOutputDir = Path.GetFullPath(expectedOutputFolder);

            if (!fullOutputDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                fullOutputDir += Path.DirectorySeparatorChar;
            }

            if (!fullPdfPathParam.StartsWith(fullOutputDir, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Calculated path does not reside in the expected output directory.");
            }

            if (!Path.GetExtension(fullPdfPathParam).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Execution target is not a valid PDF file.");
            }

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = fullPdfPathParam,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to open PDF file: {ex.Message}", ex);
            }
        }

        public bool PdfFileExists(string pdfFilePath)
        {
            return File.Exists(pdfFilePath);
        }
    }
} 