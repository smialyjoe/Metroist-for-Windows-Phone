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
using System.Windows.Navigation;
using Microsoft.Phone.Shell;
using MetroistLib.Model;
using Microsoft.Phone.Net.NetworkInformation;

namespace Metroist.Pages
{
    public partial class MainTodoistPage : PhoneApplicationPage
    {
        ApplicationBar mainApplicationBar = Utils.CreateApplicationBar();
        ApplicationBarMenuItem logoutMenuItem = new ApplicationBarMenuItem();
        ApplicationBarMenuItem aboutMenuItem = new ApplicationBarMenuItem();
        ApplicationBarIconButton filterButton = GeneralLib.Utils.createFilterButton("filter");
        ApplicationBarIconButton syncButton = GeneralLib.Utils.createRefreshButton("sync all");
        ApplicationBarIconButton addProjectIconButton = GeneralLib.Utils.createAddButton("add project");
        ApplicationBarIconButton updateAllIconButton = GeneralLib.Utils.createDownloadButton("update all");

        static ProgressIndicator progressIndicator = new ProgressIndicator { IsIndeterminate = true, IsVisible = false };

        private static Action<ProgressIndicator> _showMessage = null;
        public static Action<ProgressIndicator> showMessage
        {
            get
            {
                return _showMessage;
            }

            set
            {
                _showMessage = value;

                if (value != null)
                    value(progressIndicator);
            }
        }

        public static Action updateStartPage = null;
        public static Action<List<Project>> updateProjectList = null;
        public static bool removeBackStack = true;
        public static bool mustShowMessage = true;

        //Flag: Check if filter was changed
        public static bool changedFilter = false;

        App app = Application.Current as App;
        public MainTodoistPage()
        {
            InitializeComponent();

            SystemTray.SetProgressIndicator(this, progressIndicator);

            //Create applicationbar and buttons
            ApplicationBarCreate();

            //Update everything here
            GetData();

            //What happens after sync
            SetCallbackForSyncEvent();

            DataContext = app;

            //Set action of updateStartPage, this action will work when another page request startpage update
            updateStartPage = GetStartPage;
            updateProjectList = (projs) =>
            {
                app.projects = projs;
                ProjectsListBox.ItemsSource = null;
                ProjectsListBox.ItemsSource = app.projects;

                updateStartPage();
            };

            //Update viewer about network connection
            HandleNetworkStatusViewer();

            //Assigning to network changed event
            DeviceNetworkInformation.NetworkAvailabilityChanged +=
                new EventHandler<NetworkNotificationEventArgs>(DeviceNetworkInformation_NetworkAvailabilityChanged);
        }

        private void RemoveBackStack()
        {
            if (removeBackStack)
            {
                JournalEntry pageBackStack;
                do
                {
                    pageBackStack = NavigationService.RemoveBackEntry();
                }
                while (pageBackStack != null);
            }
        }

        private void HandleNetworkStatusViewer()
        {
            Dispatcher.BeginInvoke(() =>
            {
                BlockColorNetworkStatus.Background = NetworkInterface.GetIsNetworkAvailable() && app.service.debugWithInternet ?
                    App.Current.Resources["ProjectColor15"] as SolidColorBrush :
                    App.Current.Resources["ProjectColor13"] as SolidColorBrush;

                LabelNetworkStatus.Text = NetworkInterface.GetIsNetworkAvailable() && app.service.debugWithInternet ?
                    "online" : "offline";
            });
        }

        void DeviceNetworkInformation_NetworkAvailabilityChanged(object sender, NetworkNotificationEventArgs e)
        {
            HandleNetworkStatusViewer();
        }

