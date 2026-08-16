using System.Runtime.InteropServices;
using System.Diagnostics;
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
            var fullPdfPathParam = ValidatePathInsideOutputDirectory(pdfFilePath, expectedOutputFolder, "PDF file");

            if (!Path.GetExtension(fullPdfPathParam).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Execution target is not a valid PDF file.");
            }

            OpenPath(fullPdfPathParam, "PDF file");
        }

        public void OpenOutputFolder(string outputFolderPath, string expectedOutputFolder)
        {
            var fullFolderPath = ValidatePathInsideOutputDirectory(outputFolderPath, expectedOutputFolder, "output folder");

            if (!Directory.Exists(fullFolderPath))
            {
                throw new InvalidOperationException("Execution target is not a valid output folder.");
            }

            OpenPath(fullFolderPath, "output folder");
        }

        public bool PdfFileExists(string pdfFilePath)
        {
            return File.Exists(pdfFilePath);
        }

        private static string ValidatePathInsideOutputDirectory(string targetPath, string expectedOutputFolder, string targetDescription)
        {
            var fullTargetPath = Path.TrimEndingDirectorySeparator(Path.GetFullPath(targetPath));
            var fullOutputDir = Path.TrimEndingDirectorySeparator(Path.GetFullPath(expectedOutputFolder));
            var fullOutputDirWithSeparator = fullOutputDir + Path.DirectorySeparatorChar;

            var isExactMatch = string.Equals(fullTargetPath, fullOutputDir, StringComparison.OrdinalIgnoreCase);
            var isChildPath = fullTargetPath.StartsWith(fullOutputDirWithSeparator, StringComparison.OrdinalIgnoreCase);

            if (!isExactMatch && !isChildPath)
            {
                throw new InvalidOperationException($"Calculated {targetDescription} path does not reside in the expected output directory.");
            }

            return fullTargetPath;
        }

        private static void OpenPath(string path, string targetDescription)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true,
                        Verb = "open"
                    });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "open",
                        UseShellExecute = false
                    };
                    startInfo.ArgumentList.Add(path);
                    Process.Start(startInfo);
                }
                else // Linux
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "xdg-open",
                        UseShellExecute = false
                    };
                    startInfo.ArgumentList.Add(path);
                    Process.Start(startInfo);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PdfService] Failed to open {targetDescription}. Path: '{path}'. Exception: {ex}");
                throw new InvalidOperationException($"Failed to open {targetDescription}: {ex.Message}", ex);
            }
        }
    }
} 