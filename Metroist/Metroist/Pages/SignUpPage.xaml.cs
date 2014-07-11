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
using WatermarkedTextBoxControl;

namespace Metroist
{
    public partial class SignUpPage : PhoneApplicationPage
    {
        ApplicationBarIconButton doneBtn;
        App app = Application.Current as App;
        Brush pressedBrush = GeneralLib.Utils.ConvertStringToColor("#9D9D9D");
        Brush releasedBrush = GeneralLib.Utils.ConvertStringToColor("#00000000");

        Brush blackBrush = GeneralLib.Utils.ConvertStringToColor("#000000");
        Brush oldBrush;

        public SignUpPage()
        {
            InitializeComponent();

            doneBtn = GetDoneBtnReference();
            ControlDoneButtonEnabled();

            oldBrush = NameTextBox.Foreground;
        }

        private ApplicationBarIconButton GetDoneBtnReference()
        {
            foreach (ApplicationBarIconButton btn in ApplicationBar.Buttons)
                if (btn.Text.Contains("Done"))
                    return btn;
            return null;
        }

        private void ControlDoneButtonEnabled()
        {
            bool controlEnabled = true;
            controlEnabled = !string.IsNullOrEmpty(NameTextBox.Text) &&
               !string.IsNullOrEmpty(EmailTextBox.Text) && !string.IsNullOrEmpty(PasswordBox.Password);
            doneBtn.IsEnabled = controlEnabled;
        }

        private void PasswordLostFocus(object sender, RoutedEventArgs e)
        {
            CheckPasswordWatermark();
        }

        public void CheckPasswordWatermark()
        {
            var passwordEmpty = string.IsNullOrEmpty(PasswordBox.Password);
            PasswordWatermark.Opacity = passwordEmpty ? 100 : 0;
            PasswordBox.Opacity = passwordEmpty ? 0 : 100;
        }

        private void PasswordGotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox.Style = null;
            PasswordWatermark.Opacity = 0;
            PasswordBox.Opacity = 100;
        }

        private void BackFindTimezoneBtn_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            Grid grid = sender as Grid;

            grid.Background = pressedBrush;
        }

        private void BackFindTimezoneBtn_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Grid grid = sender as Grid;

            grid.Background = releasedBrush;
        }

        private void TimezoneBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(Utils.ChooseTimezonePage());
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ControlDoneButtonEnabled();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ControlDoneButtonEnabled();
        }

        private void LostFocus_TextBox(object sender, RoutedEventArgs e)
        {
            WatermarkedTextBox textBox = (WatermarkedTextBox)sender;
            textBox.Foreground = textBox.Text == string.Empty ? oldBrush : blackBrush;
        }

        private void GotFocus_TextBox(object sender, RoutedEventArgs e)
        {
            WatermarkedTextBox textBox = (WatermarkedTextBox)sender;
            textBox.Foreground = blackBrush;
        }

        private void DoneBtn_Click(object sender, EventArgs e)
        {
            doneBtn.IsEnabled = false;

            if (validateForm())
            {
                app.service.SignUp(NameTextBox.Text, EmailTextBox.Text, PasswordBox.Password,
                (data) =>
                {
                    app.loginInfo = data;
                    NavigationService.Navigate(Utils.MainTodoistPage(true));
                },
                (errorMsg) =>
                {
                    MessageBox.Show(errorMsg, "Metroist", MessageBoxButton.OK);
                    doneBtn.IsEnabled = true;
                });
            }
            else
            {
                if (!(GeneralLib.Utils.IsValidEmail(EmailTextBox.Text)))
                {
                    MessageBox.Show("You have entered a invalid e-mail. Please try again.", "Metroist", MessageBoxButton.OK);
                }
                else if (!(PasswordBox.Password.Length >= 5))
                {
                    MessageBox.Show("Your password must have at least 5 characters.", "Metroist", MessageBoxButton.OK);
                }

                doneBtn.IsEnabled = true;
            }
        }

        private bool validateForm()
        {
            return PasswordBox.Password.Length >= 5 && GeneralLib.Utils.IsValidEmail(EmailTextBox.Text);
        }    
    }
}
