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
using System.Collections;
using System.Collections.Generic;

namespace Metroist.Converter
{
    public class ConverterTimezoneName : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = string.Empty;

            if (value is IList)
            {
                List<string> list = (List<string>)value;
                if (list != null && list.Count == 2)
                {
                    if ((string)parameter == "0")
                    {
                        result = list[0];
                    }
                    else if ((string)parameter == "1")
                    {
                        result = list[1];
                    }
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
