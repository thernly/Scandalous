using Scandalous.Core.Models;

namespace Scandalous.Core.Services
{
    public interface IConfigurationManager
    {
        Task SaveConfigurationAsync(ScanConfiguration configuration);
        Task<ScanConfiguration> LoadConfigurationAsync();
        List<string> GetInstalledTessdataLanguageCodes(string tessdataFolder);
    }
} 