using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using MetroistLib.Model;

namespace Metroist.Converter
{
    public class ConverterTaskProjectName :IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            App app = Application.Current as App;
            QueryDataItem item = value as QueryDataItem;
            string projectName = "";

            if (item != null)
            {
                try
                {
                    var projOfTask = app.projects.Where(proj => proj.id == item.project_id).FirstOrDefault();
                    projectName = projOfTask.name;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                
            }

            return projectName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
