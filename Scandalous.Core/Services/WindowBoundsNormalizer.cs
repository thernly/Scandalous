using System;
using System.Collections.Generic;
using Scandalous.Core.Models;

namespace Scandalous.Core.Services
{
    /// <summary>
    /// A screen working area in logical pixels.
    /// </summary>
    public readonly record struct ScreenArea(double Left, double Top, double Width, double Height)
    {
        public double Right => Left + Width;
        public double Bottom => Top + Height;
    }

    /// <summary>
    /// Window bounds that are safe to apply to a real window.
    /// </summary>
    /// <param name="Left">Position to apply, or null when the window should be centered.</param>
    public sealed record NormalizedWindowBounds(
        double Width,
        double Height,
        double? Left,
        double? Top,
        WindowState State)
    {
        /// <summary>
        /// True when no saved position could be used and the window should be centered.
        /// </summary>
        public bool CenterOnScreen => Left is null || Top is null;
    }

    /// <summary>
    /// Platform-independent normalization of persisted window bounds. Prevents saved minimized
    /// or off-screen state from making the application appear missing.
    /// </summary>
    public static class WindowBoundsNormalizer
    {
        /// <summary>
        /// Minimum logical pixels of the window that must intersect a screen working area for a
        /// saved position to be reused.
        /// </summary>
        public const double MinimumVisibleExtent = 100;

        public static NormalizedWindowBounds Normalize(
            WindowStateInfo? saved,
            IReadOnlyList<ScreenArea> screens,
            double minimumWidth,
            double minimumHeight)
        {
            var defaults = new WindowStateInfo();
            var hasValidSize = saved != null
                && IsUsable(saved.Width) && saved.Width > 0
                && IsUsable(saved.Height) && saved.Height > 0;

            var width = hasValidSize ? saved!.Width : defaults.Width;
            var height = hasValidSize ? saved!.Height : defaults.Height;

            // Minimized must never be restored; a malformed size means we cannot trust the
            // rest of the saved bounds either, so fall back to a normal centered window.
            var state = saved != null && saved.State == WindowState.Maximized && hasValidSize
                ? WindowState.Maximized
                : WindowState.Normal;

            var hasValidPosition = saved != null && IsUsable(saved.Left) && IsUsable(saved.Top);
            var screen = SelectScreen(screens, hasValidPosition ? saved : null, width, height);

            if (screen != null)
            {
                width = Math.Min(width, screen.Value.Width);
                height = Math.Min(height, screen.Value.Height);
            }

            width = Math.Max(width, minimumWidth);
            height = Math.Max(height, minimumHeight);

            if (hasValidPosition && IsSufficientlyVisible(saved!.Left, saved.Top, width, height, screens))
                return new NormalizedWindowBounds(width, height, saved.Left, saved.Top, state);

            return new NormalizedWindowBounds(width, height, null, null, state);
        }

        private static bool IsUsable(double value) => !double.IsNaN(value) && !double.IsInfinity(value);

        private static ScreenArea? SelectScreen(
            IReadOnlyList<ScreenArea> screens,
            WindowStateInfo? positioned,
            double width,
            double height)
        {
            if (screens.Count == 0) return null;
            if (positioned == null) return screens[0];

            ScreenArea best = screens[0];
            double bestOverlap = -1;
            foreach (var screen in screens)
            {
                var overlap = Overlap(positioned.Left, width, screen.Left, screen.Width)
                    * Overlap(positioned.Top, height, screen.Top, screen.Height);
                if (overlap > bestOverlap)
                {
                    bestOverlap = overlap;
                    best = screen;
                }
            }

            return best;
        }

        private static bool IsSufficientlyVisible(
            double left,
            double top,
            double width,
            double height,
            IReadOnlyList<ScreenArea> screens)
        {
            foreach (var screen in screens)
            {
                if (Overlap(left, width, screen.Left, screen.Width) >= MinimumVisibleExtent
                    && Overlap(top, height, screen.Top, screen.Height) >= MinimumVisibleExtent)
                {
                    return true;
                }
            }

            return false;
        }

        private static double Overlap(double start, double length, double otherStart, double otherLength) =>
            Math.Max(0, Math.Min(start + length, otherStart + otherLength) - Math.Max(start, otherStart));
    }
}
