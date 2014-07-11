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
using MetroistLib.Model;
using Newtonsoft.Json;
using Metroist.Pages;

namespace Metroist
{
    public partial class AddTask : PhoneApplicationPage
    {
        ApplicationBarIconButton doneButton = GeneralLib.Utils.createDoneButton("done");
        App app = Application.Current as App;
        
        public static Project projectSelected = null;

        public static string DateTimeFromChooser = string.Empty;

        public AddTask()
        {
            InitializeComponent();

            CreateApplicationBar();

            DataContext = projectSelected;
        }

        private void CreateApplicationBar()
        {
            Color BackgroundColor = ((SolidColorBrush)
                    new Converter.ConverterProjectColor().Convert(projectSelected.color, null, null, null)).Color;

            if (projectSelected.color == 20)
                ApplicationBar = Utils.CreateApplicationBar(BackgroundColor, (Color)App.Current.Resources["WhiteColor2"]);
            else
                ApplicationBar = Utils.CreateApplicationBar(BackgroundColor); 

            doneButton.Click += doneButton_Click;

            ApplicationBar.Buttons.Add(doneButton);

            ApplicationBar.IsVisible = true;
        }

        void doneButton_Click(object sender, EventArgs e)
        {
            var commandTimeGenerated = DateTime.Now;

            QueryDataItem Task = new QueryDataItem
            {
                content = ContentTextBox.Text,
                date_string = DateStringTextBox.Text,
                project_id = projectSelected.id,
                project_name = projectSelected.name
            };

            doneButton.IsEnabled = false;

            app.service.AddTaskToProject(commandTimeGenerated, Task,
            (data) =>
            {
                projectSelected.items.Add(Task);

                var tempID = Utils.DateTimeToUnixTimestamp(commandTimeGenerated).ToString();
                Task.id = data.TempIdMapping[tempID];

                MainTodoistPage.updateProjectList(data.Projects);
                
                ProjectDetail.showMessage = (progress) =>
                {
                    Utils.ProgressIndicatorStatus(String.Format("\"{0}\" added.", Task.content), progress);
                    ProjectDetail.showMessage = null;
                };
            },
            (errorMsg) =>
            {
                MessageBox.Show(Utils.Message(errorMsg), "Metroist", MessageBoxButton.OK);
            },
            () =>
            {
                doneButton.IsEnabled = true;

                var currentPage = app.RootFrame.Content as PhoneApplicationPage;

                if (currentPage == this)
                    NavigationService.GoBack();
            });

            //NavigationService.GoBack();
        }

        private void CalendarButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(Utils.DateTimeChooserPage());
        }

        private void CalendarButton_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = (Color) App.Current.Resources["CalendarBackDarkGray"];

            CalendarButton.Background = brush;
        }

        private void CalendarButton_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = (Color)App.Current.Resources["CalendarBackNormalGray"];

            CalendarButton.Background = brush;
        }
    }
}
