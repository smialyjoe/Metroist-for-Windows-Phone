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

namespace MetroistLib.Component
{
    public partial class Header : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
                DependencyProperty.RegisterAttached("Title", typeof(String), typeof(Header), new PropertyMetadata(""));

        public string Title
        {
            get { return GetValue(TitleProperty).ToString(); }
            set { SetValue(TitleProperty, value); }
        }

        public Header()
        {
            InitializeComponent();
        }
    }
}
