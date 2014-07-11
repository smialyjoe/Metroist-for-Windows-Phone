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
using Microsoft.Phone.Controls.Primitives;
using System.Globalization;
using System.ComponentModel;
using System.Collections;

namespace Metroist
{
    public partial class DateTimeChooserPage : PhoneApplicationPage
    {
        public static string DateTimeFromChooser = string.Empty;
        public static Calendar calendar = null;

        public ApplicationBarIconButton CalendarButton;
        public ApplicationBarIconButton TimeButton;

        public static Uri uriCalendarPressed = new Uri("Images/CalendarPressed.png", UriKind.Relative);
        public static Uri uriCalendarReleased = new Uri("Images/CalendarReleased.png", UriKind.Relative);
        public static Uri uriTimePressed = new Uri("Images/TimePressed.png", UriKind.Relative);
        public static Uri uriTimeReleased = new Uri("Images/TimeReleased.png", UriKind.Relative);

        public ListBox beforeSelected = null;
        public Grid beforeDaySelected = null;
        public TextBlock beforeDayTextBlockSelected = null;

        Grid dayCalendarSelected = null;

        string[] weekdaysSource = null;

        public DateTimeChooserPage()
        {
            ThemeManager.ToLightTheme();

            InitializeComponent();

            DateTimeChooserPage.calendar = new Calendar();

            DateTimeChooserPage.calendar.PropertyChanged += new PropertyChangedEventHandler(calendar_PropertyChanged);

            DataContext = DateTimeChooserPage.calendar; 

            var DateTimeFormatInfo = new System.Globalization.DateTimeFormatInfo();

            weekdaysSource = (from str in DateTimeFormatInfo.AbbreviatedDayNames select str[0].ToString()).ToArray();

            CreateApplicationBar();

            CalendarListBox.ItemsSource = CreateCalendarSource(DateTimeChooserPage.calendar.Date) ;
            WeekdaysListBox.ItemsSource = weekdaysSource;
        }

        void calendar_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Date")
            {
                Dispatcher.BeginInvoke(() =>
                {
                    //CalendarListBox.ItemsSource = CreateCalendarSource(calendar.Date);

                    //FindCalendarDayElement(CalendarListBox, calendar.Date);
                });
            }
        }

        public void FindCalendarDayElement(ListBox calendar, DateTime date)
        {
            DependencyObject calendarObj = calendar as DependencyObject;

            if (calendar != null)
            {
                CallSearchCalendarDay(calendar, date);

                if(dayCalendarSelected != null)
                    CalendarDay_Tap(dayCalendarSelected,null);
            }
        }

        private void CallSearchCalendarDay(DependencyObject target, DateTime date)
        {
            dayCalendarSelected = null;

            dayCalendarSelected = SearchEspecifyCalendarDay(target, date);
        }

        private Grid SearchEspecifyCalendarDay(DependencyObject targeted_control, DateTime date)
        {
            var count = VisualTreeHelper.GetChildrenCount(targeted_control);   // targeted_control is the listbox
            DependencyObject grid = null;

            for (int i = 0; i < count; i++)
            {
                grid = VisualTreeHelper.GetChild(targeted_control, i);

                if (grid is Grid && IsCorrectCalendarDay(grid, date))
                {
                    dayCalendarSelected = grid as Grid;
                    return grid as Grid;
                }
                else
                {
                    SearchEspecifyCalendarDay(grid, date);
                }
            }
            return dayCalendarSelected;
        }

        public bool IsCorrectCalendarDay(DependencyObject grid, DateTime date)
        {
            TextBlock tBlock = GeneralLib.Utils.SearchChildElement<TextBlock>(grid);

            if (tBlock != null)
            {
                if ((tBlock.DataContext as CalendarDayVisual).Date.Date == date.Date)
                {
                    return true;
                }
            }
            return false;
        }

        private IEnumerable CreateCalendarSource(DateTime dateTime)
        {
            CalendarListBox.UpdateLayout();

            CalendarDayVisual[][] source = new CalendarDayVisual[6][];
            for(int i=0;i<source.Length;i++)
                source[i] = new CalendarDayVisual[7];

            DateTime temp;

            int dayCount;
            bool first = true;
            bool monthDone = false;

            int firstDayOfWeek = new DateTime(dateTime.Year, dateTime.Month, 1).DayOfWeek.GetHashCode();

            int lastDay = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);

