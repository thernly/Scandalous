using System.Text.Json;

namespace ScanUtility
{
    internal class ConfigurationManager
    {
        private const string ConfigFileName = "ScanUtilityConfig.json";
        private readonly string _configFilePath;
        private static readonly JsonSerializerOptions CachedJsonSerializerOptions = new()
        {
            WriteIndented = true,
            MaxDepth = 10,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ConfigurationManager()
        {
            var userAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string? appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            appName ??= "ScanUtility";
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

        public static List<string> GetInstalledTessdataLanguageCodes(string tessdataFolder)
        {
            if (string.IsNullOrWhiteSpace(tessdataFolder) || !Directory.Exists(tessdataFolder))
            {
                return new List<string>();
            }
            var languageFiles = Directory.GetFiles(tessdataFolder, "*.traineddata", SearchOption.TopDirectoryOnly);
            var languageCodes = new List<string>();
            foreach (var file in languageFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    languageCodes.Add(fileName);
                }
            }
            return languageCodes;
        }
    }
}
