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
using Microsoft.Phone.Shell;
using Microsoft.Phone.Controls;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GeneralLib
{
    public class Utils
    {
        public static void CreateMenuItemDefault(IApplicationBar appBar)
        {
            var AboutItem = new ApplicationBarMenuItem
                {
                    Text = "sobre este aplicativo"
                };

            AboutItem.Click +=
                (sender, e) =>
                {
                    ((PhoneApplicationFrame)Application.Current.RootVisual).Navigate(new Uri("/Pages/Sobre.xaml", UriKind.Relative));
                };

            appBar.MenuItems.Add(AboutItem);
        }

        public static ApplicationBarIconButton createAddButton(string text)
        {
            return new ApplicationBarIconButton
            {
                Text = text,
                IconUri = new Uri("/Images/Add.png", UriKind.Relative),
                IsEnabled = true
            };
        }

        public static ApplicationBarIconButton createBackButton(string text)
        {
            return new ApplicationBarIconButton
            {
                Text = text,
                IconUri = new Uri("/Images/Back.png", UriKind.Relative),
                IsEnabled = true
            };
        }

        public static ApplicationBarIconButton createUncheckButton(string text)
        {
            return new ApplicationBarIconButton
            {
                Text = text,
                IconUri = new Uri("/Images/Uncheck.png", UriKind.Relative),
                IsEnabled = true
            };
        }

        public static ApplicationBarIconButton createAnunciosButton(string text)
        {
            return new ApplicationBarIconButton
            {
                Text = text,
                IconUri = new Uri("/Images/Exclamation.png", UriKind.Relative),
                IsEnabled = true
            };
        }

        public static ApplicationBarIconButton createCheckButton(string p)
        {
            return new ApplicationBarIconButton
            {
                Text = p,
                IconUri = new Uri("/Images/Check.png", UriKind.Relative),
                IsEnabled = true
            };
        }

        public static ApplicationBarIconButton createRefreshButton(string p)
        {
            return new ApplicationBarIconButton
            {
                Text = p,
                IconUri = new Uri("/Images/Upload.png", UriKind.Relative),
                IsEnabled = true
            };
        }

        public static ApplicationBarIconButton createMarkerButton(string p)
        {
            return new ApplicationBarIconButton
            {
                Text = p,
                IconUri = new Uri("/Images/MapsMarker.png", UriKind.Relative),
                IsEnabled = true
            };
        }

        public static ApplicationBarIconButton createFilterButton(string text = "")
        {
            return new ApplicationBarIconButton()
            {
                Text = text,
                IconUri = new Uri("Images/Filter.png", UriKind.Relative)
            };
        }

        public static ApplicationBarIconButton createDetailsButton(string text = "")
        {
            return new ApplicationBarIconButton()
            {
                Text = text,
                IconUri = new Uri("Images/List.png", UriKind.Relative)
            };
        }

        public static ApplicationBarIconButton createTrashButton(string text = "")
        {
            return new ApplicationBarIconButton()
            {
                Text = text,
                IconUri = new Uri("Images/Trash.png", UriKind.Relative)
            };
        }

        public static IApplicationBar DefaultApplicationBar()
        {
            IApplicationBar newAppBar = new ApplicationBar
            {
                IsVisible = true,
                BackgroundColor = (Color)Application.Current.Resources["Blue"]
            };

            CreateMenuItemDefault(newAppBar);

            return newAppBar;
        }

        public static List<DateTime> GetNext7Days(DateTime date)
        {
            List<DateTime> _return = new List<DateTime>();

            for (int i = 0; i < 7; i++)
            {
                _return.Add(date.Date);
                //_return += date.Day + " " + date.ToString("MMM", CultureInfo.InvariantCulture) + " " + date.Year;
                date = date.AddDays(1);

                //if (i < 6)
                //    _return += ", ";
            }

            return _return;
        }

        public static SolidColorBrush GetColorFromHexa(string hexaColor)
        {
            return new SolidColorBrush(
                Color.FromArgb(
                    Convert.ToByte(hexaColor.Substring(1, 2), 16),
                    Convert.ToByte(hexaColor.Substring(3, 2), 16),
                    Convert.ToByte(hexaColor.Substring(5, 2), 16),
                    Convert.ToByte(hexaColor.Substring(7, 2), 16)
                )
            );
        }

        public static ApplicationBarIconButton createDoneButton(string p)
        {
            return new ApplicationBarIconButton()
            {
                Text = p,
                IconUri = new Uri("Images/Done.png", UriKind.Relative)
            };
        }

        public static ApplicationBarIconButton createEditButton(string p)
        {
            return new ApplicationBarIconButton()
            {
                Text = p,
                IconUri = new Uri("Images/Edit.png", UriKind.Relative)
            };
        }

        public static ApplicationBarIconButton createDownloadButton(string p)
        {
            return new ApplicationBarIconButton()
            {
                Text = p,
                IconUri = new Uri("Images/Download.png", UriKind.Relative)
            };
        }

        public static SolidColorBrush ConvertStringToColor(String hex)
        {
            //remove the # at the front
            hex = hex.Replace("#", "");

            byte a = 255;
            byte r = 255;
            byte g = 255;
            byte b = 255;

            int start = 0;

            //handle ARGB strings (8 characters long)
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                start = 2;
            }

            //convert RGB characters to bytes
            r = byte.Parse(hex.Substring(start, 2), System.Globalization.NumberStyles.HexNumber);
            g = byte.Parse(hex.Substring(start + 2, 2), System.Globalization.NumberStyles.HexNumber);
            b = byte.Parse(hex.Substring(start + 4, 2), System.Globalization.NumberStyles.HexNumber);

            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        public static T SearchChildElement<T>(DependencyObject targeted_control) 
        {
            var count = VisualTreeHelper.GetChildrenCount(targeted_control);   // targeted_control is the listbox
            DependencyObject child = null;
            //if (count > 0)
            //{
            for (int i = 0; i < count; i++)
            {
                child = VisualTreeHelper.GetChild(targeted_control, i);
                if (child is T) // specific/child control 
                {
                    return (T)(object)(child);
                }
                else
                {
                    SearchChildElement<T>(child);
                }
            }
            
            return default(T);
        }

        public static T SearchChildElement<T>(DependencyObject targeted_control, ref DependencyObject child) 
        {
            var count = VisualTreeHelper.GetChildrenCount(targeted_control);   // targeted_control is the listbox
            //DependencyObject child = null;
            //if (count > 0)
            //{
            for (int i = 0; i < count; i++)
            {
                child = VisualTreeHelper.GetChild(targeted_control, i);
                if (child is T) // specific/child control 
                {
                    return (T)(object)(child);
                }
                else
                {
                    SearchChildElement<T>(child, ref child);
                }
            }
            //}

            return (T)(object)(child);
        }

        public static bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn,
                   @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
        }

    }
}
