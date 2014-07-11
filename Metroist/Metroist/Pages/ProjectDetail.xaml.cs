using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MetroistLib.Model;
using System.Globalization;
using System.Windows.Navigation;
using GeneralLib;
using System.Windows.Media;
using Metroist.Pages;
using System.Collections.Generic;

namespace Metroist
{
    public partial class ProjectDetail : PhoneApplicationPage
    {
        ApplicationBarIconButton taskDetailsIconButton = GeneralLib.Utils.createDetailsButton("task details");
        ApplicationBarIconButton addTaskIconButton = GeneralLib.Utils.createAddButton("add task");
        ApplicationBarIconButton deleteIconButton = GeneralLib.Utils.createTrashButton("delete project");
        ApplicationBarIconButton editIconButton = GeneralLib.Utils.createEditButton("edit project");

        QueryDataItem beforeElementSelected = null;
        App app = Application.Current as App;

        ProgressIndicator progress = new ProgressIndicator { IsVisible = false, IsIndeterminate = true };

        public static Action<ProgressIndicator> showMessage = null;

        public static Project projectSelected = null;

        public ProjectDetail()
        {
            InitializeComponent();

            SystemTray.SetProgressIndicator(this, progress);

            if (projectSelected == null)
                throw new Exception("Error 001: No project selected here. This is weird!");

            DataContext = projectSelected;
            CreateApplicationBar();
            AssignClickEventsApplicatioBar();
        }

        private void AssignClickEventsApplicatioBar()
        {
            addTaskIconButton.Click += (sender, e) =>
            {
                NavigationService.Navigate(Utils.AddTaskPage(projectSelected));
                UncompletedTasksListBox.SelectedItem = null;
            };

            taskDetailsIconButton.Click += (sender, e) =>
            {
                if (UncompletedTasksListBox.SelectedItem != null)
                {
                    NavigationService.Navigate(Utils.TaskDetailPage(UncompletedTasksListBox.SelectedItem as QueryDataItem));
                    UncompletedTasksListBox.SelectedItem = null;
                }
            };

            editIconButton.Click += new EventHandler(editIconButton_Click);
            deleteIconButton.Click += deleteIconButton_Click;
        }

        void editIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(Utils.EditProjectPage(projectSelected));
        }

        private void CreateApplicationBar()
        {
            var BackgroundColor = ((SolidColorBrush)
                    new Converter.ConverterProjectColor().Convert(projectSelected.color, null, null, null)).Color;

            if(projectSelected.color == 20)
                ApplicationBar = Utils.CreateApplicationBar(BackgroundColor, (Color)App.Current.Resources["WhiteColor2"]); 
            else
                ApplicationBar = Utils.CreateApplicationBar(BackgroundColor); 

            ApplicationBar.Buttons.Add(addTaskIconButton);

            ApplicationBar.IsVisible = true;
        }

        void deleteIconButton_Click(object sender, EventArgs e)
        {
            var result = 
                MessageBox.Show(string.Format("Delete project \"{0}\"?", projectSelected.name), "Metroist", MessageBoxButton.OKCancel);

            //@TODO: check what is the answers. if the deleted projects come also.
            if (result == MessageBoxResult.OK)
            {
                var cmdTimeGenerated = DateTime.Now;
                var tempID = Utils.DateTimeToUnixTimestamp(cmdTimeGenerated).ToString();

                if (projectSelected.last_updated == 0.0)
                {
                    //@TODO: Check if there is a unsynchroned project with the same name.
                    //There isn't another way to check if the project wasn't sync instead of checking by name
                    //I will assume that the user never create a project with two names intentionally.
                }
                else
                {
                    deleteIconButton.IsEnabled = false;

                    app.service.RemoveProject(cmdTimeGenerated, projectSelected,
                    (data) =>
                    {
                        app.projects.Remove(projectSelected);

                        MainTodoistPage.showMessage = (progress) =>
                        {
                            Utils.ProgressIndicatorStatus(String.Format("\"{0}\" deleted.", projectSelected.name), progress);
                        };
                    },
                    (errorMsg) =>
                    {
                        MessageBox.Show(Utils.Message(errorMsg), "Metroist", MessageBoxButton.OK);
                    },
                    () =>
                    {
                        deleteIconButton.IsEnabled = true;

                        var currentPage = app.RootFrame.Content as PhoneApplicationPage;

                        if (currentPage == this)
                            NavigationService.GoBack();
                    });
                }
                
            }
        }

