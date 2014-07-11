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
using System.Collections.Generic;

namespace MetroistLib.Model
{
    public class Timezone
    {
        public Timezone()
        {
        }

        public List<TimezoneItem> Item { get; set; }
    }

    public class TimezoneItem
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Value { get; set; }

    }
}
