using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public class TimePickerVm: BaseVm
    {
        #region Private Member variable

        private DateTime _currentTime = DateTime.UtcNow;
        private ObservableCollection<string> _amPmTypes = new ObservableCollection<string>();
        private string _displayAmPm;

        #endregion

        #region Constructors

        public TimePickerVm() { }

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
                Int32.TryParse(value, out int hour);
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
                Int32.TryParse(value, out int minutes);
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
    }
}
