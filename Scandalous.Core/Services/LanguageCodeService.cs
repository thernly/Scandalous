namespace Scandalous.Core.Services
{
    public class LanguageCodeService : ILanguageCodeService
    {
        private readonly IConfigurationManager _configManager;

        public LanguageCodeService(IConfigurationManager configManager)
        {
            _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
        }

        public List<string> GetAvailableLanguageCodes(string tessdataFolder, string userPreferredCode = "eng")
        {
            return _configManager.GetInstalledTessdataLanguageCodes(tessdataFolder);
        }

        public string GetDefaultLanguageCode()
        {
            return "eng";
        }

        public bool IsLanguageCodeValid(string languageCode, string tessdataFolder)
        {
            if (string.IsNullOrEmpty(languageCode))
                return false;

            var availableCodes = GetAvailableLanguageCodes(tessdataFolder);
            return availableCodes.Contains(languageCode);
        }

        public string GetBestLanguageCode(string tessdataFolder, string userPreferredCode = "eng")
        {
            var availableCodes = GetAvailableLanguageCodes(tessdataFolder);
            
            if (availableCodes.Count == 0)
                return GetDefaultLanguageCode();

            if (!string.IsNullOrEmpty(userPreferredCode) && availableCodes.Contains(userPreferredCode))
                return userPreferredCode;

            return availableCodes[0]; // Fallback to first available code
        }
    }
} 