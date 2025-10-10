using Scandalous.Core.Models;

namespace Scandalous.Core.Services
{
    public interface IConfigurationManager
    {
        Task SaveConfigurationAsync(ScanConfiguration configuration);
        Task<ScanConfiguration> LoadConfigurationAsync();
        List<string> GetInstalledTessdataLanguageCodes(string tessdataFolder);
        
        /// <summary>
        /// Saves the window state information
        /// </summary>
        /// <param name="windowState">The window state to save</param>
        Task SaveWindowStateAsync(WindowStateInfo windowState);
        
        /// <summary>
        /// Loads the window state information
        /// </summary>
        /// <returns>The saved window state, or null if not found</returns>
        Task<WindowStateInfo?> LoadWindowStateAsync();
    }
} 