using System;
using System.Linq;
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
    public class ConverterTaskProjectColor : IValueConverter
    {
        App app = Application.Current as App;
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            QueryDataItem item = value as QueryDataItem;
            SolidColorBrush color = null;

            if (item != null)
            {
                var projOfTask = app.projects.Where(proj => proj.id == item.project_id).FirstOrDefault();

                return App.Current.Resources["ProjectColor" + (projOfTask != null ? projOfTask.color : 0)] as SolidColorBrush;
            }

            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
