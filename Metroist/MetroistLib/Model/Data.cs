using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using GeneralLib;

namespace MetroistLib.Model
{
    [DataContract]
    public class Data
    {
        [DataMember]
        public int[] ActiveProjectIds { get; set; }
        [DataMember]
        public bool FetchedAllData { get; set; }
        //[DataMember]
        //public Settings Settings { get; set; }
        [DataMember]
        public string LabelsTimestamp { get; set; }
        [DataMember]
        public string DayOrdersTimestamp { get; set; }
        [DataMember]
        public string FiltersTimestamp { get; set; }
        [DataMember]
        public int UserId { get; set; }
        //[DataMember]
        //public ActiveProjectTimestamps ActiveProjectTimestamps { get; set; }
        [DataMember]
        public string RemindersTimestamp { get; set; }
        [DataMember]
        public int LiveNotificationsUnread { get; set; }
        [DataMember]
        public int LiveNotificationsCount { get; set; }
        //[DataMember]
        //public User User { get; set; }
        [DataMember]
        public Dictionary<string, int> TempIdMapping { get; set; }
        [DataMember]
        public string CollaboratorsTimestamp { get; set; }
        [DataMember]
        public List<Project> Projects { get; set; }
    }

    [DataContract]
    //[KnownType("Project")]
    public class Project
    {
        [DataMember]
        public int user_id { get; set; }
        [DataMember]
        public double last_updated { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string last_updated_items { get; set; }
        [DataMember]
        public int color { get; set; }
        [DataMember]
        public string last_updated_meta { get; set; }
        [DataMember]
        public int collapsed { get; set; }
        [DataMember]
        public string last_updated_notes { get; set; }
        [DataMember]
        public bool inbox_project { get; set; }
        [DataMember]
        public object archived_date { get; set; }
        [DataMember]
        public int item_order { get; set; }
        [DataMember]
        public int indent { get; set; }
        [DataMember]
        public int is_archived { get; set; }
        [DataMember]
        public int cache_count { get; set; }
        [DataMember]
        public bool shared { get; set; }
        [DataMember]
        public int archived_timestamp { get; set; }
        [DataMember]
        public List<QueryDataItem> items { get; set; }
        [DataMember]
        public int id { get; set; }
    }

    [DataContract]
    public class QueryItem
    {
        [DataMember]
        public string query { get; set; }
        [DataMember]
        public string type { get; set; }
        //[DataMember(Name="data")]
        //public Item[] items { get; set; }
       
        [DataMember(Name="data")]
        public QueryDataItem[] item { get; set; }
    }

    [DataContract]
    public class Label
    {
        [DataMember]
        public int color { get; set; }
        [DataMember]
        public int uid { get; set; }
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string name { get; set; }
    }

    [DataContract]
    public class QueryDataItem : INotifyPropertyChanged
    {
        [IgnoreDataMember]
        private bool _selectedListBoxItem_ProjectDetail;
        [IgnoreDataMember]
        public bool selectedListBoxItem_ProjectDetail
        {
            get { return _selectedListBoxItem_ProjectDetail; }
            set
            {
                _selectedListBoxItem_ProjectDetail = value;
                OnPropertyChanged("selectedListBoxItem_ProjectDetail");
            }
        }

        [DataMember]
        public int project_id { get; set; }

        [DataMember]
        public string due_date { get; set; }
        [DataMember]
        public string user_id { get; set; }
        [DataMember]
        public string collapsed { get; set; }
        [DataMember]
        public string in_history { get; set; }
        [DataMember]
        public string priority { get; set; }
        [DataMember]
        public string item_order { get; set; }
        [DataMember]
        public string content { get; set; }
        [DataMember]
        public string indent { get; set; }
        [DataMember]
        public int id { get; set; }
        [DataMember(Name = "checked")]
        public bool is_checked { get; set; }
        [DataMember]
        public string date_string { get; set; }

        //[DataMember]
        //public int[] labels { get; set; }

        [DataMember]
        public string project_name { get; set; }
        [DataMember]
        public string completed_count { get; set; }
        [DataMember]
        public QueryDataItem[] uncompleted { get; set; }
        [DataMember]
        public string history_max_order { get; set; }
        [DataMember]
        public string cache_count { get; set; }

        [DataMember]
        public List<int> labels { get; set; }

        [DataMember]
        public List<Note> notes { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [DataContract]
    public class Note
    {
        [DataMember]
        public int is_deleted { get; set; }
        [DataMember]
        public int is_archived { get; set; }
        [DataMember]
        public string content { get; set; }
        [DataMember]
        public int posted_uid { get; set; }
        [DataMember]
        public int item_id { get; set; }
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string posted { get; set; }
    }

    public enum FilterTask
    {
        Today, Tomorrow, Next7Days, AllUncompleted
    }

    public class FilterOption
    {
        public static FilterOption TodayFilterOption = 
            new FilterOption { Key = FilterTask.Today, FriendlyName = "today", Value = "Today" };

        public static ObservableCollection<FilterOption> filteringOptions = 
        new ObservableCollection<FilterOption>
        {
            TodayFilterOption,
            new FilterOption { Key = FilterTask.Tomorrow, FriendlyName="tomorrow", Value = "Tomorrow" },
            new FilterOption { Key = FilterTask.Next7Days, FriendlyName = "next 7 days", Value = "Next 7 days" },
            new FilterOption { Key = FilterTask.AllUncompleted, FriendlyName = "all uncompleted", Value = "View all uncompleted" },
        };

        public FilterTask Key { get; set; }
        public string Value { get; set; }
        public string FriendlyName { get; set; }
        public bool Selected { get; set; }
    }

    [DataContract]
    public class TemporaryItem
    {
        [DataMember]
        public string type { get; set; }

        [DataMember]
        public string temp_id { get; set; }

        [DataMember]
        public double timestamp { get; set; }

        [DataMember]
        public QueryDataItem item { get; set; }

        [DataMember]
        public Project project { get; set; }
    }

}
