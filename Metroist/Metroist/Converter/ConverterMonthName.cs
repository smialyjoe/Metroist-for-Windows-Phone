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
    public class ConverterMonthName : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DateTime)
            {
                DateTime _date = (DateTime)value;

                if(parameter is string)
                {
                    int AmountToChangeMonth;
                    if (Int32.TryParse((string)parameter, out AmountToChangeMonth))
                    {
                        _date = _date.AddMonths(AmountToChangeMonth);
                    }
                }

                return Calendar.MonthShortNames[_date.Month - 1];
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
