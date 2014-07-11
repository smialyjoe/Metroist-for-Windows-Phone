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
    public class ConverterCountingTasksFromProject : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Project _value = (Project)value;
            string result = "";
            if (_value.items != null && _value.items.Count > 0)
            {
                if (_value.items.Count > 99)
                {
                    result = "+99";
                }
                else
                {
                    result = _value.items.Count.ToString();
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
