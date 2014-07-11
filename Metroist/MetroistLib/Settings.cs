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
using MetroistLib.Model;

namespace MetroistLib
{
    [DataContract]
    public class Settings
    {
        [DataMember]
        public FilterOption DateStringHome { get; set; }
    }
}
