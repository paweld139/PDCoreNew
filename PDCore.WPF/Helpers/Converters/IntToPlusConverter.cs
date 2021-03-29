using System;
using System.Globalization;
using System.Windows.Data;

namespace PDCore.WPF.Helpers.Converters
{
    public class IntToPlusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int source)
            {
                switch (source)
                {
                    case 0: return " ";
                    case 1: return "+";
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string source = value.ToString();

            switch (source)
            {
                case " ": return 0;
                case "+": return 1;
            }

            return source;
        }
    }
}
