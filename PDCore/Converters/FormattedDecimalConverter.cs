using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Threading;

namespace PDCore.Converters
{
    public class FormattedDecimalConverter : JsonConverter
    {
        private readonly CultureInfo culture;

        public FormattedDecimalConverter(CultureInfo culture)
        {
            this.culture = culture;
        }

        public FormattedDecimalConverter()
        {

        }

        private CultureInfo Culture => culture ?? Thread.CurrentThread.CurrentCulture;

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(decimal) ||
                    objectType == typeof(double) ||
                    objectType == typeof(float));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(Convert.ToString(value, Culture));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return Convert.ChangeType(reader.Value, objectType, Culture);
        }
    }
}
