using System;
using System.Globalization;
using System.Windows.Data;
using PDCore.Extensions;

namespace PDCore.WPF.Helpers.Converters
{
    public class UtcToLocalDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.CastObject<DateTime?>()?.ToLocalTime();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