        private void UncompletedTasksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;

            if (listBox.SelectedItem != null)
            {
                var selected = listBox.SelectedItem as MetroistLib.Model.QueryDataItem;
                selected.selectedListBoxItem_ProjectDetail = true;

                //previous element checkbox needs to be hidden
                if (beforeElementSelected != null)
                {
                    beforeElementSelected.selectedListBoxItem_ProjectDetail = false;
                    beforeElementSelected = selected;
                }
                else
                {
                    beforeElementSelected = selected;
                }

                //Adding button for details of a task selected
                if (ApplicationBar != null && !ApplicationBar.Buttons.Contains(taskDetailsIconButton))
                {
                    ApplicationBar.Buttons.Add(taskDetailsIconButton);
                }
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            UpdateTasksFromProject();
            //CreateApplicationBar();

            var listBoxSelectedItem = (UncompletedTasksListBox.SelectedItem as QueryDataItem);

            //Update applicationbariconbuttons of ApplicationBar
            Pivot pivot = MainPivot as Pivot;
            if (pivot.SelectedItem != null)
            {
                PivotItem item = pivot.SelectedItem as PivotItem;

                ApplicationBar.Buttons.Clear();

                if (item.Header.ToString() == "tasks")
                {
                    ApplicationBar.Buttons.Add(addTaskIconButton);
                    if (UncompletedTasksListBox.SelectedItem != null)
                        ApplicationBar.Buttons.Add(taskDetailsIconButton);
                }
                else if (item.Header.ToString() == "details")
                {
                    //ApplicationBar.Buttons.Add(editIconButton);
                    ApplicationBar.Buttons.Add(deleteIconButton);
                }
            }

            if (beforeElementSelected != null)
            {
                UncompletedTasksListBox.SelectedItem = beforeElementSelected;
                beforeElementSelected.selectedListBoxItem_ProjectDetail = true;
            }

            if (showMessage != null)
                showMessage(progress);
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            //Clean all selection accents.
            var listBoxSelectedItem = (UncompletedTasksListBox.SelectedItem as QueryDataItem);

            if (e.NavigationMode == NavigationMode.Back)
                if(listBoxSelectedItem != null)
                    listBoxSelectedItem.selectedListBoxItem_ProjectDetail = false;
        }

        private void UpdateTasksFromProject()
        {
            //app.service.GetTaskFromProject(projectSelected.id,
            //(tasks) =>
            //{
            //    UncompletedTasksListBox.ItemsSource = tasks;
            //},
            //(error) =>
            //{
            //    MessageBox.Show(error, "Metroist", MessageBoxButton.OK);
            //},
            //() =>
            //{
            //});

            UncompletedTasksListBox.ItemsSource = null;
            UncompletedTasksListBox.ItemsSource = projectSelected.items;
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot pivot = sender as Pivot;

            if (pivot.SelectedItem != null)
            {
                PivotItem item = pivot.SelectedItem as PivotItem;

                ApplicationBar.Buttons.Clear();

                if(item.Header.ToString() == "tasks")
                {
                    ApplicationBar.Buttons.Add(addTaskIconButton);
                    if (UncompletedTasksListBox.SelectedItem != null) 
                        ApplicationBar.Buttons.Add(taskDetailsIconButton);
                }
                else if (item.Header.ToString() == "details")
                {
                    //ApplicationBar.Buttons.Add(editIconButton);
                    ApplicationBar.Buttons.Add(deleteIconButton);
                }
            }
        }

        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            var cmdTime = DateTime.Now;

            QueryDataItem selected = UncompletedTasksListBox.SelectedItem as QueryDataItem;

            //selected.selectedListBoxItem_ProjectDetail = true;
            if (selected != null)
            {
                projectSelected.cache_count--;
                selected.is_checked = true;

                app.service.SetTaskAsChecked(cmdTime, selected,
                (data) =>
                {
                    if (MainTodoistPage.updateProjectList != null)
                        MainTodoistPage.updateProjectList(data.Projects);
                }, 
                (erroMsg) =>
                {
                    //@TODO: What to do here? error method
                },
                () =>
                {

                });
            }
        }

        private void UncompleteTask_Click(object sender, RoutedEventArgs e)
        {
            QueryDataItem selected = UncompletedTasksListBox.SelectedItem as QueryDataItem;

            //selected.selectedListBoxItem_ProjectDetail = true;
            if (selected != null)
                selected.is_checked = false;
        }
    }

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(Visibility.Visible);
        }
    }
}
