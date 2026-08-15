using Scandalous.Core.Services;

namespace Scandalous.Core.Tests.Services;

public class PdfServiceTests
{
    [Fact]
    public void OpenOutputFolder_WhenPathEqualsExpectedOutputFolder_UsesFolderValidationInsteadOfContainmentFailure()
    {
        var baseDir = Directory.CreateTempSubdirectory("scandalous-output-").FullName;
        var missingOutputDir = Path.Combine(baseDir, "missing-output");
        var sut = new PdfService();

        try
        {
            var ex = Assert.Throws<InvalidOperationException>(() => sut.OpenOutputFolder(missingOutputDir, missingOutputDir));
            Assert.Contains("not a valid output folder", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.Delete(baseDir, recursive: true);
        }
    }

    [Fact]
    public void OpenOutputFolder_WhenExpectedFolderHasTrailingSeparator_DoesNotFailContainmentCheck()
    {
        var baseDir = Directory.CreateTempSubdirectory("scandalous-output-").FullName;
        var missingOutputDir = Path.Combine(baseDir, "missing-output");
        var expectedWithTrailingSeparator = missingOutputDir + Path.DirectorySeparatorChar;
        var sut = new PdfService();

        try
        {
            var ex = Assert.Throws<InvalidOperationException>(() => sut.OpenOutputFolder(missingOutputDir, expectedWithTrailingSeparator));
            Assert.Contains("not a valid output folder", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.Delete(baseDir, recursive: true);
        }
    }

    [Fact]
    public void OpenPdfFile_WhenPathIsOutsideExpectedOutputFolder_Throws()
    {
        var outputDir = Directory.CreateTempSubdirectory("scandalous-output-").FullName;
        var outsideDir = Directory.CreateTempSubdirectory("scandalous-outside-").FullName;
        var outsidePdf = Path.Combine(outsideDir, "report.pdf");
        File.WriteAllBytes(outsidePdf, [1]);
        var sut = new PdfService();

        try
        {
            var ex = Assert.Throws<InvalidOperationException>(() => sut.OpenPdfFile(outsidePdf, outputDir));
            Assert.Contains("expected output directory", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.Delete(outputDir, recursive: true);
            Directory.Delete(outsideDir, recursive: true);
        }
    }

    [Fact]
    public void OpenOutputFolder_WhenPathIsOutsideExpectedOutputFolder_Throws()
    {
        var outputDir = Directory.CreateTempSubdirectory("scandalous-output-").FullName;
        var outsideDir = Directory.CreateTempSubdirectory("scandalous-outside-").FullName;
        var sut = new PdfService();

        try
        {
            var ex = Assert.Throws<InvalidOperationException>(() => sut.OpenOutputFolder(outsideDir, outputDir));
            Assert.Contains("expected output directory", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.Delete(outputDir, recursive: true);
            Directory.Delete(outsideDir, recursive: true);
        }
    }
}
