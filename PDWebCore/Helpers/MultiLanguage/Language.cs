namespace PDWebCore.Helpers.MultiLanguage
{
    public class Language
    {
        public Language(string languageFullName, string languageCultureName, string languageShortName = null)
        {
            LanguageFullName = languageFullName;
            LanguageCultureName = languageCultureName;
            LanguageShortName = languageShortName ?? languageCultureName;
        }

        public string LanguageFullName { get; set; }

        public string LanguageCultureName { get; set; }

        public string LanguageShortName { get; set; }
    }
}
