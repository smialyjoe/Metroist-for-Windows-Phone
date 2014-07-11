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
using Metroist.Pages;
using MetroistLib.Model;
using System.Text.RegularExpressions;

namespace Metroist
{
    public partial class TaskDetail : PhoneApplicationPage
    {
        public static MetroistLib.Model.QueryDataItem Task = null;
        public static Project Project;
        public App app = Application.Current as App;

        private bool taskDeleted = false;

        private ApplicationBarIconButton addNoteIconButton = GeneralLib.Utils.createAddButton("add note");
        private ApplicationBarIconButton deleteIconButton = GeneralLib.Utils.createTrashButton("delete");
        private ApplicationBarIconButton checkUncheckButton;

        ProgressIndicator progress = new ProgressIndicator { IsVisible = false, IsIndeterminate = true };

        public static bool needsUpdateView = false;

        public TaskDetail()
        {
            InitializeComponent();

            CreateApplicationBar();

            SystemTray.SetProgressIndicator(this, progress);

            UpdateView();

            SwitchButtonsFromAppBar();
        }

        private void extractUrl(List<Note> list)
        {
            foreach (Note n in list)
            {
                string url = DevolveURL(n.content);

                if (!string.IsNullOrWhiteSpace(url))
                {
                    n.content = n.content.Replace(url, "");
                    n.content = n.content.Trim();
                    int indexOfOpen = n.content.IndexOf("(");
                    if (indexOfOpen != -1)
                    {
                        n.content = n.content.Substring(indexOfOpen + 1);
                        int indexOfClose = n.content.IndexOf(")");
                        if (indexOfClose != -1)
                            n.content = n.content.Substring(0, indexOfClose);
                    }
                }
            }
        }

        public string DevolveURL(string texto)
        {
            Regex regx = new Regex("https://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);
            Match url = regx.Match(texto);
            if (url != null)
            {
                if (url.Value != string.Empty)
                    return url.Value;
                else
                    return string.Empty;
            }
            else
                return string.Empty;
        }

        private void CreateApplicationBar()
        {
            var project = (from proj in app.projects
                            where proj.id == Task.project_id
                            select proj).FirstOrDefault();

            if (project != null)
            {
                var BackgroundColor = ((SolidColorBrush)
                    new Converter.ConverterProjectColor().Convert(project.color, null, null, null)).Color;

                if (project.color == 20)
                    ApplicationBar = Utils.CreateApplicationBar(BackgroundColor, (Color)App.Current.Resources["WhiteColor2"]);
                else
                    ApplicationBar = Utils.CreateApplicationBar(BackgroundColor); 
            }
            
            deleteIconButton.Click += new EventHandler(deleteIconButton_Click);
            addNoteIconButton.Click += new EventHandler(addNoteIconButton_Click);

            ApplicationBar.IsVisible = true;
        }

        void addNoteIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(Utils.AddNotePage(Project, Task));
        }

        private void SwitchButtonsFromAppBar()
        {
            if (tasksPivot.SelectedItem == DetailsPivotItem)
            {
                checkUncheckButton =
                    Task.is_checked ?
                        GeneralLib.Utils.createUncheckButton("uncheck") :
                        GeneralLib.Utils.createCheckButton("check");

                checkUncheckButton.Click += new EventHandler(checkUncheckButton_Click);

                ApplicationBar.Buttons.Add(checkUncheckButton);
                ApplicationBar.Buttons.Add(deleteIconButton);
            }
            else if (tasksPivot.SelectedItem == NotesPivotItem)
            {
                ApplicationBar.Buttons.Add(addNoteIconButton);
            }
        }

        void deleteIconButton_Click(object sender, EventArgs e)
        {
            if(Task!=null)
            {
                var result = MessageBox.Show(String.Format("Delete task \"{0}\"?", Task.content), "Metroist", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    var project = (from proj in app.projects
                                   where proj.id == Task.project_id
                                   select proj).FirstOrDefault();

                    if (project != null)
                    {
                        var task = project.items.Where(x => x.id == Task.id).FirstOrDefault();

                        if (task != null)
                        {
                            var indexOf = project.items.IndexOf(task);
                            UpdateDeletingToServer(project, indexOf);
                        }
                        else { throw new Exception("This is weird! Task was not found!"); }
                    }
                    else
                    {
                        //throw new Exception("This is weird! No proj. found at deleting a task");
                    }
                }
            }
            else
            {
                while(NavigationService.CanGoBack) NavigationService.GoBack();
            }
        }

        private void UpdateDeletingToServer(Project project, int indexOf)
        {
            var commandTimeGenerated = DateTime.Now;

            deleteIconButton.IsEnabled = false;

            app.service.RemoveTask(commandTimeGenerated, Task,
            (data) =>
            {
                project.items.RemoveAt(indexOf);
                project.cache_count--;
                taskDeleted = true;

                MainTodoistPage.updateProjectList(data.Projects);
            },
            (errorMsg) =>
            {
                //@TODO: Error dynamic
            },
            () =>
            {
                deleteIconButton.IsEnabled = true;

                var currentPage = app.RootFrame.Content as PhoneApplicationPage;

                if (currentPage == this)
                    NavigationService.GoBack();
            });
        }

        void checkUncheckButton_Click(object sender, EventArgs e)
        {
            var cmdTime = DateTime.Now;

            QueryDataItem selected = Task as QueryDataItem;

            //selected.selectedListBoxItem_ProjectDetail = true;
            if (selected != null)
            {   
                var project = (from proj in app.projects
                                   where proj.id == Task.project_id
                                   select proj).FirstOrDefault();

                if (project != null)
                {
                    project.cache_count--;
                    selected.is_checked = true;
                }

                //Update icon to Unchecked
                var newButton = (sender as ApplicationBarIconButton);

                newButton.IconUri =
                    new Uri("/Images/" + (selected.is_checked ? "Uncheck.png" : "Check.png"), UriKind.Relative);

                newButton.Text = selected.is_checked ? "uncheck" : "check done";

                DataContext = null;
                DataContext = Task;
                //-->

                app.service.SetTaskAsChecked(cmdTime, selected,
                (data) =>
                {
                    if (MainTodoistPage.updateProjectList != null)
                        MainTodoistPage.updateProjectList(data.Projects);
                },
                (erroMsg) =>
                {
                    //@TODO: What to do here? error method
                    MessageBox.Show(erroMsg, "Metroist", MessageBoxButton.OK);
                },
                () =>
                {

                });
            }

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (needsUpdateView)
            {
                UpdateView();
            }
        }

        private void UpdateView()
        {
            Task.notes = Task.notes.OrderByDescending(e => DateTime.Parse(e.posted)).ToList();
            extractUrl(Task.notes);
            DataContext = null;
            DataContext = Task;
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (taskDeleted)
            {
                if (e.Uri.ToString().Contains("ProjectDetail"))
                {
                    ProjectDetail.showMessage = (progress) =>
                    {
                        Utils.ProgressIndicatorStatus("Deleted.", progress);
                        ProjectDetail.showMessage = null;
                    };
                }
                else if (e.Uri.ToString().Contains("MainTodoistPage"))
                {
                    MainTodoistPage.showMessage = 
                    (progress) =>
                    {
                        Utils.ProgressIndicatorStatus("Deleted.", progress);
                        MainTodoistPage.showMessage = null;
                    };
                }

                taskDeleted = false;
            }
        }

        private void tasksPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationBar.Buttons.Clear();
            SwitchButtonsFromAppBar();
        }
    }
}
