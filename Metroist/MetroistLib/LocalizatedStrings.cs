using System.Globalization;
using System.Threading;
using System.Resources;
using System.Reflection;
using System;

namespace TodoistLib
{
    public class LocalizedStrings
    {
        private ResourceManager resourceManager;

        public string login_google_login { get { return resourceManager.GetString("login_google_login"); } }
        public string login_google_login_loading { get { return resourceManager.GetString("login_google_login_loading"); } }
        public string login_email_label { get { return resourceManager.GetString("login_email_label"); } }
        public string login_password_label { get { return resourceManager.GetString("login_password_label"); } }
        public string login_action_button { get { return resourceManager.GetString("login_action_button"); } }
        public string login_about_label { get { return resourceManager.GetString("login_about_label"); } }
        public string login_newuser_label { get { return resourceManager.GetString("login_newuser_label"); } }
        public string signup_name { get { return resourceManager.GetString("signup_name"); } }
        public string signup_email { get { return resourceManager.GetString("signup_email"); } }
        public string signup_password { get { return resourceManager.GetString("signup_password"); } }
        public string signup_password_confirm { get { return resourceManager.GetString("signup_password_confirm"); } }
        public string signup_password_notequal { get { return resourceManager.GetString("signup_password_notequal"); } }
        public string signup_timezone { get { return resourceManager.GetString("signup_timezone"); } }
        public string signup_title { get { return resourceManager.GetString("signup_title"); } }
        public string loading_data { get { return resourceManager.GetString("loading_data"); } }
        public string general_confirm { get { return resourceManager.GetString("general_confirm"); } }
        public string timezone_title { get { return resourceManager.GetString("timezone_title"); } }
        public string signup_select_timezone { get { return resourceManager.GetString("signup_select_timezone"); } }
        public string login_logging { get { return resourceManager.GetString("login_logging"); } }
        public string signup_password_min { get { return resourceManager.GetString("signup_password_min"); } }
        public string signup_signing { get { return resourceManager.GetString("signup_signing"); } }
        public string signup_success { get { return resourceManager.GetString("signup_success"); } }
        public string error_ALREADY_REGISTRED { get { return resourceManager.GetString("error_ALREADY_REGISTRED"); } }
        public string error_INVALID_EMAIL { get { return resourceManager.GetString("error_INVALID_EMAIL"); } }
        public string error_TOO_SHORT_PASSWORD { get { return resourceManager.GetString("error_TOO_SHORT_PASSWORD"); } }
        public string error_UNKNOWN_ERROR { get { return resourceManager.GetString("error_UNKNOWN_ERROR"); } }
        public string general_connection_canceled { get { return resourceManager.GetString("general_connection_canceled"); } }
        public string general_no_connection { get { return resourceManager.GetString("general_no_connection"); } }
        public string login_google_button { get { return resourceManager.GetString("login_google_button"); } }
        public string general_search { get { return resourceManager.GetString("general_search"); } }

        public LocalizedStrings()
        {
            if (new Random().Next() % 2 == 0)
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            CultureInfo CurrentCulture = CultureInfo.CurrentUICulture;
            string ResourceName = "TodoistLib.Language.AppResources_";

            if (CurrentCulture.Name == "pt-BR")
                ResourceName += CurrentCulture.Name;
            else
                ResourceName += "en-US";

            resourceManager = new ResourceManager(ResourceName, Assembly.GetExecutingAssembly());
        }
    }
}
