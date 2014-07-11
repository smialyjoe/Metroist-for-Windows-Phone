using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using MetroistLib;
using MetroistLib.Model;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace Metroist
{
    public class Utils
    {
        public static ApplicationBar CreateApplicationBar()
        {
            return new ApplicationBar
            {
            };
        }

        public static ApplicationBar CreateApplicationBar(Color backColor)
        {
            return new ApplicationBar
            {
                BackgroundColor = backColor,
                ForegroundColor = (App.Current.Resources["ProjectColor20"] as SolidColorBrush).Color
            };
        }

        public static ApplicationBar CreateApplicationBar(Color backColor, Color foreColor)
        {
            return new ApplicationBar
            {
                BackgroundColor = backColor,
                ForegroundColor = foreColor
            };
        }

        public static bool IsValidEmail(string strIn)
        {   
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn,
                   @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
        }

        public static ApplicationBarIconButton createCheckButton(string text)
        {
            return new ApplicationBarIconButton() 
            { 
                //Text = (Application.Current as App).todoistLang.login_action_button, 
                Text = text, 
                IconUri = new Uri("Images/Check.png", UriKind.Relative)
            };
        }

        public static ApplicationBarIconButton createSearchButton(string text="")
        {
            return new ApplicationBarIconButton()
            {
                Text = text,
                IconUri = new Uri("Images/Find.png", UriKind.Relative)
            };
        }

        public static ApplicationBarMenuItem createSignUpMenuItem(string text)
        {
            return new ApplicationBarMenuItem()
            {
                Text = text
            };
        }

        public static ApplicationBarMenuItem createAboutMenuItem(string text)
        {
            return new ApplicationBarMenuItem()
            {
                Text = text
            };
        }

        public static Uri SignUpPage(List<TimezoneItem> items = null)
        {
            return new Uri("/Pages/SignUpPage.xaml", UriKind.Relative);
        }

        public static Uri ChooseTimezonePage()
        {
            //Page.ChooseTimezonePage.internalTimezone = timezone;
            return new Uri("/Pages/TimezonePage.xaml", UriKind.Relative);
        }

        public static Uri MainTodoistPage(bool removeBackStack = false)
        {
            Pages.MainTodoistPage.removeBackStack = removeBackStack;
            return new Uri("/Pages/MainTodoistPage.xaml", UriKind.Relative);
        }

        public static bool CheckNetworkConnection()
        {
            return DeviceNetworkInformation.IsNetworkAvailable && NetworkInterface.GetIsNetworkAvailable();
        }

        internal static Uri LoginPage()
        {
            return new Uri("/Pages/Login.xaml", UriKind.Relative);
        }

        internal static Uri WebBrowserPage()
        {
            return new Uri("/Pages/WebBrowser.xaml", UriKind.Relative);
        }

        internal static Uri LoginWithGooglePage()
        {
            return new Uri("/Pages/LoginWithGoogle.xaml", UriKind.Relative);
        }

        internal static Uri AboutPage()
        {
            return new Uri("/Pages/AboutPage.xaml", UriKind.Relative);
        }

        internal static Uri TaskDetailPage(QueryDataItem item)
        {
            TaskDetail.taskSelected = item;
            return new Uri("/Pages/TaskDetail.xaml", UriKind.Relative);
        }

        internal static string EncondeQuery(string p)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("[");

            var splitedString = p.Split(',');

            foreach (var word in splitedString)
            {
                stringBuilder.AppendFormat("\"{0}\"", word.Trim());
                stringBuilder.Append(",");
            }

            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append("]");

            return stringBuilder.ToString();
        }

        internal static Uri ProjectDetailPage(Project selected)
        {
            ProjectDetail.projectSelected = selected;
            return new Uri("/Pages/ProjectDetail.xaml", UriKind.Relative);
        }

        internal static Uri AddTaskPage(Project selected)
        {
            AddTask.projectSelected = selected;
            return new Uri("/Pages/AddTask.xaml", UriKind.Relative);
        }

        internal static Uri SettingsPage()
        {
            return new Uri("/Pages/SettingsPage.xaml", UriKind.Relative);
        }

        internal static Uri FilterOptionsPage()
        {
            return new Uri("/Pages/FilterOptionsPage.xaml", UriKind.Relative);
        }

        internal static Uri SignUpPage()
        {
            return new Uri("/Pages/SignUpPage.xaml", UriKind.Relative);
        }

        internal static Uri DateTimeChooserPage()
        {
            return new Uri("/Pages/DateTimeChooserPage.xaml", UriKind.Relative);
        }

        internal static string Message(string errorMsg)
        {
            App app = (Application.Current as App);

            if (errorMsg == Error.ERROR_WRONG_DATE_SYNTAX)
                return "Invalid date/time format. Try again!";

            return errorMsg;
        }

        internal static string EncodeJsonItems(List<Project> projects)
        {
            string stringResponse = "{";

            foreach (var project in projects)
            {
                stringResponse += "\"" + project.id + "\"";
                stringResponse += ":";
                stringResponse += "\"" + project.last_updated + "\"";

                if(project != projects.Last())
                    stringResponse += ",";
            }

            stringResponse += "}";

            return stringResponse;
        }

        internal static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }

        internal static string EncodeJsonItems(Dictionary<string, object> internalArgs)
        {
            string stringResponse = "{";

            foreach (var keyval in internalArgs)
            {
                stringResponse += "\"" + keyval.Key + "\"";
                stringResponse += ":";

                if(keyval.Value.GetType() == typeof(Dictionary<string,object>))
                    stringResponse += Utils.EncodeJsonItems(keyval.Value as Dictionary<string,object>);
                else if((keyval.Value is string && ((string)keyval.Value).Contains("[") && ((string)keyval.Value).Contains("]") ))
                    stringResponse += keyval.Value;
                else
                    stringResponse += "\"" + keyval.Value + "\"";

                if (!keyval.Equals(internalArgs.Last()))
                    stringResponse += ",";
            }

            stringResponse += "}";

            return stringResponse;
        }

        internal static Uri AddProjectPage()
        {
            return new Uri("/Pages/AddProject.xaml", UriKind.Relative);
        }

        internal static Uri EditProjectPage(Project project)
        {
            EditProject.projSelected = project;
            return new Uri("/Pages/EditProject.xaml", UriKind.Relative);
        }

        public static void ProgressIndicatorStatus(string text, ProgressIndicator progress, TimeSpan? time = null)
        {
            DispatcherTimer timerTo = null;
            timerTo = new DispatcherTimer
            {
                Interval = new TimeSpan(0,0,0,0,0)
            };

            timerTo.Tick += (sender, e) =>
            {
                progress.Text = text;
                progress.IsIndeterminate = false;
                progress.IsVisible = true;

                //progress.Dispatcher.BeginInvoke(() =>
                //{
                    DispatcherTimer timer = null;
                    timer = new DispatcherTimer
                    {
                        Interval = !time.HasValue ? new TimeSpan(0,0,2) : time.Value
                    };

                    timer.Tick += (p, args) =>
                    {
                        progress.IsIndeterminate = true;
                        progress.Text = string.Empty;
                        progress.IsVisible = false;
                        (p as DispatcherTimer).Stop();
                    };

                    timer.Start();
                //});

                (sender as DispatcherTimer).Stop();
            };

            timerTo.Start();
            
        }

        internal static string ErrorMessage(string stringError)
        {
            string result = "";
            stringError = stringError.Replace("\"", "");
            if (stringError == "LOGIN_ERROR")
            {
                result = "Your username or password are incorrect. Please try again.";
            }
            else if (stringError == "ALREADY_REGISTRED")
            {
                result = "Your email is already registered. \nPlease, try another email or visit Todoist.com to recovery your password.";
            }
            else if (stringError == "TOO_SHORT_PASSWORD")
            {
                result = "Your password must have at least 5 characters.";
            }
            else if (stringError == "INVALID_EMAIL")
            {
                result = "You have entered a invalid e-mail. Please try again.";
            }
            else if (stringError == "INVALID_TIMEZONE")
            {
                result = "You have entered a invalid timezone. Please try again.";
            }
            else if (stringError == "INVALID_FULL_NAME")
            {
                result = "You have entered a invalid full name. Please try again.";
            }
            else if (stringError == "UNKNOWN_ERROR")
            {
                result = "Oops! We don't know what happened. Please, try again in a few moments.";
            }

            return result;
        }



        internal static Uri WebPage(String urlToGo, Action successCallback)
        {
            Pages.WebPage.urlToGo = urlToGo;
            Pages.WebPage.successCallback = successCallback;
            return new Uri("/Pages/WebPage.xaml", UriKind.Relative);
        }
    }
}
