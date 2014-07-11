using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using GeneralLib;
using MetroistLib.Model;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using Microsoft.Phone.Controls;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Metroist
{
    public class TodoistService : Service
    {
        public readonly string urlbaseStandardAPI = "https://todoist.com/API/";
        public readonly string urlbaseSyncV2 = "https://todoist.com/TodoistSync/v2/";
        public readonly string urlGoogleOAuth = "https://accounts.google.com/o/oauth2/";
        public readonly string urlGoogleAPI = "https://www.googleapis.com/oauth2/";
        public readonly string urlGoogleAPIEmail = "https://www.googleapis.com/oauth2/v1/userinfo";

        public readonly string googleClientSecret = "xw4lUecqoYaj_pgb0YNWWy2d";
        public string googleCode { get; set; }

        public readonly string googleClientID = "538834818097-2bfcbvv8r6r9t5jhonqot7i4hvibs5rs.apps.googleusercontent.com";

        public bool debugWithInternet = true;

        void request<T>(
            string url, Dictionary<string, object> args, Action<T> success,
            Action<string> error = null, String requestType = "GET")
        {
            if (!debugWithInternet)
            {
                error("Debug: Without Internet");
            }
            else
            {
                base.request(new Uri(url), args,
                (data) =>
                {
                    try
                    {
                        String errorMessage = "";
                        if (data is Exception)
                        {
                            //Exceção na requisição Web
                            Exception e = (Exception)data;

                            if (e.Message.Contains("NotFound"))
                                errorMessage = "We are having problem to use your internet connection. Please, could you check it?";
                            else
                                errorMessage = e.Message;

                            error(errorMessage);
                        }
                        else
                        {
                            errorMessage = Utils.ErrorMessage((string)data);
                            if (errorMessage != "")
                            {
                                error(errorMessage);
                                return;
                            }

                            //A resposta esperada é string, retornar a própria resposta da requisição
                            if (typeof(T) == typeof(string))
                            {
                                success((T)(object)(data));
                            }
                            //Em caso de ser necessário, o parsing para object usando json
                            else if (data is string)
                            {
                                string dataString = Regex.Replace((string)data, "[\n\r\t]|=\\\\", delegate(Match match)
                                {
                                    return match.Value == @"=\" ? "=" : " ";
                                });

                                T obj = JsonConvert.DeserializeObject<T>(dataString);
                                success(obj);
                            }
                        }
                    }
                    catch (JsonException global)
                    {
                        //Caso não seja json a resposta
                        Debug.WriteLine("Error: JSON format is incorrect");
                        Debug.WriteLine(global.Message);
                        error((string)data);
                    }

                }, requestType);
            }
        }

        public void post<T>(
            string url, Dictionary<string, object> args, Action<T> success,
            Action<string> error = null)
        {
            request<T>(url, args, success, error, "POST");
        }

        public void get<T>(
            string url, Dictionary<string, object> args, Action<T> success,
            Action<string> error = null)
        {
            request<T>(url, args, success, error, "GET");
        }

        public void Login(string email, string password, Action<Login> onSuccess, Action<string> onError, Action onFinally)
        {
            App app = Application.Current as App;
            Dictionary<string, object> Arguments = new Dictionary<string, object>
            {
                {"email",email},
                {"password",password}
            };

            get<Login>(urlbaseStandardAPI + "login", Arguments,
            (response) =>
            {
                app.loginInfo = response;
                onSuccess(response);
                if (onFinally != null)
                    onFinally();
            },
            (stringError) =>
            {
                onError(stringError);
                if (onFinally != null)
                    onFinally();
            });
        }

        public void GetData(Action<Data> onSuccess, Action<string> onError, Action onFinally = null)
        {
            App app = Application.Current as App;
            Dictionary<string, object> Arguments = new Dictionary<string, object>
            {
                {"api_token", app.loginInfo.api_token}
            };

            get<Data>(urlbaseSyncV2 + "get", Arguments,
            (response) =>
            {
                onSuccess(response);
                if (onFinally != null)
                    onFinally();
            },
            (stringError) =>
            {
                onError(stringError);
                if (onFinally != null)
                    onFinally();
            });
        }

        public void SyncAndGetUpdated(
            ProgressIndicator progress,
            Action<Data> onSuccess, Action<string> onError,
            Action onFinally = null, Action onTimerOver = null)
        {
            progress.IsVisible = true;

            App app = Application.Current as App;
            Dictionary<string, object> Arguments = new Dictionary<string, object>
            {
                {"api_token", app.loginInfo.api_token},
                {"",""}
               /*{"project_timestamps", Utils.EncodeJsonItems(app.projects)},*/
            };

            post<Data>(urlbaseSyncV2 + "syncAndGetUpdated", Arguments,
            (response) =>
            {
                onSuccess(response);
                if (onFinally != null)
                {
                    onFinally();
                    progress.Text = "Update completed";
                    progress.IsIndeterminate = false;

                    progress.Dispatcher.BeginInvoke(() =>
                    {
                        DispatcherTimer timer = null;
                        timer = new DispatcherTimer
                        {
                            Interval = new TimeSpan(0, 0, 2)
                        };

                        timer.Tick += (sender, e) =>
                        {
                            progress.IsIndeterminate = true;
                            progress.Text = string.Empty;
                            progress.IsVisible = false;

                            if (onTimerOver != null)
                                onTimerOver();

                            (sender as DispatcherTimer).Stop();
                        };

                        timer.Start();
                    });
                }
            },
            (stringError) =>
            {
                onError(stringError);
                if (onFinally != null)
                {
                    onFinally();

                    progress.Text = "Update failed";
                    progress.IsIndeterminate = false;
                    progress.Dispatcher.BeginInvoke(() =>
                    {
                        DispatcherTimer timer = null;
                        timer = new DispatcherTimer
                        {
                            Interval = new TimeSpan(0, 0, 2)
                        };

                        timer.Tick += (sender, e) =>
                        {
                            progress.IsIndeterminate = true;
                            progress.Text = string.Empty;
                            progress.IsVisible = false;
                            (sender as DispatcherTimer).Stop();
                        };

                        timer.Start();
                    });
                }
            });
        }

        //TODO: Warning. Too heavy!
        public void GetStartPage(FilterOption filterOption, Action<List<QueryDataItem>> onSuccess, Action<string> onError, Action onFinally)
        {
            App app = Application.Current as App;
            List<QueryDataItem> filteredList = new List<QueryDataItem>();

            if (filterOption.Key == FilterTask.Today)
            {
                foreach (var proj in app.projects)
                    foreach (var task in proj.items)
                        if (task.due_date != null && task.due_date != string.Empty)
                            if (DateTime.Parse(task.due_date).Date == DateTime.Now.Date)
                                filteredList.Add(task);

            }
            else if (filterOption.Key == FilterTask.Tomorrow)
            {
                foreach (var proj in app.projects)
                    foreach (var task in proj.items)
                        if (task.due_date != null && task.due_date != string.Empty)
                            if (DateTime.Parse(task.due_date).Date == DateTime.Now.AddDays(1).Date)
                                filteredList.Add(task);

            }
            else if (filterOption.Key == FilterTask.Next7Days)
            {
                foreach (var proj in app.projects)
                    foreach (var task in proj.items)
                        if (task.due_date != null && task.due_date != string.Empty)
                            if (GeneralLib.Utils.GetNext7Days(DateTime.Now).Contains(DateTime.Parse(task.due_date).Date))
                                filteredList.Add(task);
                filteredList = filteredList.OrderBy(x => DateTime.Parse(x.due_date)).ToList();
            }
            else
            {
                foreach (var proj in app.projects)
                    foreach (var task in proj.items)
                        filteredList.Add(task);
            }
            onSuccess(filteredList);
        }

        internal void SetTaskAsChecked(DateTime commandTimeGenerated, QueryDataItem task, Action<Data> onSuccess, Action<string> onError, Action onFinally, bool complete = true)
        {
            App app = Application.Current as App;

            #region argument
            //Main argument list
            Dictionary<string, object> Arguments = new Dictionary<string, object>
            {
                {"api_token", app.loginInfo.api_token},
            };

            //Items to sync argument list
            Dictionary<string, object> itemsToSyncArgs = new Dictionary<string, object>
            {
                {"type", "item_complete"},
                {"timestamp", Utils.DateTimeToUnixTimestamp(commandTimeGenerated)},
            };

            //Internal 'args' argument list
            Dictionary<string, object> internalArgs = new Dictionary<string, object>
            {
                {"project_id", task.project_id},
                {"ids", "[" + task.id + "]" },
                {"force_history", 1 }
            };
            itemsToSyncArgs.Add("args", internalArgs);
            Arguments.Add("items_to_sync", "[" + Utils.EncodeJsonItems(itemsToSyncArgs) + "]");
            #endregion

            post<Data>(urlbaseSyncV2 + "syncAndGetUpdated", Arguments,
            (response) =>
            {
                onSuccess(response);
                if (onFinally != null)
                    onFinally();
            },
            (stringError) =>
            {
                onError(stringError);
                if (onFinally != null)
                    onFinally();
            });

        }


        #region project
        //Project handling: Add, Edit, Remove

        internal void AddProject(DateTime commandTimeGenerated, Project proj, Action<Data> onSuccess, Action<string> onError, Action onFinally)
        {
            App app = Application.Current as App;

            //Main argument list
            Dictionary<string, object> Arguments = new Dictionary<string, object>
            {
                {"api_token", app.loginInfo.api_token},
                //{"project_timestamps", Utils.EncodeJsonProperties(app.projects)},
            };

            //Items to sync argument list
            Dictionary<string, object> itemsToSyncArgs = new Dictionary<string, object>
            {
                {"type", "project_add"},
                {"temp_id", Utils.DateTimeToUnixTimestamp(commandTimeGenerated)},
                {"timestamp", Utils.DateTimeToUnixTimestamp(commandTimeGenerated)},
            };

            //Internal 'args' argument list
            Dictionary<string, object> internalArgs = new Dictionary<string, object>
            {
                {"name", proj.name },
                {"ident", 1 }, //@TODO: Implement this visually 
                {"color", proj.color},
                //{"item_order", Task.date_string} //@TODO: Implement this visually [Feature 2]
            };

            //Adding item_to_sync and args to their respective argument dictonaries.
            itemsToSyncArgs.Add("args", internalArgs);
            Arguments.Add("items_to_sync", "[" + Utils.EncodeJsonItems(itemsToSyncArgs) + "]");

            post<Data>(urlbaseSyncV2 + "syncAndGetUpdated", Arguments,
            (response) =>
            {
                onSuccess(response);
                if (onFinally != null)
                    onFinally();
            },
            (stringError) =>
            {
                onError(stringError);
                //app.TemporaryDesynchronized.Add(itemsToSyncArgs);
                if (onFinally != null)
                    onFinally();
            });
        }

        internal void RemoveProject(DateTime commandTimeGenerated, Project proj, Action<Data> onSuccess, Action<string> onError, Action onFinally)
        {
            App app = Application.Current as App;

            //Main argument list
            Dictionary<string, object> Arguments = new Dictionary<string, object>
            {
                {"api_token", app.loginInfo.api_token},
                //{"project_timestamps", Utils.EncodeJsonProperties(app.projects)},
            };

            //Items to sync argument list
            Dictionary<string, object> itemsToSyncArgs = new Dictionary<string, object>
            {
                {"type", "project_delete"},
                {"timestamp", Utils.DateTimeToUnixTimestamp(commandTimeGenerated)},
            };

            //Internal 'args' argument list
            Dictionary<string, object> internalArgs = new Dictionary<string, object>
            {
                {"ids", "[" + proj.id + "]" }
            };

            //Adding item_to_sync and args to their respective argument dictonaries.
            itemsToSyncArgs.Add("args", internalArgs);
            Arguments.Add("items_to_sync", "[" + Utils.EncodeJsonItems(itemsToSyncArgs) + "]");

            post<Data>(urlbaseSyncV2 + "syncAndGetUpdated", Arguments,
            (response) =>
            {
                onSuccess(response);
                if (onFinally != null)
                    onFinally();
            },
            (stringError) =>
            {
                onError(stringError);

                //app.TemporaryDesynchronized.Add(itemsToSyncArgs);
                if (onFinally != null)
                    onFinally();
            });
        }

        internal void EditProject(DateTime commandTimeGenerated, Project proj, Action<Data> onSuccess, Action<string> onError, Action onFinally)
        {
            App app = Application.Current as App;

            //Main argument list
            Dictionary<string, object> Arguments = new Dictionary<string, object>
            {
                {"api_token", app.loginInfo.api_token},
                //{"project_timestamps", Utils.EncodeJsonProperties(app.projects)},
            };

            //Items to sync argument list
            Dictionary<string, object> itemsToSyncArgs = new Dictionary<string, object>
            {
                {"type", "project_add"},
                {"temp_id", Utils.DateTimeToUnixTimestamp(commandTimeGenerated)},
                {"timestamp", Utils.DateTimeToUnixTimestamp(commandTimeGenerated)},
            };

            //Internal 'args' argument list
            Dictionary<string, object> internalArgs = new Dictionary<string, object>
            {
                {"name", proj.name },
                {"ident", 1 }, //@TODO: Implement this visually 
                {"color", proj.color},
                //{"item_order", Task.date_string} //@TODO: Implement this visually [Feature 2]
            };

            //Adding item_to_sync and args to their respective argument dictonaries.
            itemsToSyncArgs.Add("args", internalArgs);
            Arguments.Add("items_to_sync", "[" + Utils.EncodeJsonItems(itemsToSyncArgs) + "]");

            post<Data>(urlbaseSyncV2 + "syncAndGetUpdated", Arguments,
            (response) =>
            {
                onSuccess(response);
                if (onFinally != null)
                    onFinally();
            },
            (stringError) =>
            {
                onError(stringError);
                //app.TemporaryDesynchronized.Add(itemsToSyncArgs);
                if (onFinally != null)
                    onFinally();
            });
        }

        #endregion project

        #region task

        internal void AddTaskToProject(DateTime commandTimeGenerated, QueryDataItem Task, Action<Data> onSuccess, Action<string> onError, Action onFinally)
        {
            App app = Application.Current as App;

            //Main argument list
            Dictionary<string, object> Arguments = new Dictionary<string, object>
            {
                {"api_token", app.loginInfo.api_token},
                //{"project_timestamps", Utils.EncodeJsonProperties(app.projects)},
            };

            //Items to sync argument list
            Dictionary<string, object> itemsToSyncArgs = new Dictionary<string, object>
            {
                {"type", "item_add"},
                {"temp_id", Utils.DateTimeToUnixTimestamp(commandTimeGenerated)},
                {"timestamp", Utils.DateTimeToUnixTimestamp(commandTimeGenerated)},
            };

            //Internal 'args' argument list
            Dictionary<string, object> internalArgs = new Dictionary<string, object>
            {
                {"project_id", Task.project_id },
                {"content", Task.content },
                {"priority", "1"},
                {"date_string", Task.date_string} 
            };

            if (Task.date_string.Trim() == string.Empty)
                internalArgs.Remove("date_string");

            //Adding item_to_sync and args to their respective argument dictonaries.
            itemsToSyncArgs.Add("args", internalArgs);
            Arguments.Add("items_to_sync", "[" + Utils.EncodeJsonItems(itemsToSyncArgs) + "]");

            //onError("Sync testing! Please, if this message is being viewed, contact us!");
            //return;

            post<Data>(urlbaseSyncV2 + "syncAndGetUpdated", Arguments,
            (response) =>
            {
                onSuccess(response);
                if (onFinally != null)
                    onFinally();
            },
            (stringError) =>
            {
                onError(stringError);
                //app.TemporaryDesynchronized.Add(itemsToSyncArgs);
                if (onFinally != null)
                    onFinally();
            });

        }

        internal void RemoveTask(DateTime commandTimeGenerated, QueryDataItem task, Action<Data> onSuccess, Action<string> onError, Action onFinally)
        {
            App app = Application.Current as App;

            #region argument
            //Main argument list
            Dictionary<string, object> Arguments = new Dictionary<string, object>
            {
                {"api_token", app.loginInfo.api_token},
            };

            //Items to sync argument list
            Dictionary<string, object> itemsToSyncArgs = new Dictionary<string, object>
            {
                {"type", "item_delete"},
                {"timestamp", Utils.DateTimeToUnixTimestamp(commandTimeGenerated)},
            };

            //Internal 'args' argument list
            Dictionary<string, object> internalArgs = new Dictionary<string, object>
            {
                {"ids", "[" + task.id + "]" }
            };
            itemsToSyncArgs.Add("args", internalArgs);
            Arguments.Add("items_to_sync", "[" + Utils.EncodeJsonItems(itemsToSyncArgs) + "]");
            #endregion


            post<Data>(urlbaseSyncV2 + "syncAndGetUpdated", Arguments,
            (response) =>
            {
                onSuccess(response);
                if (onFinally != null)
                    onFinally();
            },
            (stringError) =>
            {
                onError(stringError);
                //app.TemporaryDesynchronized.Add(itemsToSyncArgs);
                if (onFinally != null)
                    onFinally();
            });
        }

        #endregion

        #region note

        public void AddNoteToTask(DateTime commandTimeGenerated, int idOfTask, String content, Action<Data> onSuccess, Action<string> onError, Action onFinally)
        {
            App app = Application.Current as App;

            //Main argument list
            Dictionary<string, object> Arguments = new Dictionary<string, object>
            {
                {"api_token", app.loginInfo.api_token},
                //{"project_timestamps", Utils.EncodeJsonProperties(app.projects)},
            };

            //Items to sync argument list
            Dictionary<string, object> itemsToSyncArgs = new Dictionary<string, object>
            {
                {"type", "note_add"},
                {"temp_id", Utils.DateTimeToUnixTimestamp(commandTimeGenerated)},
                {"timestamp", Utils.DateTimeToUnixTimestamp(commandTimeGenerated)},
            };

            //Internal 'args' argument list
            Dictionary<string, object> internalArgs = new Dictionary<string, object>
            {
                {"item_id", idOfTask },
                {"content", content },
            };

            //Adding item_to_sync and args to their respective argument dictonaries.
            itemsToSyncArgs.Add("args", internalArgs);
            Arguments.Add("items_to_sync", "[" + Utils.EncodeJsonItems(itemsToSyncArgs) + "]");

            //onError("Sync testing! Please, if this message is being viewed, contact us!");
            //return;

            post<Data>(urlbaseSyncV2 + "syncAndGetUpdated", Arguments,
            (response) =>
            {
                onSuccess(response);
                if (onFinally != null)
                    onFinally();
            },
            (stringError) =>
            {
                onError(stringError);
                //app.TemporaryDesynchronized.Add(itemsToSyncArgs);
                if (onFinally != null)
                    onFinally();
            });
        }

        #endregion

        #region timezone

        public void GetTimezoneList(Action<List<List<string>>> onSuccess, Action<string> onError, Action onFinally = null)
        {
            get<List<List<string>>>(urlbaseStandardAPI + "getTimezones", null,
                (response) =>
                {
                    onSuccess(response);
                    if (onFinally != null)
                        onFinally();
                },

                (response) =>
                {
                    onError(response);
                    if (onFinally != null)
                        onFinally();
                });
        }

        #endregion

        internal void GetTaskFromProject(int projectID, Action<List<QueryDataItem>> onSuccess, Action<string> onError, Action onFinally)
        {
            App app = Application.Current as App;
            Project project = app.projects.Where(x => x.id == projectID).FirstOrDefault();

            if (project != null)
            {
                onSuccess(project.items);
                if (onFinally != null)
                    onFinally();
            }
            else
            {
                onError("Project not found");
                if (onFinally != null)
                    onFinally();
            }
        }

        //Sync all tasks, projects, labels, reminder, etc. that wasn't.
        internal void SyncAll(Action<Data> onSuccess, Action<string> onError, Action onFinally)
        {
            App app = Application.Current as App;

            //Main argument list
            Dictionary<string, object> Arguments = new Dictionary<string, object>
            {
                {"api_token", app.loginInfo.api_token},
            };

            if (app.TemporaryDesynchronized.Count > 0)
            {
                //Adding item_to_sync to its respective argument dictonaries.
                string MultipleTasks = "[";
                foreach (var temp in app.TemporaryDesynchronized)
                {
                    MultipleTasks += Utils.EncodeJsonItems(temp);

                    if (temp != app.TemporaryDesynchronized.Last())
                        MultipleTasks += ",";
                }
                MultipleTasks += "]";

                Arguments.Add("items_to_sync", MultipleTasks);

                post<Data>(urlbaseSyncV2 + "syncAndGetUpdated", Arguments,
                (response) =>
                {
                    //@TODO: check if the sync was sucessful

                    onSuccess(response);
                    if (onFinally != null)
                        onFinally();
                },
                (stringError) =>
                {
                    onError(stringError);
                    if (onFinally != null)
                        onFinally();
                });
            }
        }

        private object CreateTemporySyncIdProject(Project project)
        {
            return "[" + project.id + "]";
        }

        private object CreateTemporarySyncProject(Project project)
        {
            //Internal 'args' argument list
            Dictionary<string, object> internalArgs = new Dictionary<string, object>
            {
                {   "project_id",  project.id      },
                {   "name",        project.name    },
                {   "color",       project.color   }
            };

            return internalArgs;
        }

        public Dictionary<string, object> CreateTemporarySyncItem(QueryDataItem Task)
        {
            //Internal 'args' argument list
            Dictionary<string, object> internalArgs = new Dictionary<string, object>
            {
                {"project_id", Task.project_id },
                {"content", Task.content },
                {"priority", "1"},
                {"date_string", Task.date_string} 
            };

            if (Task.date_string.Trim() == string.Empty)
                internalArgs.Remove("date_string");

            return internalArgs;
        }

        public void GoogleAuth(Action onSuccess)
        {
            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"client_id", googleClientID},
                {"response_type", "code"},
                {"scope", "openid email"},
                {"redirect_uri", "http://localhost/"}
            };

            Uri targetUri = Utils.WebPage(urlGoogleOAuth + "auth?" + DictionaryToString(args), onSuccess);
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(targetUri);
        }

        public void GoogleAccessToken(string code, Action success)
        {
            Dictionary<string, object> args = new Dictionary<string, object>
            {
                {"code", code },
                {"client_id", googleClientID},
                {"client_secret", googleClientSecret },
                {"redirect_uri", "http://localhost/"},
                {"grant_type", "authorization_code"}
            };

            post<GoogleLogin>(urlGoogleOAuth + "token", args, (response) =>
            {
                Dictionary<string, object> internalArgs = new Dictionary<string, object>
                {
                    {"alt", "json" },
                    {"access_token", response.access_token},
                };

                get<GoogleUser>(urlGoogleAPIEmail, internalArgs, (googleUser) =>
                {
                    Dictionary<string, object> todoistArgs = new Dictionary<string, object>
                    {
                        {"email", googleUser.email },
                        {"oauth2_token", response.access_token},
                    };

                    get<Login>(urlbaseStandardAPI + "loginWithGoogle", todoistArgs, (dataTodoist) =>
                    {
                        (Application.Current as App).loginInfo = dataTodoist;
                        success();
                        (Application.Current.RootVisual as PhoneApplicationFrame).GoBack();
                    });
                });

            });


        }

        public void SignUp(string userName, string email, string password, Action<Login> onSuccess, Action<string> onError)
        {
            Dictionary<String, Object> args = new Dictionary<string, object>()
            {
                { "email", email },
                { "full_name", userName },
                { "password", password },
            };

            get<Login>(urlbaseStandardAPI + "register", args, onSuccess, onError);
        }
    }
}
