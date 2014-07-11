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
using MetroistLib.Model;

namespace Metroist.Converter
{
    public class ConverterListItemAccent : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            App app = Application.Current as App;
            FilterOption _value = value as FilterOption;
            SolidColorBrush color = null;

            if (_value != null)
            {
                color = _value.Selected ? 
                    new SolidColorBrush((App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color) :
                    new SolidColorBrush((App.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush).Color);
            }

            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
