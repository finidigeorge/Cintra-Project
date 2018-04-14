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

        public BookingEditWindow(DateTime beginTime, DateTime endTime, BookingDtoUi bookindData, bool isEditMode)
        {
            InitializeComponent();
            Model.BookingData = bookindData;
            Model.IsEditMode = isEditMode;
            Model.RecurrentScheduler = WeeklyScheduler;
            BeginTimePicker.Model.CurrentTime = beginTime;
            EndTimePicker.Model.CurrentTime = endTime;

            Model.BookingData.PropertyChanged += (sender, args) => {
                if (args.PropertyName == "EndTime")
                {
                    EndTimePicker.Model.CurrentTime = Model.BookingData.EndTime;
                }
            };

            BeginTimePicker.Model.PropertyChanged += (sender, args) => {
                if (args.PropertyName == "CurrentTime")
                {
                    Model.BookingData.BeginTime = Model.BookingData.DateOn + (((TimePickerVm)sender).CurrentTime - ((TimePickerVm)sender).CurrentTime.TruncateToDayStart()); 
                }
            };

            EndTimePicker.Model.PropertyChanged += (sender, args) => {
                if (args.PropertyName == "CurrentTime")
                {
                    Model.BookingData.EndTime = Model.BookingData.DateOn + (((TimePickerVm)sender).CurrentTime - ((TimePickerVm)sender).CurrentTime.TruncateToDayStart());
                }
            };

            Model.AddWeeklyScheduledIntervalCommand = new Command<object>(() =>
            {
                var editorResult = ShowScheduleEditor(ScheduleIntervalEnum.Weekly);
                if (editorResult.Item1)
                {
                    var e = new Event() { Color = weeklyEventBrush };                                                            
                    AddWeeklyEvent(editorResult.Item2, e);
                }

            }, (x) => true);

            Model.UpdateWeeklyScheduledIntervalCommand = new Command<object>(() => { }, (x) => false);

            Model.DeleteWeeklyScheduledIntervalCommand = new Command<object>(() =>
            {                        
                WeeklyScheduler.DeleteEvent(SelectedEvent.Id);                
            }, (x) => SelectedEvent != null);
        }

        private void AddWeeklyEvent(ScheduleDataDtoUi item, Event e)
        {
            e.UpdateFromScheduleDtoData(item);
            e.Start = WeeklyScheduler.WeekScheduler.FirstDay.TruncateToDayStart().AddDays((double)item.DayNumber) + item.BeginTime.TimeOfDay;
            e.End = WeeklyScheduler.WeekScheduler.FirstDay.TruncateToDayStart().AddDays((double)item.DayNumber) + item.EndTime.TimeOfDay;
            WeeklyScheduler.AddEvent(e);
        }

        private (bool, ScheduleDataDtoUi) ShowScheduleEditor(ScheduleIntervalEnum mode)
        {
            var beginTime = DateTime.Now.TruncateToCurrentHourStart();
            var endTime = DateTime.Now.TruncateToCurrentHourEnd();

            var editor = new SchedulerIntervalEditWindow(beginTime, endTime, mode, false)
            {
                DataContext = new ScheduleDataDtoUi()
                {
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

        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
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
                    CenterWindowOnScreen();
                    return;
                }

                if (item.IsSelected &&  item.TabIndex == 1)
                {
                    Height = 900;
                    MaxHeight = 900;
                    Width = 1280;

                    CenterWindowOnScreen();
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

