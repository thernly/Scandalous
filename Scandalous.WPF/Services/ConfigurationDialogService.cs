using Microsoft.Win32;
using System.Windows;
using System.Windows.Forms;

using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Scandalous.WPF.Services;

/// <summary>
/// Implementation of the configuration dialog service for WPF.
/// </summary>
public class ConfigurationDialogService : IConfigurationDialogService
{
    /// <summary>
    /// Shows a folder picker dialog.
    /// </summary>
    public async Task<string?> ShowFolderPickerAsync(string initialPath = "", string title = "Select Folder")
    {
        return await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = title,
                UseDescriptionForTitle = true,
                ShowNewFolderButton = true
            };

            if (!string.IsNullOrEmpty(initialPath) && System.IO.Directory.Exists(initialPath))
            {
                dialog.InitialDirectory = initialPath;
            }

            return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : null;
        });
    }

    /// <summary>
    /// Shows a language selector dialog.
    /// </summary>
    public async Task<string?> ShowLanguageSelectorAsync(List<string> availableLanguages, string currentLanguage = "", string title = "Select Language")
    {
        return await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            // For now, we'll use a simple message box with a list
            // In a full implementation, this would be a custom dialog with a proper list
            var languageList = string.Join("\n", availableLanguages);
            var message = $"Available languages:\n\n{languageList}\n\nPlease enter the language code:";
            
            var result = Microsoft.VisualBasic.Interaction.InputBox(
                message,
                title,
                currentLanguage);

            return string.IsNullOrEmpty(result) ? null : result;
        });
    }

    /// <summary>
    /// Shows a file picker dialog for selecting tessdata folder.
    /// </summary>
    public async Task<string?> ShowTessdataFolderPickerAsync(string initialPath = "", string title = "Select Tessdata Folder")
    {
        return await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = title,
                UseDescriptionForTitle = true,
                ShowNewFolderButton = true
            };

            if (!string.IsNullOrEmpty(initialPath) && System.IO.Directory.Exists(initialPath))
            {
                dialog.InitialDirectory = initialPath;
            }

            return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : null;
        });
    }
} 