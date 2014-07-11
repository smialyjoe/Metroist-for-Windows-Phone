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
using System.Windows.Navigation;
using System.Windows.Media.Imaging;

namespace Metroist.Pages
{
    public partial class Login : PhoneApplicationPage
    {
        App app = Application.Current as App;

        private readonly ApplicationBarIconButton loginButton = GeneralLib.Utils.createDoneButton("login");
        //private readonly static ApplicationBarMenuItem signUpMenuItem = Utils.createSignUpMenuItem();
        private readonly ApplicationBarMenuItem aboutMenuItem = Utils.createAboutMenuItem("about this application");

        ProgressIndicator progressIndicator = new ProgressIndicator();

        bool emailShowHint = true;
        bool pwdShowHint = true;

        private Brush GoogleSignInBtnBackground;
        private Brush GoogleSignInBtnForeground;
        private ImageSource GoogleSignInBtnImgSource;
        private Brush GoogleSignInBtnBorderBrush;

        // Constructor
        public Login()
        {
            InitializeComponent();

            //Forcing light theme for this application;
            ThemeManager.ToLightTheme();

            //@TODO: Review here
            //Apply language
            //DataContext = app.todoistLang;

            //Starting login screen with e-mail field focused;
            EmailTxtBox.Focus();

            //Create the application bar (buttons and menu items)
            ApplicationBarCreate();

            //Disable the login button if text is empty (obviously, here it is);
            ToogleLoginButton();

            KeepGoogleBtnReferences();
        }

        private void KeepGoogleBtnReferences()
        {
            GoogleSignInBtnBackground = GoogleSignInBtn.Background;
            GoogleSignInBtnForeground = GoogleSignInBtn.Foreground;
            GoogleSignInBtnBorderBrush = GoogleSignInBtn.BorderBrush;
            GoogleSignInBtnImgSource = GoogleSignInBtnLeftImage.Source;
        }

        public void ApplicationBarCreate()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBar.Buttons.Clear();
            ApplicationBar.MenuItems.Clear();

            loginButton.Click +=
                (sender, e) =>
                {
                    Logar();
                };

            //signUpMenuItem.Click +=
            //    (sender, e) =>
            //    {
            //        NavigationService.Navigate(Utils.SignUpPage());
            //    };

            aboutMenuItem.Click +=
                (sender, e) =>
                {
                    NavigationService.Navigate(Utils.AboutPage());
                };

            ApplicationBar.Buttons.Add(loginButton);
            //ApplicationBar.MenuItems.Add(signUpMenuItem);
            ApplicationBar.MenuItems.Add(aboutMenuItem);
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            Logar();
        }

