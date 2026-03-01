using Scandalous.Core.Models;

namespace Scandalous.Core.Services
{
    public interface IPdfService
    {
        string GetPdfFilePath(ScanConfiguration configuration);
        void OpenPdfFile(string pdfFilePath, string expectedOutputFolder);
        bool PdfFileExists(string pdfFilePath);
    }
} 