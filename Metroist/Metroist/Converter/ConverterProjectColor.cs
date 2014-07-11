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
    public class ConverterProjectColor : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value is string)
                return App.Current.Resources["ProjectColor" + (string)value] as SolidColorBrush;
            else if (value is int)
                return App.Current.Resources["ProjectColor" + ((int)value).ToString()] as SolidColorBrush;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class ConverterPanoramaProjectColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Project)
            {
                Project item = (Project)value;

                if (item != null && item.name.Contains("Inbox"))
                {
                    ImageBrush background = new ImageBrush();
                    background.ImageSource =
                        new System.Windows.Media.Imaging.BitmapImage(
                            new Uri("/MetroistLib;component/Images/MetroistInbox.png", UriKind.Relative));
                    return background;
                }
                else
                {
                    return App.Current.Resources["ProjectColor" + item.color] as SolidColorBrush;
                }
            }
            else if (value is string)
            {
                return App.Current.Resources["ProjectColor" + (string)value] as SolidColorBrush;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

}
