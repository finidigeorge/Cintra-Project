using Client.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Client.Controls.WpfScheduler
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class WeekScheduler : UserControl
    {
        private Scheduler _scheduler;

        internal event EventHandler<Event> OnEventClick;
        internal event EventHandler<Event> OnEventDoubleClick;
        internal event EventHandler<DateTime> OnScheduleDoubleClick;

        #region FirstDay
        private DateTime _firstDay;
        public DateTime FirstDay
        {
            get { return _firstDay; }
            set {
                while (value.DayOfWeek != DayOfWeek.Monday)
                    value = value.AddDays(-1);
                _firstDay = value.TruncateToDayStart();
                AdjustFirstDay(value);
            }
        }

        private void AdjustFirstDay(DateTime firstDay)
        {
            dayLabel1.Content = firstDay.ToString("dddd dd/MM");
            dayLabel2.Content = firstDay.AddDays(1).ToString("dddd dd/MM");
            dayLabel3.Content = firstDay.AddDays(2).ToString("dddd dd/MM");
            dayLabel4.Content = firstDay.AddDays(3).ToString("dddd dd/MM");
            dayLabel5.Content = firstDay.AddDays(4).ToString("dddd dd/MM");
            dayLabel6.Content = firstDay.AddDays(5).ToString("dddd dd/MM");
            dayLabel7.Content = firstDay.AddDays(6).ToString("dddd dd/MM");

            PaintAllEvents(null);
            PaintAllDayEvents();
        }
        #endregion

        public WeekScheduler()
        {
            InitializeComponent();

            column1.MouseDown += Canvas_MouseDown;
            column1.Background = Brushes.Transparent;
            column2.MouseDown += Canvas_MouseDown;
            column2.Background = Brushes.Transparent;
            column3.MouseDown += Canvas_MouseDown;
            column3.Background = Brushes.Transparent;
            column4.MouseDown += Canvas_MouseDown;
            column4.Background = Brushes.Transparent;
            column5.MouseDown += Canvas_MouseDown;
            column5.Background = Brushes.Transparent;
            column6.MouseDown += Canvas_MouseDown;
            column6.Background = Brushes.Transparent;
            column7.MouseDown += Canvas_MouseDown;
            column7.Background = Brushes.Transparent;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs ea)
        {
            DependencyObject ucParent = (sender as WeekScheduler).Parent;
            while (!(ucParent is Scheduler))
            {
                ucParent = LogicalTreeHelper.GetParent(ucParent);
            }
            _scheduler = ucParent as Scheduler;

            _scheduler.OnEventAdded += ((s, e) =>
            {
                if (e.Start.Date == e.End.Date)
                    PaintAllEvents(e.Start);
                else
                    PaintAllDayEvents();
            });

            _scheduler.OnEventDeleted += ((s, e) =>
            {
                if (e.Start.Date == e.End.Date)
                    PaintAllEvents(e.Start);
                else
                    PaintAllDayEvents();
            });

            _scheduler.OnEventsModified += ((s, e) =>
            {
                PaintAllEvents(null);
                PaintAllDayEvents();
            });

            _scheduler.OnStartJourneyChanged += ((s, t) =>
            {
                if (_scheduler.StartJourney.Hours == 0)
                    startJourney.Visibility = Visibility.Hidden;
                else
                    Grid.SetRowSpan(startJourney, _scheduler.StartJourney.Hours);
            });

            _scheduler.OnEndJourneyChanged += ((s, t) =>
            {
                if (_scheduler.EndJourney.Hours == 0)
                    endJourney.Visibility = Visibility.Hidden;
                else
                {
                    Grid.SetRow(endJourney, _scheduler.EndJourney.Hours);
                    Grid.SetRowSpan(endJourney, 24 - _scheduler.EndJourney.Hours);
                }
            });

            SizeChanged += WeekScheduler_SizeChanged;

            ResizeGrids(new Size(ActualWidth, ActualHeight));
            PaintAllEvents(null);
            PaintAllDayEvents();
            if (_scheduler.StartJourney.Hours != 0)
            {
                double hourHeight = EventsGrid.ActualHeight / 22;
                ScrollEventsViewer.ScrollToVerticalOffset(hourHeight * (_scheduler.StartJourney.Hours - 1));
            }

            if (_scheduler.StartJourney.Hours == 0)
                startJourney.Visibility = Visibility.Hidden;
            else
                Grid.SetRowSpan(startJourney, _scheduler.StartJourney.Hours);

            if (_scheduler.EndJourney.Hours == 0)
                endJourney.Visibility = Visibility.Hidden;
            else
            {
                Grid.SetRow(endJourney, _scheduler.EndJourney.Hours);
                Grid.SetRowSpan(endJourney, 24 - _scheduler.EndJourney.Hours);
            }            
        }

        private void WeekScheduler_SizeChanged(object sender, SizeChangedEventArgs e)
        {            
            ResizeGrids(e.NewSize);
            PaintAllEvents(null);
            PaintAllDayEvents();            
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                int dayoffset = Convert.ToInt32(((Canvas)sender).Name.Replace("column", "")) - 1;
                OnScheduleDoubleClick?.Invoke(sender, new DateTime(FirstDay.Year, FirstDay.Month, FirstDay.Day).AddDays(dayoffset));
            }
        }

        private void PaintAllEvents(DateTime? date)
        {
            if (_scheduler == null || _scheduler.Events == null) return;

            IEnumerable<Event> eventList = _scheduler.Events.Where(ev => ev.Start.Date == ev.End.Date && !ev.AllDay).OrderBy(ev => ev.Start);

            if (date == null)
            {
                column1.Children.Clear();
                column2.Children.Clear();
                column3.Children.Clear();
                column4.Children.Clear();
                column5.Children.Clear();
                column6.Children.Clear();
                column7.Children.Clear();
            }
            else
            {
                int numColumn = (int)date.Value.Date.TruncateToDayStart().Subtract(FirstDay.Date).TotalDays + 1;
                ((Canvas)FindName("column" + numColumn))?.Children.Clear();

                eventList = eventList.Where(ev => ev.Start.Date == date.Value.Date).OrderBy(ev => ev.Start);
            }

            double columnWidth = EventsGrid.ColumnDefinitions[1].Width.Value;

            foreach (Event e in eventList)
            {
                int numColumn = (int)e.Start.Date.Subtract(FirstDay.Date).TotalDays + 1;

                if (numColumn >= 0 && numColumn <= 7 && e.Start.Date >= FirstDay.Date)
                {
                    Canvas sp = (Canvas)FindName("column" + numColumn);
                    sp.Width = columnWidth;

                    double oneHourHeight = sp.ActualHeight / 22;

                    var concurrentEvents = _scheduler.Events.Where(e1 => ((e1.Start <= e.Start && e1.End > e.Start) ||
                                                                    (e1.Start > e.Start && e1.Start < e.End)) &&
                                                                    e1.End.Date == e1.Start.Date).OrderBy(ev => ev.Start);

                    double marginTop = oneHourHeight * (e.Start.Hour + (e.Start.Minute / 60.0));
                    double width = columnWidth / (concurrentEvents.Count());
                    double marginLeft = width * GetIndex(e, concurrentEvents.ToList());

                    EventUserControl wEvent = new EventUserControl(e, true)
                    {
                        Width = width,
                        Height = e.End.Subtract(e.Start).TotalHours * oneHourHeight,
                        Margin = new Thickness(marginLeft, marginTop, 0, 0)
                    };

                    wEvent.MouseLeftButtonUp += ((object sender, MouseButtonEventArgs ea) =>
                    {
                        if (_doubleClicked)
                        {
                            _doubleClicked = false;
                            return;
                        }

                        foreach (var ev in eventList)
                            ev.IsSelected = false;

                        wEvent.Event.IsSelected = true;
                        PaintAllEvents(date);

                        ea.Handled = true;
                        OnEventClick(sender, wEvent.Event);
                    });

                    wEvent.MouseDoubleClick += ((sender, ea) =>
                    {
                        _doubleClicked = true;

                        foreach (var ev in eventList)
                            ev.IsSelected = false;
                        wEvent.Event.IsSelected = true;
                        PaintAllEvents(date);

                        ea.Handled = true;
                        OnEventDoubleClick?.Invoke(sender, wEvent.Event);
                    });

                    sp.Children.Add(wEvent);
                }
            }            
        }

        private int GetIndex(Event e, List<Event> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (e.Id == list[i].Id) return i;
            }
            return -1;
        }

        bool _doubleClicked = false;

        private void PaintAllDayEvents()
        {

            if (_scheduler?.Events == null) return;

            allDayEvents.Children.Clear();

            double columnWidth = EventsGrid.ColumnDefinitions[1].Width.Value;

            foreach (Event e in _scheduler.Events.Where(ev => ev.End.Date > ev.Start.Date || ev.AllDay))
            {
                int numColumn = (int)e.Start.Date.Subtract(FirstDay.Date).TotalDays;
                int numEndColumn = (int)e.End.Date.Subtract(FirstDay.Date).TotalDays + 1;

                if (numColumn >= 7 || numEndColumn <= 0) continue;

                if (numColumn < 0) numColumn = 0;
                if (numEndColumn > 7) numEndColumn = 7;

                if ((numColumn >= 0 && numColumn < 7) || (numEndColumn >= 0 && numEndColumn < 7))
                {
                    double marginLeft = (numColumn) * columnWidth;

                    EventUserControl wEvent = new EventUserControl(e, false)
                    {
                        Width = columnWidth * (numEndColumn - numColumn),
                        Margin = new Thickness(marginLeft, 0, 0, 0)
                    };

                    wEvent.MouseLeftButtonUp += ((object sender, MouseButtonEventArgs ea) =>
                    {
                        if (_doubleClicked)
                        {
                            _doubleClicked = false;
                            return;
                        }

                        ea.Handled = true;
                        OnEventClick(sender, wEvent.Event);
                    });

                    wEvent.MouseDoubleClick += ((sender, ea) =>
                    {
                        _doubleClicked = true;
                        ea.Handled = true;
                        OnEventDoubleClick?.Invoke(sender, wEvent.Event);
                    });
                    allDayEvents.Children.Add(wEvent);
                }
            }            
        }

        private void ResizeGrids(Size newSize)
        {
            EventsGrid.Width = newSize.Width;
            EventsHeaderGrid.Width = newSize.Width;

            double columnWidth = (ActualWidth - EventsGrid.ColumnDefinitions[0].ActualWidth) / 7;
            for (int i = 1; i < EventsGrid.ColumnDefinitions.Count; i++)
            {
                EventsGrid.ColumnDefinitions[i].Width = new GridLength(columnWidth);
                EventsHeaderGrid.ColumnDefinitions[i].Width = new GridLength(columnWidth);
            }
        }
    }
}
