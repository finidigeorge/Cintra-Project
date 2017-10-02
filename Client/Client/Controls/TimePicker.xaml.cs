using Bindables;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client.Controls
{
    /// <summary>
    /// Interaction logic for ucDateTimeUpDown.xaml
    /// adopted from http://helpyourselfhere.blogspot.com.au/2015/03/wpf-simple-timepicker-user-control-for.html
    /// </summary>    
    public partial class TimePicker : UserControl, INotifyPropertyChanged
    {
        #region Private Member variable

        private DateTime _currentTime = DateTime.UtcNow;
        private ObservableCollection<string> _amPmTypes = new ObservableCollection<string>();
        private string _displayAmPm;

        #endregion

        #region Constructors

        public TimePicker()
        {
            InitializeComponent();
            this.DataContext = this;
            AmPmTypes.Add("AM");
            AmPmTypes.Add("PM");
            CurrentTime = DateTime.UtcNow.ToLocalTime();            
        }

        #endregion

        #region Public Properties

        public ObservableCollection<string> AmPmTypes
        {
            get => _amPmTypes;
            set => _amPmTypes = value;
        }

        public int MinsInterval { get; set; } = 15;

        public int HoursInterval { get; set; } = 1;


        public string DisplayTime => _currentTime.ToLocalTime().ToString("t");

        public string DisplayAmPm
        {
            get
            {
                if (_currentTime.ToLocalTime().Hour >= 0 && _currentTime.ToLocalTime().Hour < 12)
                    _displayAmPm = AmPmTypes.FirstOrDefault(s => s.Equals("AM"));
                else
                {
                    if (_currentTime.ToLocalTime().Hour >= 12)
                    {
                        _displayAmPm = AmPmTypes.FirstOrDefault(s => s.Equals("PM"));
                    }
                }

                return _displayAmPm;
            }
            set
            {
                if (!value.Equals(_displayAmPm))
                {
                    if (value.Equals("PM"))
                        CurrentTime = CurrentTime.ToLocalTime().AddHours(12);
                    else
                    {
                        CurrentTime = CurrentTime.ToLocalTime().AddHours(-12);
                    }
                }
                _displayAmPm = value;
            }
        }

        public string DisplayTimeHours
        {
            get
            {
                var hours = _currentTime.ToLocalTime().Hour;
                return hours > 12 ? (hours - 12).ToString("00") : hours.ToString("00");                
            }
            set
            {
                var hour = 0;
                Int32.TryParse(value, out hour);
                CurrentTime = CurrentTime.ToLocalTime().AddHours(hour);
                OnPropertyChanged("DisplayTime");
                OnPropertyChanged("DisplayTimeHours");
                OnPropertyChanged("DisplayTimeMinutes");
            }
        }

        public string DisplayTimeMinutes
        {
            get { return _currentTime.ToLocalTime().Minute.ToString("00"); }
            set
            {
                var minutes = 0;
                Int32.TryParse(value, out minutes);
                CurrentTime = CurrentTime.ToLocalTime().AddMinutes(minutes);
                OnPropertyChanged("DisplayTime");
                OnPropertyChanged("DisplayTimeHours");
                OnPropertyChanged("DisplayTimeMinutes");
            }
        }

        public DateTime CurrentTime
        {
            get { return _currentTime; }
            set
            {
                _currentTime = value;

                OnPropertyChanged("CurrentTime");
                OnPropertyChanged("DisplayTime");
                OnPropertyChanged("DisplayTimeHours");
                OnPropertyChanged("DisplayTimeMinutes");
                OnPropertyChanged("DisplayAmPm");                
            }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty SelectedTimeProperty = DependencyProperty.Register(
          "CurrentTime", typeof(DateTime), typeof(TimePicker), new PropertyMetadata(default(DateTime)));

        #endregion

        #region Methods

        private void MinutesUpButton_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentTime = CurrentTime.AddMinutes(MinsInterval);            
        }

        private void MinutesDownButton_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentTime = CurrentTime.AddMinutes(-MinsInterval);            
        }

        private void HourUpButton_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentTime = CurrentTime.AddHours(HoursInterval);            
        }

        private void HourDownButton_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentTime = CurrentTime.AddHours(-HoursInterval);            
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox) sender).Text.Any(x => !char.IsDigit(x)))
            {
                Dispatcher.BeginInvoke(new Action(() => ((TextBox) sender).Undo()));
            }
        }

        private void AddHoursTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemPlus)
            {
                CurrentTime = CurrentTime.AddHours(HoursInterval);
            }
            if (e.Key == Key.OemMinus)
            {
                CurrentTime = CurrentTime.AddHours(-HoursInterval);
            }
        }

        private void AddMinutesTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemPlus)
            {
                CurrentTime = CurrentTime.AddMinutes(MinsInterval);
            }
            if (e.Key == Key.OemMinus)
            {
                CurrentTime = CurrentTime.AddMinutes(-MinsInterval);
            }
        }
    }
}
