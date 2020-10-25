﻿using PDCore.Common.Services.Serv;
using PDWebCore.Helpers.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Web;

namespace PDCore.Web.Helpers.MultiLanguage
{
    public static class LanguageHelper
    {
        public static readonly List<Language> AvailableLanguages = new List<Language>
        {
            new Language("English", "en", "us"),
            new Language("Polski", "pl"),
        };

        public static bool IsLanguageAvailable(string lang) => AvailableLanguages.Exists(a => a.LanguageCultureName.Equals(lang));

        public static string GetDefaultLanguage() => AvailableLanguages[0].LanguageCultureName;

        private static string GetLanguage(HttpRequest httpRequest)
        {
            HttpCookie languageCookie = httpRequest.Cookies["culture"];

            string language;

            if (languageCookie != null)
            {
                language = languageCookie.Value;
            }
            else
            {
                var userLanguages = httpRequest.UserLanguages;
                var userLanguage = userLanguages != null ? userLanguages[0] : string.Empty;

                if (!string.IsNullOrEmpty(userLanguage))
                {
                    language = userLanguage;
                }
                else
                {
                    language = GetDefaultLanguage();
                }
            }

            return language;
        }

        public static void SetLanguage(string language)
        {
            try
            {
                var cultureInfo = new CultureInfo(language);

                string languageISO = cultureInfo.TwoLetterISOLanguageName; //Możliwe, że sostał przekazany specyficzny

                if (!IsLanguageAvailable(languageISO))
                    languageISO = GetDefaultLanguage();

                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(languageISO);
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);

                HttpCookie langCookie = new HttpCookie("culture", language)
                {
                    Expires = DateTime.UtcNow.AddYears(1)
                };

                HttpContext.Current.Response.Cookies.Add(langCookie);
            }
            catch (Exception ex)
            {
                LogService.Error("Błąd podczas ustawiania języka", ex);
            }
        }

        public static void SetLanguage(HttpRequest httpRequest)
        {
            string language = GetLanguage(httpRequest);

            SetLanguage(language);
        }
    }
}
