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
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using Metroist.Pages;
using MetroistLib.Model;

namespace Metroist
{
    public partial class FilterOptionsPage : PhoneApplicationPage
    {
        FilterOption filterOption;
        App app = Application.Current as App;

        public ObservableCollection<FilterOption> filteringOptions = FilterOption.filteringOptions;

        public FilterOptionsPage()
        {
            InitializeComponent();

            filterOption = filteringOptions.Where(x=>x.Key == app.settings.DateStringHome.Key).FirstOrDefault(); 

            FilteringOptionsListBox.SelectedIndex = FilteringOptionsListBox.Items.IndexOf(filteringOptions);

            filterOption.Selected = true;

            DataContext = filteringOptions;

            FilteringOptionsListBox.SelectionChanged += new SelectionChangedEventHandler(FilteringOptionsListBox_SelectionChanged);
        }

        void FilteringOptionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DoWork(sender);

            DataContext = null;
            DataContext = filteringOptions;

            Dispatcher.BeginInvoke(() =>
            {
                NavigationService.GoBack();
            });

            MainTodoistPage.changedFilter = true;
        }

        private void DoWork(object sender)
        {
            ListBox listbox = sender as ListBox;

            if (listbox.SelectedItem != null)
            {
                var filterSelected = listbox.SelectedItem as FilterOption;

                app.settings.DateStringHome = filterSelected != null ? 
                    filteringOptions[filteringOptions.IndexOf(filterSelected)] : 
                    app.settings.DateStringHome;

                filterOption.Selected = false;
                filterOption = filterSelected;

                if (filterSelected != null)
                    filterSelected.Selected = filteringOptions[filteringOptions.IndexOf(filterOption)].Selected = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //filteringOptions[(DateTime.Now.Second % 4) -1].Selected = false;
            filteringOptions[DateTime.Now.Second % 4].Selected = true;
        }
    }

}
