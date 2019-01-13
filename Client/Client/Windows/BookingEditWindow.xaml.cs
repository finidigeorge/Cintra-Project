using Client.Commands;
using Client.Controls.WpfScheduler;
using Client.Extentions;
using Client.ViewModels;
using Common;
using Common.DtoMapping;
using Shared;
using Shared.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFCustomMessageBox;

namespace Client.Windows
{
    /// <summary>
    /// Interaction logic for BookingEditWindow.xaml
    /// </summary>
    public partial class BookingEditWindow : Window
    {
        private SolidColorBrush weeklyEventBrush = new SolidColorBrush(Colors.WhiteSmoke);

        public BookingEditWindowVm Model => (BookingEditWindowVm)Resources["ViewModel"];

        private Event SelectedEvent { get; set; }

        public BookingEditWindow()
        {
            InitializeComponent();
        }

        public BookingEditWindow(BookingDtoUi bookindData, bool isEditMode)
        {
            InitializeComponent();
            Model.IsEditMode = isEditMode;
            Model.BookingData = bookindData;
            Model.RecurrentScheduler = WeeklyScheduler;
            BeginTimePicker.Model.CurrentTime = bookindData.BeginTime;
            EndTimePicker.Model.CurrentTime = bookindData.EndTime;

            Model.BookingData.PropertyChanged += (sender, args) => {
                if (args.PropertyName == "EndTime")
                {
                    EndTimePicker.Model.CurrentTime = Model.BookingData.EndTime;
                }
            };

            BeginTimePicker.Model.PropertyChanged += (sender, args) => {
                if (args.PropertyName == "CurrentTime")
                {
                    Model.BookingData.BeginTime = Model.BookingData.DateOn.SetTime(((TimePickerVm)sender).CurrentTime);
                }
            };

            EndTimePicker.Model.PropertyChanged += (sender, args) => {
                if (args.PropertyName == "CurrentTime")
                {
                    Model.BookingData.EndTime = Model.BookingData.DateOn.SetTime(((TimePickerVm)sender).CurrentTime);
                }
            };

            Model.AddWeeklyScheduledIntervalCommand = new Command<object>(() =>
            {
                var editorResult = ShowScheduleEditor(ScheduleIntervalEnum.Weekly, Model.BookingData.BeginTime, Model.BookingData.EndTime);
                if (editorResult.Item1)
                {
                    var e = new Event() { Color = weeklyEventBrush, IsFirstWeek = (Model.WeekNumber == 0) };
                    AddWeeklyEvent(editorResult.Item2, e);
                }

            }, (x) => true);

            Model.UpdateWeeklyScheduledIntervalCommand = new Command<object>(() => { }, (x) => false);

            Model.DeleteWeeklyScheduledIntervalCommand = new Command<object>(() =>
            {
                WeeklyScheduler.DeleteEvent(SelectedEvent.Id);
            }, (x) => SelectedEvent != null);

            Model.NextWeekCommand = new Command<object>(() =>
            {
                WeeklyScheduler.NextPage();
                Model.WeekNumber++;

            }, (x) => Model.WeekNumber == 0);

            Model.PrevWeekCommand = new Command<object>(() =>
            {
                WeeklyScheduler.PrevPage();
                Model.WeekNumber--;
            }, (x) => Model.WeekNumber == 1);
        }

        private void AddWeeklyEvent(ScheduleDataDtoUi item, Event e)
        {
            e.UpdateFromScheduleDtoData(item);
            e.Start = WeeklyScheduler.WeekScheduler.FirstDay.TruncateToDayStart().AddDays((double)item.DayNumber) + item.BeginTime.TimeOfDay;
            e.End = WeeklyScheduler.WeekScheduler.FirstDay.TruncateToDayStart().AddDays((double)item.DayNumber) + item.EndTime.TimeOfDay;
            WeeklyScheduler.AddEvent(e);
        }

        private (bool, ScheduleDataDtoUi) ShowScheduleEditor(ScheduleIntervalEnum mode, DateTime begin, DateTime end)
        {
            var beginTime = begin;
            var endTime = end;

            var editor = new SchedulerIntervalEditWindow(beginTime, endTime, mode, false)
            {
                DataContext = new ScheduleDataDtoUi()
                {
                    BeginTime = beginTime,
                    EndTime = endTime,
                    IsAvialable = true,
                    AvailabilityDescription = Model.BookingData.BookingPayment.PaymentOptions
                },
                Owner = this
            };

            var res = editor.ShowDialog() ?? false;
            return (res, editor.Model);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Model.CoachValidationFitScheduleWarning))
            {
                MessageBoxResult result = CustomMessageBox.Show($"{Model.CoachValidationFitScheduleWarning}. Do you want to proceed ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                    return;
            }

            if (!string.IsNullOrEmpty(Model.HorseValidationHoursPerDayWarning))
            {
                MessageBoxResult result = CustomMessageBox.Show($"{Model.HorseValidationHoursPerDayWarning}. Do you want to proceed ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                    return;
            }

            if (!string.IsNullOrEmpty(Model.HorseValidationHoursInRowWarning))
            {
                MessageBoxResult result = CustomMessageBox.Show($"{Model.HorseValidationHoursInRowWarning}. Do you want to proceed ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                    return;
            }

            DialogResult = true;
            Close();
        }

        private void CenterWindowOnScreen(int hight)
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = hight;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is TabControl tc)
            {
                var item = (TabItem)tc.SelectedItem;
                if (item == null || (item.IsSelected && item.TabIndex != 1))
                {
                    Width = 740;
                    MaxHeight = 875;
                    CenterWindowOnScreen((int)MaxHeight);
                    return;
                }

                if (item.IsSelected &&  item.TabIndex == 1)
                {
                    Height = 1000;
                    MaxHeight = 1000;
                    Width = 1280;

                    CenterWindowOnScreen((int)MaxHeight);
                    return;
                }
            }
        }

        private void WeeklyScheduler_OnEventClick(object sender, Controls.WpfScheduler.Event e)
        {
            SelectedEvent = e;
        }
    }
}

