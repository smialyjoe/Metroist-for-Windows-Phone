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
    public partial class EditProject : PhoneApplicationPage
    {
        App app = Application.Current as App;

        int MaxBasicColors = 11;
        int MaxPremiumColors = 21;

        Border lastTapped = null;
        Thickness thickColorSelected = new Thickness(3);
        Thickness thickColorUnselected = new Thickness(0);

        List<int> colorsToShow = new List<int>();

        ApplicationBarIconButton doneIconButton = GeneralLib.Utils.createDoneButton("done");

        public static Project projSelected { get; set; }

        public EditProject()
        {
            InitializeComponent();

            DataContext = projSelected;

            CreateApplicationBar();

            int MaxLoopColorSelection = app.loginInfo.is_premium ? MaxPremiumColors : MaxBasicColors;

            for (int i = 0; i < MaxLoopColorSelection; i++) colorsToShow.Add(i);

            ColorPickerListBox.ItemsSource = colorsToShow;

        }

        private void SelectProjectColor()
        {
            SearchElementTapIt(ColorPickerListBox);

            ColorPickerListBox.SelectedItem = ColorPickerListBox.Items[projSelected.color];
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

            app.service.EditProject(cmdTimeGenerated, projSelected,
            (data) =>
            {
                Utils.DateTimeToUnixTimestamp(cmdTimeGenerated).ToString();
                projSelected.id = data.TempIdMapping[tempID];

                //MainTodoistPage.updateProjectList(data.Projects);

                MainTodoistPage.showMessage = (progress) =>
                {
                    Utils.ProgressIndicatorStatus(String.Format("\"{0}\" added.", projSelected.name), progress);
                };
            },
            (errorMsg) =>
            {
                MessageBox.Show(Utils.Message(errorMsg), "Metroist", MessageBoxButton.OK);
            },
            () =>
            {

            });

            NavigationService.GoBack();
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

        //SearchElement populates above variables for checkboxes in specified "targeted_control"
        private void SearchElementTapIt(DependencyObject targeted_control)
        {
            var count = VisualTreeHelper.GetChildrenCount(targeted_control);   // targeted_control is the listbox
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(targeted_control, i);
                    if (child is Border) // specific/child control 
                    {
                        Border targeted_element = (Border)child;

                        if (targeted_element.Tag != null && targeted_element.Tag.ToString() == projSelected.color.ToString())
                        {
                            ColorPickItemTap(targeted_element, null);
                        }
                        else
                        {
                            SearchElementTapIt(child);
                        }
                    }
                    else
                    {
                        SearchElementTapIt(child);
                    }
                }
            }
            else
            {
                return;
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            SelectProjectColor();

            ToogleDoneButton();
        }
    }
}
