using System.Text.Json;
using Scandalous.Core.Models;

namespace Scandalous.Core.Services
{
    public class ConfigurationManager : IConfigurationManager
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

        // Protected constructor for testability
        protected ConfigurationManager(string configFilePath)
        {
            _configFilePath = configFilePath;
        }

        public async Task SaveConfigurationAsync(ScanConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            // Ensure the directory exists
            var directory = Path.GetDirectoryName(_configFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(configuration, CachedJsonSerializerOptions);
            var tempFilePath = $"{_configFilePath}.{Guid.NewGuid():N}.tmp";
            await File.WriteAllTextAsync(tempFilePath, json);
            File.Move(tempFilePath, _configFilePath, overwrite: true);
        }

        public async Task<ScanConfiguration> LoadConfigurationAsync()
        {
            if (!File.Exists(_configFilePath))
            {
                return new ScanConfiguration();
            }

            try
            {
                var json = await File.ReadAllTextAsync(_configFilePath);
                
                // Handle empty or whitespace files
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new ScanConfiguration();
                }

                var result = JsonSerializer.Deserialize<ScanConfiguration>(json, CachedJsonSerializerOptions);
                return result ?? new ScanConfiguration();
            }
            catch (JsonException)
            {
                // Return default configuration if JSON is corrupted or invalid
                return new ScanConfiguration();
            }
            catch (IOException)
            {
                // Return default configuration if file is in use or inaccessible
                return new ScanConfiguration();
            }
        }

        public List<string> GetInstalledTessdataLanguageCodes(string tessdataFolder)
        {
            if (string.IsNullOrWhiteSpace(tessdataFolder) || !Directory.Exists(tessdataFolder))
            {
                return new List<string>();
            }

            var languageCodes = new List<string>();
            try
            {
                var languageFiles = Directory.GetFiles(tessdataFolder, "*.traineddata", SearchOption.TopDirectoryOnly);
                foreach (var file in languageFiles)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        languageCodes.Add(fileName);
                    }
                }
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is IOException)
            {
                // Surface an empty list on IO exceptions
                return new List<string>();
            }

            return languageCodes;
        }

        public async Task SaveWindowStateAsync(WindowStateInfo windowState)
        {
            if (windowState == null)
            {
                throw new ArgumentNullException(nameof(windowState));
            }

            var windowStateFilePath = Path.Combine(
                Path.GetDirectoryName(_configFilePath) ?? string.Empty, 
                "WindowState.json");

            // Ensure the directory exists
            var directory = Path.GetDirectoryName(windowStateFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(windowState, CachedJsonSerializerOptions);
            var tempFilePath = $"{windowStateFilePath}.{Guid.NewGuid():N}.tmp";
            await File.WriteAllTextAsync(tempFilePath, json);
            File.Move(tempFilePath, windowStateFilePath, overwrite: true);
        }

        public async Task<WindowStateInfo?> LoadWindowStateAsync()
        {
            var windowStateFilePath = Path.Combine(
                Path.GetDirectoryName(_configFilePath) ?? string.Empty, 
                "WindowState.json");

            if (!File.Exists(windowStateFilePath))
            {
                return null;
            }

            try
            {
                var json = await File.ReadAllTextAsync(windowStateFilePath);
                
                // Handle empty or whitespace files
                if (string.IsNullOrWhiteSpace(json))
                {
                    return null;
                }

                var result = JsonSerializer.Deserialize<WindowStateInfo>(json, CachedJsonSerializerOptions);
                return result;
            }
            catch (JsonException)
            {
                // Return null if JSON is corrupted or invalid
                return null;
            }
            catch (IOException)
            {
                // Return null if file is in use or inaccessible
                return null;
            }
        }
    }
} 