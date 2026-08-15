using Scandalous.Core.Models;
using Scandalous.Core.Services;
using Xunit;

namespace Scandalous.Core.Tests.Services
{
    public class WindowBoundsNormalizerTests
    {
        private const double MinWidth = 800;
        private const double MinHeight = 600;

        private static readonly ScreenArea[] SingleScreen = { new(0, 0, 1920, 1080) };

        private static NormalizedWindowBounds Normalize(
            WindowStateInfo saved,
            params ScreenArea[] screens) =>
            WindowBoundsNormalizer.Normalize(saved, screens, MinWidth, MinHeight);

        [Fact]
        public void MinimizedStateIsNormalizedToNormal()
        {
            var result = Normalize(
                new WindowStateInfo { Width = 1000, Height = 700, Left = 100, Top = 100, State = WindowState.Minimized },
                SingleScreen);

            Assert.Equal(WindowState.Normal, result.State);
            Assert.Equal(100, result.Left);
            Assert.Equal(100, result.Top);
        }

        [Fact]
        public void ValidMaximizedStateIsPreserved()
        {
            var result = Normalize(
                new WindowStateInfo { Width = 1000, Height = 700, Left = 100, Top = 100, State = WindowState.Maximized },
                SingleScreen);

            Assert.Equal(WindowState.Maximized, result.State);
            Assert.False(result.CenterOnScreen);
        }

        [Fact]
        public void ValidBoundsOnSingleScreenAreApplied()
        {
            var result = Normalize(
                new WindowStateInfo { Width = 1000, Height = 700, Left = 200, Top = 150 },
                SingleScreen);

            Assert.Equal(1000, result.Width);
            Assert.Equal(700, result.Height);
            Assert.Equal(200, result.Left);
            Assert.Equal(150, result.Top);
            Assert.False(result.CenterOnScreen);
        }

        [Fact]
        public void OversizedDimensionsAreClampedToWorkingArea()
        {
            var result = Normalize(
                new WindowStateInfo { Width = 4000, Height = 3000, Left = 0, Top = 0 },
                SingleScreen);

            Assert.Equal(1920, result.Width);
            Assert.Equal(1080, result.Height);
        }

        [Fact]
        public void UndersizedDimensionsAreClampedToMinimums()
        {
            var result = Normalize(
                new WindowStateInfo { Width = 120, Height = 90, Left = 0, Top = 0 },
                SingleScreen);

            Assert.Equal(MinWidth, result.Width);
            Assert.Equal(MinHeight, result.Height);
        }

        [Fact]
        public void PositionFromRemovedMonitorFallsBackToCenter()
        {
            var result = Normalize(
                new WindowStateInfo { Width = 1000, Height = 700, Left = 3000, Top = 200 },
                SingleScreen);

            Assert.True(result.CenterOnScreen);
            Assert.Null(result.Left);
            Assert.Null(result.Top);
        }

        [Fact]
        public void PositionOnSecondScreenIsAppliedAndClampedToThatScreen()
        {
            var result = Normalize(
                new WindowStateInfo { Width = 2000, Height = 700, Left = 1920, Top = 0 },
                new ScreenArea(0, 0, 1920, 1080),
                new ScreenArea(1920, 0, 1280, 1024));

            Assert.Equal(1280, result.Width);
            Assert.Equal(1920, result.Left);
        }

        [Fact]
        public void PartiallyVisiblePositionWithEnoughOverlapIsKept()
        {
            // 150 logical pixels of the window remain on screen horizontally.
            var result = Normalize(
                new WindowStateInfo { Width = 1000, Height = 700, Left = 1770, Top = 950 },
                SingleScreen);

            Assert.False(result.CenterOnScreen);
            Assert.Equal(1770, result.Left);
        }

        [Fact]
        public void PartiallyVisiblePositionWithTooLittleOverlapIsRejected()
        {
            // Only 50 logical pixels remain on screen horizontally.
            var result = Normalize(
                new WindowStateInfo { Width = 1000, Height = 700, Left = 1870, Top = 200 },
                SingleScreen);

            Assert.True(result.CenterOnScreen);
        }

        [Fact]
        public void MalformedBoundsFallBackToCenteredDefaults()
        {
            var result = Normalize(
                new WindowStateInfo
                {
                    Width = double.NaN,
                    Height = -10,
                    Left = double.PositiveInfinity,
                    Top = double.NaN,
                    State = WindowState.Maximized
                },
                SingleScreen);

            Assert.True(result.CenterOnScreen);
            Assert.Equal(WindowState.Normal, result.State);
            Assert.Equal(1200, result.Width);
            Assert.Equal(800, result.Height);
        }

        [Fact]
        public void UnsetPositionCentersWithoutDiscardingSize()
        {
            var result = Normalize(
                new WindowStateInfo { Width = 1000, Height = 700 },
                SingleScreen);

            Assert.True(result.CenterOnScreen);
            Assert.Equal(1000, result.Width);
            Assert.Equal(700, result.Height);
        }

        [Fact]
        public void NoScreensAvailableCentersAndStillClampsToMinimums()
        {
            var result = WindowBoundsNormalizer.Normalize(
                new WindowStateInfo { Width = 100, Height = 100, Left = 10, Top = 10 },
                new List<ScreenArea>(),
                MinWidth,
                MinHeight);

            Assert.True(result.CenterOnScreen);
            Assert.Equal(MinWidth, result.Width);
            Assert.Equal(MinHeight, result.Height);
        }
    }
}
