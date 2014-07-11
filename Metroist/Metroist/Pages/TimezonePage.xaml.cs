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
using System.Collections.ObjectModel;
using Microsoft.Phone.Shell;

namespace Metroist.Pages
{
    public partial class TimezonePage : PhoneApplicationPage
    {
        App app = Application.Current as App;
        ProgressIndicator progressIndicator = new ProgressIndicator();

        public TimezonePage()
        {
            InitializeComponent();

            GetTimezoneList();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            SystemTray.ProgressIndicator = progressIndicator;
        }

        private void GetTimezoneList()
        {
            progressIndicator.IsVisible = true;
            progressIndicator.IsIndeterminate = true;
            progressIndicator.Text = "Loading";

            SystemTray.SetProgressIndicator(this, progressIndicator);

            app.service.GetTimezoneList(
                (result) =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        SearchTxtBox.IsEnabled = true;
                        timezonesListBox.ItemsSource = result;
                    });
                },
                (error) =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show(error, "Metroist", MessageBoxButton.OK);
                    });
                },
                () =>
                {
                    progressIndicator.IsVisible = false;
                    progressIndicator.IsIndeterminate = false;
                });
        }

        private void FilterTimezoneBySearchField()
        {
            //var Text = SearchTxtBox.Text;

            //if (internalTimezoneList == null)
            //    return;

            //var result = from item in internalTimezoneList
            //             where item.Title.ToLowerInvariant().Contains(Text.ToLowerInvariant())
            //             select item;

            //timezonesListBox.ItemsSource = result;
        }

        private void SearchTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SearchTxtBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void SearchButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void SearchImage_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {

        }

        private void SearchImage_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {

        }

        private void timezonesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}