            //Start with number of days of the before month
            dayCount = DateTime.DaysInMonth(dateTime.AddMonths(-1).Year, dateTime.AddMonths(-1).Month);

            temp = new DateTime(dateTime.AddMonths(-1).Year, dateTime.AddMonths(-1).Month, dayCount - firstDayOfWeek);
            for (int i = firstDayOfWeek - 1; i >= 0; i--)
            {
                temp = temp.AddDays(1);

                source[0][i] = new CalendarDayVisual() 
                { 
                    Day = dayCount--,
                    Color = GeneralLib.Utils.ConvertStringToColor("#777777"), 
                    IsOutOfCurrentMonth = true,
                    Date = temp
                };
            }

            //Start with the first day of current month
            dayCount = 1;

            for (int i = 0; i < source.Length; i++)
            {
                for (int j = firstDayOfWeek; j < source[i].Length; j++)
                {
                    temp = temp.AddDays(1);

                    source[i][j] = new CalendarDayVisual() 
                    { 
                        Day = dayCount++, 
                        Color = monthDone ?
                        GeneralLib.Utils.ConvertStringToColor("#777777")
                        : GeneralLib.Utils.ConvertStringToColor("#000000"),

                        IsOutOfCurrentMonth = monthDone,

                        Date = temp
                    };

                    if (first)
                    {    
                        firstDayOfWeek = 0;
                        first = false;
                    }

                    if (dayCount > lastDay)
                    {
                        dayCount = 1;
                        monthDone = true;
                    }
                }
            }

