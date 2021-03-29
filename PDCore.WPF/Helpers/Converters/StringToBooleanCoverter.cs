using System;
using System.Windows.Data;
using PDCore.Extensions;

namespace PDCore.WPF.Helpers.Converters
{
    public class StringToBooleanCoverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string val = (string)value;

            return val.ToBoolean();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
