namespace Scandalous.Core.Services
{
    public interface ILanguageCodeService
    {
        List<string> GetAvailableLanguageCodes(string tessdataFolder, string userPreferredCode = "eng");
        string GetDefaultLanguageCode();
        bool IsLanguageCodeValid(string languageCode, string tessdataFolder);
        string GetBestLanguageCode(string tessdataFolder, string userPreferredCode = "eng");
    }
} 