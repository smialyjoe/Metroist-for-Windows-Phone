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
using Microsoft.Phone.Tasks;

namespace Metroist
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void LinkClick(object sender, RoutedEventArgs e)
        {
            HyperlinkButton link = sender as HyperlinkButton;

            try
            {
                var taskBrowser = new WebBrowserTask();
                taskBrowser.Uri = new Uri(link.Tag.ToString(), UriKind.Absolute);
                taskBrowser.Show();
            }
            catch { }
            
        }

        private void MailSendClick(object sender, RoutedEventArgs e)
        {
            HyperlinkButton link = sender as HyperlinkButton;

            try
            {
                var mailTask = new EmailComposeTask();
                mailTask.To = link.Tag.ToString();
                mailTask.Show();
            }
            catch { }
        }
    }
}
