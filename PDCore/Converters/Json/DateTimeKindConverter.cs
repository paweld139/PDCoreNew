using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace PDCore.Converters.Json
{
    public class DateTimeKindConverter : IsoDateTimeConverter
    {
        private readonly DateTimeKind dateTimeKind;

        public DateTimeKindConverter(DateTimeKind dateTimeKind)
        {
            this.dateTimeKind = dateTimeKind;
        }

        public DateTimeKindConverter() : this(DateTimeKind.Utc)
        {
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = base.ReadJson(reader, objectType, existingValue, serializer);

            DateTime? dateTime = (DateTime?)result;

            if (dateTime != null && dateTime.Value.Kind == DateTimeKind.Local)
            {
                dateTime = dateTime.Value.ToUniversalTime();
            }

            if (dateTime != null)
                return dateTime.Value;
            else
                return dateTime;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DateTime? dateTime = (DateTime?)value;

            if (dateTime != null)
                dateTime = DateTime.SpecifyKind(dateTime.Value, dateTimeKind);

            base.WriteJson(writer, dateTime, serializer);
        }
    }
}
