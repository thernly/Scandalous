namespace Scandalous.Core.Models
{
    public sealed class ScanResult
    {
        public int CapturedPageCount { get; init; }

        public IReadOnlyList<string> OutputFiles { get; init; } = [];
    }
}