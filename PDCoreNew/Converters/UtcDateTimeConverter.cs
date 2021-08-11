using System;
using System.ComponentModel;
using System.Globalization;

namespace PDCoreNew.Converters
{
    public sealed class UtcDateTimeConverter : DateTimeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            DateTime dateTime = (DateTime)base.ConvertFrom(context, culture, value);

            if (dateTime.Kind == DateTimeKind.Local)
            {
                dateTime = dateTime.ToUniversalTime();
            }

            return dateTime;
        }
    }
}