        private void SetCallbackForSyncEvent()
        {
            app.TemporaryDesynchronized.CollectionChanged += (sender, e) =>
            {
                if (app.TemporaryDesynchronized.Count > 0)
                    SyncStatusLabel.Text = String.Format("{0} item(s) to be sync.", app.TemporaryDesynchronized.Count);
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            RemoveBackStack();

            DataContext = null;
            DataContext = app;

            if (changedFilter)
            {
                changedFilter = false;
                GetStartPage();
            }

            ProjectsListBox.ItemsSource = null;
            ProjectsListBox.ItemsSource = app.projects;

            if (showMessage != null)
            {
                showMessage(progressIndicator);
            }

            //Check network connection here too
            HandleNetworkStatusViewer();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            showMessage = null;
        }

        private void ApplicationBarCreate()
        {
            ApplicationBar = mainApplicationBar;
            mainApplicationBar.IsVisible = true;

            updateAllIconButton.IsEnabled = false;

            updateAllIconButton.Click += new EventHandler(updateAllIconButton_Click);

            addProjectIconButton.Click += new EventHandler(addProjectIconButton_Click);

            filterButton.Click += (sender, e) =>
            {
                NavigationService.Navigate(Utils.FilterOptionsPage());
            };

            aboutMenuItem.Text = "about";
            aboutMenuItem.Click += (sender, e) =>
            {
                NavigationService.Navigate(Utils.AboutPage());
            };

            logoutMenuItem.Text = "logout";
            logoutMenuItem.Click += (sender, e) =>
            {
                app.service.cancel();
                app.service = new TodoistService();

                app.loginInfo = null;
                app.localLoginInfo.isRecorded = false;
                app.localLoginInfo.email = app.localLoginInfo.password = null;

                if (app.projects != null)
                    app.projects.Clear();

                if (app.startPageTasks != null)
                    app.startPageTasks.Clear();

                if (app.TemporaryDesynchronized != null)
                    app.TemporaryDesynchronized.Clear();

                GeneralLib.IsolatedStorage.clear();

                NavigationService.Navigate(Utils.LoginPage());
            };

            syncButton.IsEnabled = false;
            syncButton.Click += new EventHandler(syncButton_Click);

            mainApplicationBar.MenuItems.Add(logoutMenuItem);
            mainApplicationBar.MenuItems.Add(aboutMenuItem);
        }

        void updateAllIconButton_Click(object sender, EventArgs e)
        {
            GetData();
        }

        void syncButton_Click(object sender, EventArgs e)
        {
            if (app.TemporaryDesynchronized.Count > 0)
            {
                app.service.SyncAll(
                    (data) =>
                    {
                        if (app.TemporaryDesynchronized.Count > 0)
                        {
                            //Update all based on temporary
                            //This is needed because the responde doesn't bring the synced item or project.
                            UpdateBasedOnTemporaryDessychronized(data);

                            app.TemporaryDesynchronized.Clear();

                            SyncStatusLabel.Text = String.Format("{0} item(s) to be sync.", app.TemporaryDesynchronized.Count);
                        }

                        app.projects = data.Projects;
                    },
                    (errorMsg) =>
                    {
                        MessageBox.Show(errorMsg, "Metroist", MessageBoxButton.OK);
                    },
                    () =>
                    {
                        ProjectsListBox.ItemsSource = null;
                        ProjectsListBox.ItemsSource = app.projects;
                    });
            }
        }

        void addProjectIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(Utils.AddProjectPage());
        }

        private void GetData()
        {
            if (app.projects != null && app.projects.Count() > 0)
            {
                UpdateFilterTasksAndProjects();
            }

            if (app.TemporaryDesynchronized.Count > 0)
                SyncStatusLabel.Text = String.Format("{0} item(s) to be sync.", app.TemporaryDesynchronized.Count);

            progressIndicator.Text = "Updating data";

            updateAllIconButton.IsEnabled = false;

            app.service.SyncAndGetUpdated(progressIndicator,
            (data) =>
            {
                UpdateBasedOnTemporaryDessychronized(data);

                app.projects = data.Projects;
            },
            (error) =>
            {
                MessageBox.Show(error, "Metroist", MessageBoxButton.OK);
            },
            () =>
            {
                UpdateFilterTasksAndProjects();
            },
            () =>
            {
                updateAllIconButton.IsEnabled = true;
            });

            HandleNetworkStatusViewer();
        }

        private void UpdateFilterTasksAndProjects()
        {
            ProjectsListBox.ItemsSource = null;
            ProjectsListBox.ItemsSource = app.projects;

            //@TODO [1]: Generating duplicated requests
            GetStartPage();
        }

        private void UpdateBasedOnTemporaryDessychronized(Data data)
        {
            if (app.TemporaryDesynchronized.Count > 0)
            {
                var addProjects = app.TemporaryDesynchronized.Where(x => (x["type"] as string).Contains("project_add")).ToList();
                var removeProjects = app.TemporaryDesynchronized.Where(x => (x["type"] as string).Contains("project_delete")).ToList();

                if (addProjects.Count > 0)
                {
                    foreach (var addProject in addProjects)
                    {
                        Project Project = new Project
                        {
                            name = (string)(addProject["args"] as Dictionary<string, object>)["name"],
                            color = (int)(addProject["args"] as Dictionary<string, object>)["color"],
                            items = new List<QueryDataItem>()
                        };

                        string tempKey = (addProject["temp_id"]).ToString();

                        if (data.TempIdMapping.ContainsKey(tempKey))
                            Project.id = data.TempIdMapping[tempKey];

                        data.Projects.Add(Project);
                    }
                }

                if (removeProjects.Count > 0)
                {
                    foreach (var removeProject in removeProjects)
                    {
                        var dicTempArgs = removeProject["args"] as Dictionary<string, object>;

                        var tempStringIdProjects = (dicTempArgs["ids"] as string).Replace("[", string.Empty).Replace("]", string.Empty).Split(',');

                        List<int> idProjects = new List<int>();

                        foreach (var itemStringID in tempStringIdProjects)
                        {
                            idProjects.Add(Int32.Parse(itemStringID));
                        }

                        if (idProjects.Count > 1)
                        {
                            throw new Exception("Multiple deletions is not implemented yet.");
                        }

                        var realProjectToRemove = data.Projects.Where(x => x.id == idProjects.First()).FirstOrDefault();

                        if (realProjectToRemove != null)
                        {
                            data.Projects.Remove(realProjectToRemove);
                        }
                        else
                        {
                            //Project was already remove at server, another words, do nothing :)
                        }
                    }
                }
            }
        }

