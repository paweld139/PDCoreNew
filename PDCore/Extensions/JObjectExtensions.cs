using Newtonsoft.Json.Linq;
using System.Linq;

namespace PDCore.Extensions
{
    public static class JObjectExtensions
    {
        public static T GetProperty<T>(this JObject obj, string propertyName, bool throwIfNull, T defaultValue = default(T))
        {
            T result;

            var property = obj.Properties().FirstOrDefault(p => p.Name == propertyName);

            if (throwIfNull)
                property.ThrowIfNull($"{propertyName} ({typeof(T)})");

            if (property != null)
                result = property.Value.Value<T>();
            else
                result = defaultValue;

            return result;
        }

        public static int GetInt(this JObject obj, string propertyName) => GetProperty<int>(obj, propertyName, true);

        public static int GetInt(this JObject obj, string propertyName, int defaultValue) => GetProperty(obj, propertyName, false, defaultValue);

        public static string GetString(this JObject obj, string propertyName) => GetProperty<string>(obj, propertyName, true);

        public static string GetString(this JObject obj, string propertyName, string defaultValue) => GetProperty(obj, propertyName, false, defaultValue);
    }
}
