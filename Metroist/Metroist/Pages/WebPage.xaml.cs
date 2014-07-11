using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Metroist.Pages
{
    public partial class WebPage : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator = 
            new ProgressIndicator { IsIndeterminate = true, IsVisible = false, Text = "Loading" };

        App app = (Application.Current as App);

        public static String urlToGo;
        public static Action successCallback;

        public WebPage()
        {
            InitializeComponent();

            SystemTray.SetProgressIndicator(this, progressIndicator);

            MetroistWebBrowser.Navigate(new Uri(urlToGo, UriKind.Absolute));

            MetroistWebBrowser.Navigating += new EventHandler<NavigatingEventArgs>(MetroistWebBrowser_Navigating);
            MetroistWebBrowser.Navigated += new EventHandler<System.Windows.Navigation.NavigationEventArgs>(MetroistWebBrowser_Navigated);
        }

        void MetroistWebBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            progressIndicator.IsVisible = false;
            SystemTray.SetProgressIndicator(this, null);
        }

        private bool SetGoogleCode(string url)
        {
            if (url.StartsWith("http://localhost"))
            {
                string urlParamsString = url.Split('?')[1];
                string[] urlParamsArray = urlParamsString.Split('&');

                for (int i = 0; i < urlParamsArray.Length; i++)
                {
                    string[] keyValue = urlParamsArray[i].Split('=');
                    if (keyValue[0] == "code")
                    {
                        app.service.googleCode = keyValue[1];
                        app.service.GoogleAccessToken(keyValue[1], successCallback);
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        void MetroistWebBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            var url = e.Uri.ToString();
            if (url.StartsWith("http://localhost"))
            {
                MetroistWebBrowser.Visibility = Visibility.Collapsed;
                SetGoogleCode(url);
            }

            progressIndicator.IsVisible = true;
            SystemTray.SetProgressIndicator(this, progressIndicator);
        }
    }
}