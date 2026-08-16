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
            return availableCodes.Contains(languageCode, StringComparer.OrdinalIgnoreCase);
        }

        public string GetBestLanguageCode(string tessdataFolder, string userPreferredCode = "eng")
        {
            var availableCodes = GetAvailableLanguageCodes(tessdataFolder);

            if (availableCodes.Count == 0)
                return string.Empty;

            if (!string.IsNullOrWhiteSpace(userPreferredCode))
            {
                var preferredMatch = availableCodes.FirstOrDefault(code =>
                    string.Equals(code, userPreferredCode, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(preferredMatch))
                    return preferredMatch;
            }

            var englishMatch = availableCodes.FirstOrDefault(code =>
                string.Equals(code, GetDefaultLanguageCode(), StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(englishMatch))
                return englishMatch;

            return availableCodes[0];
        }
    }
} 