        private void TextBoxLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FocusEmptyTextBox();
            }
        }

        private void PasswordBoxLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FocusEmptyTextBox();
            }
        }

        private void FocusEmptyTextBox()
        {
            if (EmailTxtBox.Text == string.Empty)
                EmailTxtBox.Focus();
            else if (PasswordBox.Password == string.Empty)
                PasswordBox.Focus();
            else
                Logar();
        }

        private void Logar()
        {
            //Take off the focus from any.
            Focus();

            ApplicationBar.IsVisible = false;
            OverlayPopup.IsOpen = true;
            ShowOverlay.Begin();

            progressIndicator.IsVisible = true;
            progressIndicator.IsIndeterminate = true;
            progressIndicator.Text = "Connecting to Todoist service";

            signUpLoginBtn.IsEnabled = false;

            SystemTray.SetProgressIndicator(this, progressIndicator);

            loginButton.IsEnabled = false;

            app.service.Login(EmailTxtBox.Text, PasswordBox.Password,
            (data) =>
            {
                NavigationService.Navigate(Utils.MainTodoistPage());
            },
            (error) =>
            {
                loginButton.IsEnabled = true;
                MessageBox.Show(error, "Metroist", MessageBoxButton.OK);
            },
            () =>
            {
                ShowOverlay.Stop();
                OverlayPopup.IsOpen = false;
                progressIndicator.IsIndeterminate = false;
                progressIndicator.IsVisible = false;
                ApplicationBar.IsVisible = true;
                signUpLoginBtn.IsEnabled = true;
            });

            //Empty the passwordbox, just for visual security.
            PasswordBox.Password = string.Empty;

            if (RememberCheckBox.IsChecked == true)
            {
                app.localLoginInfo.isRecorded = true;
                app.localLoginInfo.email = EmailTxtBox.Text;
                app.localLoginInfo.password = PasswordBox.Password;
            }
        }

        private void HandleStyleHintBox(Control email, Control pwd)
        {
            if (emailShowHint)
            {
                email.Style = Resources["HintBox"] as Style;
            }
            else
            {
                email.Style = null;
            }

            if (pwdShowHint)
            {
                pwd.Style = Resources["HintBox"] as Style;
                //pwd2.Style = Resources["HintBox"] as Style;
            }
            else
            {
                pwd.Style = null;
                //pwd2.Style = null;
            }
        }

        private void EmailTxtBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (emailShowHint)
            {
                EmailTxtBox.Text = "";
            }

            EmailTxtBox.Style = null;
        }

        private void EmailTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ToogleLoginButton();
        }

        private void EmailTxtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (EmailTxtBox.Text == string.Empty)
            {
                EmailTxtBox.Text = "Email";
                emailShowHint = true;
            }
            else
            {
                emailShowHint = false;
            }

            HandleStyleHintBox(EmailTxtBox, PasswordBox);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ToogleLoginButton();
        }

        private void ToogleLoginButton()
        {
            loginButton.IsEnabled =
                EmailTxtBox.Text != string.Empty && PasswordBox.Password != string.Empty;
        }

        private void ConnectWithGoogleBtn_Click(object sender, RoutedEventArgs e)
        {
            //var URL = app.service.GetGoogleToken();
            //Page.WebBrowser.URL = URL;
            //NavigationService.Navigate(Utils.WebBrowserPage());
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (app.localLoginInfo != null)
            {
                if (app.loginInfo != null)
                    NavigationService.Navigate(Utils.MainTodoistPage());
            }

            //Logout: Erase all pages on backstack
            if (e.Content is Login)
            {
                JournalEntry pageBackStack;
                do
                {
                    pageBackStack = NavigationService.RemoveBackEntry();
                }
                while (pageBackStack != null);

                if (app.loginInfo == null)
                {
                    HandleGoogleBtn(true);
                    ShowOverlay.Stop();
                    OverlayPopup.IsOpen = false;
                    progressIndicator.IsIndeterminate = false;
                    progressIndicator.IsVisible = false;
                    ApplicationBar.IsVisible = true;
                }
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (e.Content is MainTodoistPage)
            {
                JournalEntry pageBackStack;
                do
                {
                    pageBackStack = NavigationService.RemoveBackEntry();
                }
                while (pageBackStack != null);
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Handle the style of Email and Password boxes.
            HandleStyleHintBox(EmailTxtBox, PasswordBox);
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

        private void GoogleSignInBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ListBoxItem googleSignInBtn = (ListBoxItem)sender;

            if (googleSignInBtn.IsEnabled)
            {
                Focus();

                ApplicationBar.IsVisible = false;
                OverlayPopup.IsOpen = true;
                ShowOverlay.Begin();

                progressIndicator.IsVisible = true;
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = "Connecting to Todoist service";

                SystemTray.SetProgressIndicator(this, progressIndicator);

                ToggleGoogleBtn();

                app.service.GoogleAuth(() =>
                {
                    NavigationService.Navigate(Utils.MainTodoistPage(removeBackStack: true));
                });
            }
        }

        private void HandleGoogleBtn(bool enabled)
        {
            GoogleSignInBtn.IsEnabled = !enabled;
            ToggleGoogleBtn();
        }

        private void ToggleGoogleBtn()
        {
            if (GoogleSignInBtn.IsEnabled)
            {
                GoogleSignInBtn.BorderBrush = (SolidColorBrush)Resources["PhoneDisabledBrush"];
                GoogleSignInBtn.Foreground = (SolidColorBrush)Resources["PhoneDisabledBrush"];
                GoogleSignInBtnLeftImage.Source =
                    new BitmapImage(new Uri("/MetroistLib;component/Images/Google_Disabled.png", UriKind.Relative));
            }
            else
            {
                GoogleSignInBtn.Foreground = GoogleSignInBtnForeground;
                GoogleSignInBtn.BorderBrush = GoogleSignInBtnBorderBrush;
                GoogleSignInBtnLeftImage.Source = GoogleSignInBtnImgSource;
            }

            GoogleSignInBtn.IsEnabled = !GoogleSignInBtn.IsEnabled;
        }

        private void GoogleSignInBtn_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            ListBoxItem googleSignInBtn = (ListBoxItem)sender;
            if (googleSignInBtn.IsEnabled)
            {
                GoogleSignInBtnImgSource = GoogleSignInBtnLeftImage.Source;
                GoogleSignInBtnBackground = googleSignInBtn.Background;
                GoogleSignInBtnForeground = googleSignInBtn.Foreground;
                googleSignInBtn.Background = GeneralLib.Utils.ConvertStringToColor("#000000");
                googleSignInBtn.Foreground = GeneralLib.Utils.ConvertStringToColor("#FFFFFF");
                GoogleSignInBtnLeftImage.Source =
                    new BitmapImage(new Uri("/MetroistLib;component/Images/Google_Pressed.png", UriKind.Relative));
            }
        }

        private void GoogleSignInBtn_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            ListBoxItem googleSignInBtn = (ListBoxItem)sender;
            if (googleSignInBtn.IsEnabled)
            {
                googleSignInBtn.Background = GoogleSignInBtnBackground;
                googleSignInBtn.Foreground = GoogleSignInBtnForeground;
                GoogleSignInBtnLeftImage.Source = GoogleSignInBtnImgSource;
            }
        }

        private void SignUpLoginBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(Utils.SignUpPage());
        }
    }
}