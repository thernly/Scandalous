using System.Windows;

namespace Scandalous.Core.Models
{
    /// <summary>
    /// Represents the state of a WPF window for persistence
    /// </summary>
    public class WindowStateInfo
    {
        /// <summary>
        /// Window width in pixels
        /// </summary>
        public double Width { get; set; } = 1200;

        /// <summary>
        /// Window height in pixels
        /// </summary>
        public double Height { get; set; } = 800;

        /// <summary>
        /// Window left position in pixels (NaN means center on screen)
        /// </summary>
        public double Left { get; set; } = double.NaN;

        /// <summary>
        /// Window top position in pixels (NaN means center on screen)
        /// </summary>
        public double Top { get; set; } = double.NaN;

        /// <summary>
        /// Window state (Normal, Minimized, Maximized)
        /// </summary>
        public WindowState State { get; set; } = WindowState.Normal;

        /// <summary>
        /// Width of the left panel (for collapsible panels)
        /// </summary>
        public double LeftPanelWidth { get; set; } = 500;
    }

    /// <summary>
    /// Represents the state of a window
    /// </summary>
    public enum WindowState
    {
        /// <summary>
        /// Normal window state
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Minimized window state
        /// </summary>
        Minimized = 1,

        /// <summary>
        /// Maximized window state
        /// </summary>
        Maximized = 2
    }
} 