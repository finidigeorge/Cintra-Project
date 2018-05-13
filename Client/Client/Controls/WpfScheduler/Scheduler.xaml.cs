using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Client.Controls.WpfScheduler
{
    public enum Mode
    {
        Day,
        Week
    }

    /// <summary>
    /// Interaction logic for WpfSchedule.xaml, adopted from 
    /// http://www.oscardelossantos.es/programacion/wpf-programacion/wpf-scheduler-control/
    /// </summary>
    public partial class Scheduler : UserControl
    {
        public event EventHandler<Event> OnEventClick;
        public event EventHandler<Event> OnEventDoubleClick;
        public event EventHandler<DateTime> OnScheduleDoubleClick;

        internal event EventHandler<Event> OnEventAdded;
        internal event EventHandler<Event> OnEventDeleted;
        internal event EventHandler OnEventsModified;

        internal event EventHandler<TimeSpan> OnStartJourneyChanged;
        internal event EventHandler<TimeSpan> OnEndJourneyChanged;


        #region SelectedDate
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register("SelectedDate", typeof(DateTime), typeof(Scheduler),
            new FrameworkPropertyMetadata(SelectedDateChanged));

        public DateTime SelectedDate
        {
            get { return (DateTime)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        private static void SelectedDateChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            DateTime SelectedDate = (DateTime)e.NewValue;
            Scheduler sc = source as Scheduler;
            sc.DayScheduler.CurrentDay = SelectedDate;
            sc.WeekScheduler.FirstDay = SelectedDate;
        }
        #endregion

        #region Events
        public static readonly DependencyProperty EventsProperty =
            DependencyProperty.Register("Events", typeof(ObservableCollection<Event>), typeof(Scheduler),
            new FrameworkPropertyMetadata(AdjustEvents));

        public ObservableCollection<Event> Events
        {
            get { return (ObservableCollection<Event>)GetValue(EventsProperty); }
            set { SetValue(EventsProperty, value); }
        }

        private static void AdjustEvents(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if ((source as Scheduler).OnEventsModified != null) 
                (source as Scheduler).OnEventsModified(source, null);
        }
        #endregion

        #region StartJourney
        public static readonly DependencyProperty StartJourneyProperty =
            DependencyProperty.Register("StartJourney", typeof(TimeSpan), typeof(Scheduler),
            new FrameworkPropertyMetadata(StartJourneyChanged));

        public TimeSpan StartJourney
        {
            get { return (TimeSpan)GetValue(StartJourneyProperty); }
            set { SetValue(StartJourneyProperty, value); }
        }

        private static void StartJourneyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if ((source as Scheduler).OnStartJourneyChanged != null)
                (source as Scheduler).OnStartJourneyChanged(source, (TimeSpan)e.NewValue);
        }
        #endregion

        #region EndJourney
        public static readonly DependencyProperty EndJourneyProperty =
            DependencyProperty.Register("EndJourney", typeof(TimeSpan), typeof(Scheduler),
            new FrameworkPropertyMetadata(EndJourneyChanged));

        public TimeSpan EndJourney
        {
            get { return (TimeSpan)GetValue(EndJourneyProperty); }
            set { SetValue(EndJourneyProperty, value); }
        }

        private static void EndJourneyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if ((source as Scheduler).OnEndJourneyChanged != null)
                (source as Scheduler).OnEndJourneyChanged(source, (TimeSpan)e.NewValue);
        }
        #endregion

        #region DayScheduler
        internal static readonly DependencyProperty DaySchedulerProperty =
            DependencyProperty.Register("DayScheduler", typeof(Client.Controls.WpfScheduler.DayScheduler), typeof(Scheduler),
            new FrameworkPropertyMetadata());

        internal Client.Controls.WpfScheduler.DayScheduler DayScheduler
        {
            get { return ucDayScheduler; }
        }
        #endregion

        #region WeekScheduler
        internal static readonly DependencyProperty WeekSchedulerProperty =
            DependencyProperty.Register("WeekScheduler", typeof(WeekScheduler), typeof(Scheduler),
            new FrameworkPropertyMetadata());

        internal WeekScheduler WeekScheduler
        {
            get { return ucWeekScheduler; }
        }
        #endregion

        #region Mode
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(Mode), typeof(Scheduler),
            new FrameworkPropertyMetadata(ModeChanged));

        public Mode Mode
        {
            get { return (Mode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        private static void ModeChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            Mode mode = (Mode)e.NewValue;
            Scheduler sc = source as Scheduler;
            sc.DayScheduler.Visibility = (mode == Mode.Day ? Visibility.Visible : Visibility.Hidden);
            sc.WeekScheduler.Visibility = (mode == Mode.Week ? Visibility.Visible : Visibility.Hidden);            

            switch (mode)
            {
                case Mode.Week:
                    sc.DayScheduler.CurrentDay = sc.WeekScheduler.FirstDay;
                    break;
                case Mode.Day:
                    sc.WeekScheduler.FirstDay = sc.DayScheduler.CurrentDay;
                    break;
            }
        }
        #endregion

        public bool EnableEventsIntersection { get; set; } = false;

        public Scheduler()
        {
            InitializeComponent();
            Mode = Mode.Week;
            Events = new ObservableCollection<Event>();
            SelectedDate = DateTime.Now;

            WeekScheduler.OnEventClick += InnerScheduler_OnEventClick;
            DayScheduler.OnEventClick += InnerScheduler_OnEventClick;

            WeekScheduler.OnEventDoubleClick += InnerScheduler_OnEventDoubleClick;
            DayScheduler.OnEventDoubleClick += InnerScheduler_OnEventDoubleClick;

            WeekScheduler.OnScheduleDoubleClick += InnerScheduler_OnScheduleDoubleClick;
            DayScheduler.OnScheduleDoubleClick += InnerScheduler_OnScheduleDoubleClick;
        }

        void InnerScheduler_OnScheduleDoubleClick(object sender, DateTime e)
        {
            OnScheduleDoubleClick?.Invoke(sender, e);
        }

        void InnerScheduler_OnEventDoubleClick(object sender, Event e)
        {
            OnEventDoubleClick?.Invoke(sender, e);
        }

        void InnerScheduler_OnEventClick(object sender, Event e)
        {
            OnEventClick?.Invoke(sender, e);
        }

        public void AddEvent(Event e)
        {
            if (e.Start > e.End)
                throw new ArgumentException("End date is before Start date");

            if (!EnableEventsIntersection && Events.Any(x => x.Start < e.End && x.End > e.Start))
                throw new ArgumentException("Can't add new scheduled event because it intersects another one added before");

            Events.Add(e);

            OnEventAdded?.Invoke(this, e);
        }

        public void DeleteEvent(Guid id)
        {
            Event e = Events.SingleOrDefault(ev => ev.Id.Equals(id));
            if (e != null)
            {
                DateTime date = e.Start;
                Events.Remove(e);
                OnEventDeleted?.Invoke(this, e);
            }
        }

        public void DeleteAllEvents()
        {
            foreach (var e in Events.ToList())
            {
                Events.Remove(e);
                OnEventDeleted?.Invoke(this, e);
            }
        }

        public void NextPage()
        {
            switch (Mode)
            {
                case Mode.Day:
                    SelectedDate = SelectedDate.AddDays(1);
                    break;
                case Mode.Week:
                    SelectedDate = SelectedDate.AddDays(7);
                    break;                
            }
        }

        public void PrevPage()
        {
            switch (Mode)
            {
                case Mode.Day:
                    SelectedDate = SelectedDate.AddDays(-1);
                    break;
                case Mode.Week:
                    SelectedDate = SelectedDate.AddDays(-7);
                    break;
            }
        }
    }
}
