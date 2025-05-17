using System.Text.Json;

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
            var userAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string? appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            if (appName == null)
            {
                appName = "ScanUtility";
            }
            var appDataPath = Path.Combine(userAppDataPath, appName);

            Directory.CreateDirectory(appDataPath);
            _configFilePath = Path.Combine(appDataPath, ConfigFileName);
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
