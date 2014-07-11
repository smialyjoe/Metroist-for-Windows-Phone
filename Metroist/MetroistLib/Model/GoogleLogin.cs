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
    public class GoogleLogin
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string id_token { get; set; }
        [DataMember]
        public string expires_in { get; set; }
        [DataMember]
        public string token_type { get; set; }
        [DataMember]
        public string refresh_token { get; set; }
    }

    [DataContract]
    public class GoogleUser
    {
        [DataMember]
        public string email { get; set; }
    }
}
