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

namespace MetroistLib.Model
{
    [DataContract]
    public class Login
    {
        [DataMember]
        public string gmail_domain { get; set; }

        [DataMember]
        public string notifo { get; set; }

        [DataMember]
        public string start_page { get; set; }

        [DataMember]
        public string last_used_ip { get; set; }

        [DataMember]
        public string twitter { get; set; }

        [DataMember]
        public bool is_premium { get; set; }

        [DataMember]
        public string sort_order { get; set; }

        [DataMember]
        public string full_name { get; set; }

        [DataMember]
        public string api_token { get; set; }

        [DataMember]
        public string timezone { get; set; }

        [DataMember]
        public string jabber { get; set; }

        [DataMember]
        public string id { get; set; }

        //[DataMember]
        //public string tz_offset { get; set; }

        [DataMember]
        public string msn { get; set; }

        [DataMember]
        public string email { get; set; }

        [DataMember]
        public string start_day { get; set; }

        [DataMember]
        public string time_format { get; set; }

        [DataMember]
        public string beta { get; set; }

        [DataMember]
        public string karma_trend { get; set; }

        [DataMember]
        public string mobile_number { get; set; }

        [DataMember]
        public string mobile_host { get; set; }

        [DataMember]
        public string date_format { get; set; }

        [DataMember]
        public string premium_until { get; set; }

        [DataMember]
        public string join_date { get; set; }

        [DataMember]
        public string karma { get; set; }

        [DataMember]
        public string default_reminder { get; set; }
    }

    [DataContract]
    public class GAccessToken
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string expires_in { get; set; }
        [DataMember]
        public string token_type { get; set; }
    }

    [DataContract]
    public class GUserInfo 
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string picture { get; set; }
    }

    [DataContract]
    public class LocalLogin
    {
        [DataMember]
        public bool isRecorded { get; set; }

        [DataMember]
        public string email { get; set; }

        [DataMember]
        public string password { get; set; }
    }
}
