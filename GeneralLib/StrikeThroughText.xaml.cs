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
using System.ComponentModel;

namespace GeneralLib
{
    public partial class StrikeThroughText : UserControl
    {

        public StrikeThroughText()
        {
            InitializeComponent();
            MainTextBlock.SizeChanged += OnSizeChanged;
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool StrikeThrough
        {
            get { return (bool)GetValue(StrikeThroughProperty); }
            set 
            {
                SetValue(StrikeThroughProperty, value);
            }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(StrikeThroughText),
            new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnPropertyChanged)));

        public static readonly DependencyProperty StrikeThroughProperty =
            DependencyProperty.Register("StrikeThrough", typeof(bool), typeof(StrikeThroughText),
            new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StrikeThroughText thisObj = d as StrikeThroughText;

            if (e.Property == TextProperty)
                thisObj.MainTextBlock.Text = e.NewValue.ToString();
            else if (e.Property == StrikeThroughProperty)
                thisObj.StrikeThroughLine.Visibility =
                    thisObj.StrikeThrough ? Visibility.Visible : Visibility.Collapsed;

        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            StrikeThroughLine.Stroke = MainTextBlock.Foreground;
            StrikeThroughLine.X1 = 0;
            StrikeThroughLine.Y1 = MainTextBlock.ActualHeight * 0.6;
            StrikeThroughLine.X2 = MainTextBlock.ActualWidth - 1;
            StrikeThroughLine.Y2 = StrikeThroughLine.Y1;
        }



        
    }
}