            return source.Select(x => x.ToList()).ToList();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            ThemeManager.ToLightTheme();
        }

        private void CreateApplicationBar()
        {
            ApplicationBar = new ApplicationBar()
            {

                ForegroundColor = GeneralLib.Utils.ConvertStringToColor("#000000").Color,
                IsVisible = false
            };

            CalendarButton = new ApplicationBarIconButton { IconUri = uriCalendarPressed, Text = "Calendar" };
            TimeButton = new ApplicationBarIconButton { IconUri = uriTimeReleased, Text = "Time" };

            ApplicationBar.Buttons.Add(CalendarButton);
            ApplicationBar.Buttons.Add(TimeButton);

            CalendarButton.Click += new EventHandler(CalendarButton_Click);
            TimeButton.Click += new EventHandler(TimeButton_Click);

            //TimeButton.IconUri = uriTimePressed;

            ApplicationBar.IsVisible = true;


        }

        void CalendarButton_Click(object sender, EventArgs e)
        {
            TimeButton.IconUri = uriTimeReleased;
            CalendarButton.IconUri = uriCalendarPressed;
        }

        void TimeButton_Click(object sender, EventArgs e)
        {
            TimeButton.IconUri = uriTimePressed;
            CalendarButton.IconUri = uriCalendarReleased;
        }

        public void HandleListBoxColorCalendarWeek(ListBox listbox)
        {
            listbox.Background = GeneralLib.Utils.ConvertStringToColor("#d9d9d9");
            if (beforeSelected != null && beforeSelected != listbox)
                beforeSelected.Background = null;
            beforeSelected = listbox;
        }

        private void CalendarDay_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid grid = sender as Grid;

            if (grid != null)
            {
                //Trying to find parent listbox to put a gray color
                var _listbox = VisualTreeHelper.GetParent(grid) as DependencyObject;
                while (!(_listbox is ListBox))
                    _listbox = VisualTreeHelper.GetParent(_listbox) as DependencyObject;

                if (_listbox is ListBox)
                {
                    var countChild = VisualTreeHelper.GetChildrenCount(grid);
                    if (countChild > 0)
                    {
                        var textblock = GeneralLib.Utils.SearchChildElement<TextBlock>(grid as DependencyObject);
                        if (textblock != null)
                        {
                            var dataContext = textblock.DataContext as CalendarDayVisual;

                            if (!dataContext.IsOutOfCurrentMonth)
                            {
                                HandleListBoxColorCalendarWeek(_listbox as ListBox);

                                HandleGridColorCalendarDay(grid);

                                HandleTextBlockForegroundCalendarDay(textblock as TextBlock);

                                ExtractDateFromContext(dataContext);
                            }
                            else
                            {
                                //One month before
                                if (dataContext.Date.Month == calendar.Date.Month - 1)
                                {
                                    //CalendarListBox.ItemsSource = CreateCalendarSource(calendar.Date.AddMonths(-1));
                                }
                                //One month after
                                else
                                {
                                    //CalendarListBox.ItemsSource = CreateCalendarSource(calendar.Date.AddMonths(1));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ExtractDateFromContext(CalendarDayVisual dataContext)
        {
            calendar.Date = dataContext.Date;
        }

        private void HandleGridColorCalendarDay(Grid grid)
        {
            grid.Background = App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush;

            if (beforeDaySelected != null && beforeDaySelected != grid)
                beforeDaySelected.Background = null;

            beforeDaySelected = grid;
        }

        private void HandleTextBlockForegroundCalendarDay(TextBlock textblock)
        {
            if (beforeDayTextBlockSelected != null)
                beforeDayTextBlockSelected.Foreground = GeneralLib.Utils.ConvertStringToColor("#000000");

            beforeDayTextBlockSelected = textblock;
            textblock.Foreground = GeneralLib.Utils.ConvertStringToColor("#ffffff");
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ArrowPressed(object sender, ManipulationStartedEventArgs e)
        {
            e.Handled = true;

            StackPanel stackpanel = sender as StackPanel;

            if (stackpanel != null)
            {
                stackpanel.Background = GeneralLib.Utils.ConvertStringToColor("#d9d9d9");
            }
        }

        private void ArrowReleased(object sender, ManipulationCompletedEventArgs e)
        {
            StackPanel stackpanel = sender as StackPanel;

            if (stackpanel != null)
            {
                stackpanel.Background = GeneralLib.Utils.ConvertStringToColor("#ffffff"); ;
            }
        }

        private void MonthUpStackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            calendar.Date = calendar.ChangeMonth(-1);
            Dispatcher.BeginInvoke(() =>
            {
                CalendarListBox.ItemsSource = CreateCalendarSource(calendar.Date);
            });
        }

        private void MonthDownStackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            calendar.Date = calendar.ChangeMonth(1);
            CalendarListBox.ItemsSource = CreateCalendarSource(calendar.Date);
        }

        private void DayUpStackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            calendar.Date = calendar.ChangeDay(-1);
            CalendarListBox.ItemsSource = CreateCalendarSource(calendar.Date);
        }

        private void DayDownStackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            calendar.Date = calendar.ChangeDay(1);
            CalendarListBox.ItemsSource = CreateCalendarSource(calendar.Date);
        }

        private void YearUpStackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            calendar.Date = calendar.ChangeYear(-1);
            CalendarListBox.ItemsSource = CreateCalendarSource(calendar.Date);
        }

        private void YearDownStackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            calendar.Date = calendar.ChangeYear(1);
            CalendarListBox.ItemsSource = CreateCalendarSource(calendar.Date);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            FindCalendarDayElement(CalendarListBox, DateTimeChooserPage.calendar.Date);
        }

        private void CalendarListBox_Loaded(object sender, RoutedEventArgs e)
        {
            FindCalendarDayElement(CalendarListBox, DateTimeChooserPage.calendar.Date);
        }

    }

    public class CalendarDayVisual
    {
        public int Day { get; set; }
        public bool IsOutOfCurrentMonth { get; set; }
        public Brush Color { get; set; }

        public DateTime Date { get; set; }
    }

    public class Calendar : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private DateTime _date;
        public DateTime Date 
        { 
            get { return _date; } 
            set { SetField(ref _date, value, "Date"); } 
        }

        private string _MonthName;
        public string MonthName 
        { 
            get { return _MonthName; } 
            set { SetField(ref _MonthName, value, "MonthName"); } 
        }

        public static string[] MonthShortNames =
            new DateTimeFormatInfo().AbbreviatedMonthNames.Take(12).ToArray();

        public Calendar(DateTime date)
        {
            this.Date = date;
        }

        public Calendar()
        {
            this.Date = DateTime.Now;
        }

        private DateTimeFormatInfo DateTimeFormatInfo = new System.Globalization.DateTimeFormatInfo();

        //If amount > 0, it is increasing. If amount < 0, it is decreasing.
        public DateTime ChangeDay(int amount = 1)
        {
            return Date.AddDays(amount);
        }

        public DateTime ChangeMonth(int amount = 1)
        {
            MonthName = Calendar.MonthShortNames[Date.Month - 1];

            return Date.AddMonths(amount);
        }

        public DateTime ChangeYear(int amount = 1)
        {
            return Date.AddYears(amount);
        }


        
    }
}
