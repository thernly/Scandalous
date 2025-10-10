using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Scandalous.WPF.Services;

/// <summary>
/// Service for showing configuration-related dialogs in the WPF application.
/// </summary>
public interface IConfigurationDialogService
{
    /// <summary>
    /// Shows a folder picker dialog.
    /// </summary>
    /// <param name="initialPath">The initial path to show in the dialog</param>
    /// <param name="title">The dialog title (optional)</param>
    /// <returns>The selected folder path, or null if cancelled</returns>
    Task<string?> ShowFolderPickerAsync(string initialPath = "", string title = "Select Folder");

    /// <summary>
    /// Shows a language selector dialog.
    /// </summary>
    /// <param name="availableLanguages">List of available language codes</param>
    /// <param name="currentLanguage">Currently selected language code</param>
    /// <param name="title">The dialog title (optional)</param>
    /// <returns>The selected language code, or null if cancelled</returns>
    Task<string?> ShowLanguageSelectorAsync(List<string> availableLanguages, string currentLanguage = "", string title = "Select Language");

    /// <summary>
    /// Shows a file picker dialog for selecting tessdata folder.
    /// </summary>
    /// <param name="initialPath">The initial path to show in the dialog</param>
    /// <param name="title">The dialog title (optional)</param>
    /// <returns>The selected tessdata folder path, or null if cancelled</returns>
    Task<string?> ShowTessdataFolderPickerAsync(string initialPath = "", string title = "Select Tessdata Folder");
} 