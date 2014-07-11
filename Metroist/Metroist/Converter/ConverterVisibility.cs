using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace Metroist.Converter
{
    public class ConverterVisibility : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool visible = true;

            if (value is Array)
                visible = ((Array)value).Length != 0;
            else if (value is string)
                visible = ((string)value) != "0" || ((string)value) != "";
            else if (value == null)
                visible = false;
            else if (value is bool)
                visible = (bool)value;

            if ((string)parameter == "!")
                visible = !visible;

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
