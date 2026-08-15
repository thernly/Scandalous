using Scandalous.Core.Enums;

namespace Scandalous.Core.Tests.Enums
{
    public class ScannerPaperSizeTests
    {
        [Fact]
        public void ScannerPaperSize_ContainsExpectedValues()
        {
            var values = Enum.GetValues<ScannerPaperSize>();

            Assert.Equal(3, values.Length);
            Assert.Contains(ScannerPaperSize.Letter, values);
            Assert.Contains(ScannerPaperSize.A4, values);
            Assert.Contains(ScannerPaperSize.Legal, values);
        }

        [Fact]
        public void ScannerPaperSize_ValuesHaveCorrectUnderlyingIntegers()
        {
            Assert.Equal(0, (int)ScannerPaperSize.Letter);
            Assert.Equal(1, (int)ScannerPaperSize.A4);
            Assert.Equal(2, (int)ScannerPaperSize.Legal);
        }
    }
}