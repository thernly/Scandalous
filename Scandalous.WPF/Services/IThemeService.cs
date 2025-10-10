using System;
using System.Threading.Tasks;

namespace Scandalous.WPF.Services
{
    /// <summary>
    /// Service for managing application themes
    /// </summary>
    public interface IThemeService
    {
        /// <summary>
        /// Gets the current theme
        /// </summary>
        Theme CurrentTheme { get; }

        /// <summary>
        /// Event raised when the theme changes
        /// </summary>
        event EventHandler<ThemeChangedEventArgs> ThemeChanged;

        /// <summary>
        /// Switches to the specified theme
        /// </summary>
        /// <param name="theme">The theme to switch to</param>
        void SwitchTheme(Theme theme);

        /// <summary>
        /// Toggles between light and dark themes
        /// </summary>
        void ToggleTheme();

        /// <summary>
        /// Loads the saved theme preference
        /// </summary>
        Task LoadThemeAsync();

        /// <summary>
        /// Saves the current theme preference
        /// </summary>
        Task SaveThemeAsync();
    }

    /// <summary>
    /// Available themes
    /// </summary>
    public enum Theme
    {
        /// <summary>
        /// Light theme
        /// </summary>
        Light,

        /// <summary>
        /// Dark theme
        /// </summary>
        Dark,

        /// <summary>
        /// System theme (follows OS setting)
        /// </summary>
        System
    }

    /// <summary>
    /// Event arguments for theme changes
    /// </summary>
    public class ThemeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new theme
        /// </summary>
        public Theme NewTheme { get; }

        /// <summary>
        /// The previous theme
        /// </summary>
        public Theme PreviousTheme { get; }

        public ThemeChangedEventArgs(Theme newTheme, Theme previousTheme)
        {
            NewTheme = newTheme;
            PreviousTheme = previousTheme;
        }
    }
} 