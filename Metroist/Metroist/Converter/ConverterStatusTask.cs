using System;
using System.Windows.Data;

namespace Metroist.Converter
{
    public class ConverterStatusTask : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value ? "Uncompleted" : "Done";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
