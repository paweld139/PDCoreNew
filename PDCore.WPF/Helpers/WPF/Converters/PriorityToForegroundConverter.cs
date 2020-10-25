using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PDCore.WPF.Helpers.WPF.Converters
{
    public class PriorityToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Brush))
            {
                throw new InvalidOperationException("Celem powinien być typ Brush");
            }

            int priorytet = System.Convert.ToInt32(value);

            return (priorytet == 1 ? Brushes.Red : Brushes.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
