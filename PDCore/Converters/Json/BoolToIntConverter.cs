using Newtonsoft.Json;
using System;

namespace PDCore.Converters.Json
{
    public class BoolToIntConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return ((existingValue as int?) ?? 0) == 1;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var v = (bool?)value;
            var w = v.HasValue && v.Value ? 1 : 0;

            writer.WriteValue(w);
        }
    }
}
