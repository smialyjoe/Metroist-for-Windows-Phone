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
using Metroist.Pages;

namespace Metroist
{
    public partial class AddProject : PhoneApplicationPage
    {
        App app = Application.Current as App;

        int MaxBasicColors = 11;
        int MaxPremiumColors = 21;

        Border lastTapped = null;
        Thickness thickColorSelected = new Thickness(3);
        Thickness thickColorUnselected = new Thickness(0);

        List<int> colorsToShow = new List<int>();

        ApplicationBarIconButton doneIconButton = GeneralLib.Utils.createDoneButton("done") ;

        public AddProject()
        {
            InitializeComponent();

            CreateApplicationBar();

            int MaxLoopColorSelection = app.loginInfo.is_premium ? MaxPremiumColors : MaxBasicColors;

            for (int i = 0; i < MaxLoopColorSelection; i++) colorsToShow.Add(i);

            ColorPickerListBox.ItemsSource = colorsToShow;
        }

        private void CreateApplicationBar()
        {
            ApplicationBar = Utils.CreateApplicationBar();

            doneIconButton.IsEnabled = false;
            doneIconButton.Click += new EventHandler(doneIconButton_Click);

            ApplicationBar.Buttons.Add(doneIconButton);
        }

        void doneIconButton_Click(object sender, EventArgs e)
        {
            var cmdTimeGenerated = DateTime.Now;
            var tempID = Utils.DateTimeToUnixTimestamp(cmdTimeGenerated).ToString();

            Project Project = new Project
            {
                name = projectNameTextBox.Text,
                color = ((int)ColorPickerListBox.SelectedItem),
                items = new List<QueryDataItem>()
            };

            doneIconButton.IsEnabled = false;

            app.service.AddProject(cmdTimeGenerated, Project,
            (data) =>
            {
                app.projects.Add(Project);

                Utils.DateTimeToUnixTimestamp(cmdTimeGenerated).ToString();
                Project.id = data.TempIdMapping[tempID];

                //MainTodoistPage.updateProjectList(data.Projects);

                MainTodoistPage.showMessage = (progress) =>
                {
                    Utils.ProgressIndicatorStatus(String.Format("\"{0}\" added.", Project.name), progress);
                };
            },
            (errorMsg) =>
            {
                MessageBox.Show(Utils.Message(errorMsg), "Metroist", MessageBoxButton.OK);
            },
            () =>
            {
                doneIconButton.IsEnabled = true;
                
                var currentPage = app.RootFrame.Content as PhoneApplicationPage;

                if(currentPage == this)
                    NavigationService.GoBack();
            });

        }

        private void ColorPickItemTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Border gridTapped = sender as Border;

            if (lastTapped == null)
            {
                lastTapped = gridTapped;
                gridTapped.BorderThickness = thickColorSelected;
            }
            else
            {
                lastTapped.BorderThickness = thickColorUnselected;
                lastTapped = gridTapped;
                gridTapped.BorderThickness = thickColorSelected;
            }

        }

        private void projectNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ToogleDoneButton();
        }

        private void ColorPickerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ToogleDoneButton();
        }

        private void ToogleDoneButton()
        {
            doneIconButton.IsEnabled =
                projectNameTextBox.Text != string.Empty && ColorPickerListBox.SelectedItem != null;
        }

    }
}
