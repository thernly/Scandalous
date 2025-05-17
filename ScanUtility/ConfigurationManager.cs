using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ScanUtility
{
    internal class ConfigurationManager
    {
        private const string ConfigFileName = "ScanUtilityConfig.json";
        private readonly string _configFilePath;
        private static readonly JsonSerializerOptions CachedJsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            MaxDepth = 10,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ConfigurationManager()
        {
            var applicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Directory.CreateDirectory(applicationDataPath);
            _configFilePath = Path.Combine(applicationDataPath, ConfigFileName);
        }

        public async Task SaveConfigurationAsync(ScanConfiguration configuration)
        {
            var json = JsonSerializer.Serialize(configuration, CachedJsonSerializerOptions);
            await File.WriteAllTextAsync(_configFilePath, json);
        }

        public async Task<ScanConfiguration> LoadConfigurationAsync()
        {
            if (!File.Exists(_configFilePath))
            {
                return new ScanConfiguration();
            }
            var json = await File.ReadAllTextAsync(_configFilePath);
            return JsonSerializer.Deserialize<ScanConfiguration>(json, CachedJsonSerializerOptions) ?? new ScanConfiguration();
        }
    }
}