        private void GetStartPage()
        {
            app.service.GetStartPage
            (app.settings.DateStringHome,
            (data) =>
            {
                //@TODO: Change this for more flexibility.
                ApplyStartPage(data);
            },
            (error) =>
            {
                MessageBox.Show(error);
            },
            () =>
            {

            });
        }

        private void ApplyStartPage(List<QueryDataItem> data)
        {
            NoTasksLabel.Visibility = data.Count > 0 ? Visibility.Collapsed : Visibility.Visible;

            //StartPageListBox.ItemTemplate = Resources["ItemListTaskTemplateListBox"] as DataTemplate;
            StartPageListBox.ItemsSource = data;

            //if (data.Count() > 0)
            //{
            //    //if (data[0].item.Count() > 0 && data[0].item[0].project_name != null)
            //    //{
            //    //    StartPageListBox.ItemTemplate = Resources["QueryProjectTemplateListBox"] as DataTemplate;
            //    //    StartPageListBox.ItemsSource = data[0].item;
            //    //}
            //    //else
            //    //{
            //        StartPageListBox.ItemTemplate = Resources["ItemListTaskTemplateListBox"] as DataTemplate;
            //        StartPageListBox.ItemsSource = data;
            //    //}

            //    //NoTasksLabel.Visibility = data.Any(x=>x.item.Count() != 0) ? Visibility.Collapsed: Visibility.Visible;

            //    NoTasksLabel.Visibility = data.Any(x => x.item.Count() != 0) ? Visibility.Collapsed : Visibility.Visible;

            //    //app.startPageTasks = data;
            //}
        }

        private void StartPageItemListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;

            if (listBox.SelectedItem != null)
            {
                var selected = listBox.SelectedItem as MetroistLib.Model.QueryDataItem;

                NavigationService.Navigate(Utils.TaskDetailPage(selected));

                listBox.SelectedItem = null;
            }
        }

        private void ProjectsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;

            if (listBox.SelectedItem != null)
            {
                var selected = listBox.SelectedItem as MetroistLib.Model.Project;

                NavigationService.Navigate(Utils.ProjectDetailPage(selected));

                listBox.SelectedItem = null;
            }
        }

        private void Panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Panorama panoramaContainer = sender as Panorama;

            if (panoramaContainer.SelectedItem != null)
            {
                var panoramaSelected = panoramaContainer.SelectedItem as PanoramaItem;

                mainApplicationBar.Buttons.Clear();

                UpdateButtonsByPanoramaItem(panoramaSelected);
            }
        }

        private void UpdateButtonsByPanoramaItem(PanoramaItem panoramaSelected)
        {
            if (panoramaSelected != null && panoramaSelected.Name == "TasksPanoramaItem" && !mainApplicationBar.Buttons.Contains(filterButton))
            {
                mainApplicationBar.Buttons.Add(filterButton);
            }

            if (panoramaSelected != null && panoramaSelected.Name == "StatusPanoramaItem" && !mainApplicationBar.Buttons.Contains(syncButton))
            {
                mainApplicationBar.Buttons.Add(syncButton);
            }

            if (panoramaSelected != null && panoramaSelected.Name == "ProjectsPanoramaItem"
                && !mainApplicationBar.Buttons.Contains(addProjectIconButton)
                && !mainApplicationBar.Buttons.Contains(updateAllIconButton))
            {
                mainApplicationBar.Buttons.Add(updateAllIconButton);
                mainApplicationBar.Buttons.Add(addProjectIconButton);
            }
        }

        private void MainPanorama_Loaded(object sender, RoutedEventArgs e)
        {
            Panorama panoramaContainer = sender as Panorama;

            if (panoramaContainer.SelectedItem != null)
            {
                var panoramaSelected = panoramaContainer.SelectedItem as PanoramaItem;

                UpdateButtonsByPanoramaItem(panoramaSelected);
            }

        }
    }
}