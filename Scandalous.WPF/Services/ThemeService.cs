using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.IO;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using Microsoft.Win32;
using MaterialDesignThemes.Wpf;

namespace Scandalous.WPF.Services
{
    /// <summary>
    /// Implementation of theme service using Material Design themes
    /// </summary>
    public class ThemeService : IThemeService
    {
        private readonly ILogger<ThemeService>? _logger;
        private readonly string _themeConfigPath;
        private Theme _currentTheme = Theme.Light;

        public Theme CurrentTheme => _currentTheme;

        public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

        public ThemeService(ILogger<ThemeService>? logger = null)
        {
            _logger = logger;
            var userAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? "Scandalous.WPF";
            var appDataPath = Path.Combine(userAppDataPath, appName);
            Directory.CreateDirectory(appDataPath);
            _themeConfigPath = Path.Combine(appDataPath, "ThemeConfig.json");
        }

        public void SwitchTheme(Theme theme)
        {
            if (_currentTheme == theme)
                return;

            var previousTheme = _currentTheme;
            _currentTheme = theme;

            try
            {
                ApplyTheme(theme);
                ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(theme, previousTheme));
                _logger?.LogInformation("Theme switched from {PreviousTheme} to {NewTheme}", previousTheme, theme);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error switching theme from {PreviousTheme} to {NewTheme}", previousTheme, theme);
                // Revert to previous theme
                _currentTheme = previousTheme;
                throw;
            }
        }

        public void ToggleTheme()
        {
            var newTheme = _currentTheme switch
            {
                Theme.Light => Theme.Dark,
                Theme.Dark => Theme.Light,
                Theme.System => GetSystemTheme(),
                _ => Theme.Light
            };

            SwitchTheme(newTheme);
        }

        public async Task LoadThemeAsync()
        {
            try
            {
                if (!File.Exists(_themeConfigPath))
                {
                    // Use system theme as default
                    SwitchTheme(Theme.System);
                    return;
                }

                var json = await File.ReadAllTextAsync(_themeConfigPath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    SwitchTheme(Theme.System);
                    return;
                }

                var config = JsonSerializer.Deserialize<ThemeConfig>(json);
                if (config?.Theme == Theme.System)
                {
                    SwitchTheme(GetSystemTheme());
                }
                else
                {
                    SwitchTheme(config?.Theme ?? Theme.System);
                }

                _logger?.LogDebug("Theme loaded from configuration: {Theme}", _currentTheme);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading theme configuration");
                // Fall back to system theme
                SwitchTheme(GetSystemTheme());
            }
        }

        public async Task SaveThemeAsync()
        {
            try
            {
                var config = new ThemeConfig { Theme = _currentTheme };
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_themeConfigPath, json);
                _logger?.LogDebug("Theme configuration saved: {Theme}", _currentTheme);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error saving theme configuration");
            }
        }

        private void ApplyTheme(Theme theme)
        {
            var application = System.Windows.Application.Current;
            if (application == null)
                return;

            try
            {
                var paletteHelper = new PaletteHelper();
                var currentTheme = paletteHelper.GetTheme();

                var baseTheme = theme switch
                {
                    Theme.Dark => BaseTheme.Dark,
                    Theme.Light => BaseTheme.Light,
                    Theme.System => GetSystemTheme() == Theme.Dark ? BaseTheme.Dark : BaseTheme.Light,
                    _ => BaseTheme.Light
                };

                currentTheme.SetBaseTheme(baseTheme);
                paletteHelper.SetTheme(currentTheme);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error applying Material Design theme");
                // Fallback to manual resource dictionary approach
                ApplyThemeFallback(theme);
            }
        }

        private void ApplyThemeFallback(Theme theme)
        {
            var application = System.Windows.Application.Current;
            if (application == null)
                return;

            // Remove existing Material Design theme dictionaries
            var existingThemes = application.Resources.MergedDictionaries
                .Where(d => d.Source?.OriginalString.Contains("MaterialDesignTheme") == true)
                .ToList();

            foreach (var existingTheme in existingThemes)
            {
                application.Resources.MergedDictionaries.Remove(existingTheme);
            }

            // Add new theme dictionary
            var themeUri = theme switch
            {
                Theme.Dark => new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Dark.xaml"),
                Theme.Light => new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml"),
                _ => new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml")
            };

            var themeDictionary = new System.Windows.ResourceDictionary { Source = themeUri };
            application.Resources.MergedDictionaries.Add(themeDictionary);
        }

        private Theme GetSystemTheme()
        {
            try
            {
                // Check Windows theme setting
                using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                if (key?.GetValue("AppsUseLightTheme") is int value)
                {
                    return value == 0 ? Theme.Dark : Theme.Light;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Could not detect system theme, defaulting to light theme");
            }

            return Theme.Light;
        }

        private class ThemeConfig
        {
            public Theme Theme { get; set; } = Theme.System;
        }
    }
}