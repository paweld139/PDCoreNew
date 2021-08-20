using Microsoft.Extensions.Localization;
using System.Reflection;

namespace PDCoreNew.Services.Serv
{
    public class LocalizationService
    {
        private readonly IStringLocalizer _localizer;

        public LocalizationService(IStringLocalizerFactory factory)
        {
            var type = typeof(Resource);

            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);

            _localizer = factory.Create("Resource", assemblyName.Name);
        }

        public LocalizedString GetLocalizedString(string key) => _localizer[key];

        public string GetValue(string key)
        {
            var localizedString = GetLocalizedString(key);

            return localizedString.Value;
        }
    }
    public class LocalizationService2
    {
        private readonly IStringLocalizer _localizer;

        public LocalizationService2(IStringLocalizerFactory factory)
        {
            _localizer = factory.Create(typeof(Resource));
        }

        public LocalizedString GetLocalizedString(string key) => _localizer[key];

        public string GetValue(string key)
        {
            var localizedString = GetLocalizedString(key);

            return localizedString.Value;
        }
    }

}
