/*using Todoist.Core;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace Todoist.Page
{
    public partial class ChooseTimezonePage : PhoneApplicationPage
    {
        App app = Application.Current as App;
        ProgressIndicator progressIndicator = new ProgressIndicator();
        public TimezoneItem internalTimezone;
        private static ObservableCollection<TimezoneItem> internalTimezoneList;

        private readonly ApplicationBarIconButton checkButton = Utils.createCheckButton();
        private readonly ApplicationBarIconButton searchButton = Utils.createSearchButton();

        public ChooseTimezonePage()
        {
            InitializeComponent();

            DataContext = app.todoistLang;

            GetTimezone();
        }

        private void ApplicationBarCreate()
        {
            ApplicationBar.Buttons.Add(searchButton);

            searchButton.Text = app.todoistLang.general_search;
            searchButton.Click += 
            (sender, e) =>
            {
                //time
            };

        }

        private void GetTimezone()
        {
            if (internalTimezoneList != null)
            {
                timezonesListBox.ItemsSource = internalTimezoneList;
            }
            else
            {
                progressIndicator.IsVisible = true;
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = app.todoistLang.loading_data;

                SystemTray.SetProgressIndicator(this, progressIndicator);

                app.todoistService.GetTimezone(
                    (result) =>
                    {
                        checkButton.IsEnabled = true;
                        Dispatcher.BeginInvoke(() =>
                            {
                                SearchTxtBox.IsEnabled = true;
                                timezonesListBox.ItemsSource = result.Item;
                                internalTimezoneList = new ObservableCollection<TimezoneItem>(result.Item);
                            });
                    },
                    (error) =>
                    {
                        //@TODO: Tratar exceções
                        Dispatcher.BeginInvoke(() =>
                        {
                            MessageBox.Show(error, "Todoist", MessageBoxButton.OK);
                        });
                    },
                    () =>
                    {
                        progressIndicator.IsVisible = false;
                        progressIndicator.IsIndeterminate = false;
                    });
            }
        }

        private void timezonesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listbox = sender as ListBox;
            TimezoneItem selectedItem = listbox.SelectedItem as TimezoneItem;

            if (selectedItem != null)
            {
                internalTimezone = selectedItem;
                NavigationService.GoBack();
            }

            listbox.SelectedItem = null;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if(e.Content is Page.SignUpPage)
            {
                ((Page.SignUpPage)e.Content).selectedTimezone = internalTimezone;
            }
        }

        private void SearchButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            FilterTimezoneBySearchField();
        }

        private void FilterTimezoneBySearchField()
        {
            var Text = SearchTxtBox.Text;

            if (internalTimezoneList == null)
                return;

            var result = from item in internalTimezoneList
                         where item.Title.ToLowerInvariant().Contains(Text.ToLowerInvariant())
                         select item;

            timezonesListBox.ItemsSource = result;
        }

        private void SearchImage_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            //ChangeColorSearchButton();
        }

        private void SearchImage_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            //ChangeColorSearchButton();
        }
        
        private void SearchTxtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FilterTimezoneBySearchField();
                Focus();
            }
        }

        private void SearchTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Text == string.Empty)
                Dispatcher.BeginInvoke(() =>
                {
                    timezonesListBox.ItemsSource = internalTimezoneList;
                });
        }

    }
}*/