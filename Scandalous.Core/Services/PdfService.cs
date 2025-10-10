using Scandalous.Core.Models;

namespace Scandalous.Core.Services
{
    public class PdfService : IPdfService
    {
        public string GetPdfFilePath(ScanConfiguration configuration)
        {
            return Path.Combine(configuration.OutputFolder, $"{configuration.OutputBaseFileName}.pdf");
        }

        public void OpenPdfFile(string pdfFilePath)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = pdfFilePath,
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