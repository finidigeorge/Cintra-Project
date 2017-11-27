using Client.Commands;
using Client.Controls.WpfScheduler;
using Client.Extentions;
using Client.ViewModels;
using Common.DtoMapping;
using Shared.Dto;
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

namespace Client.Windows
{
    /// <summary>
    /// Interaction logic for BookingWindow.xaml
    /// </summary>
    public partial class BookingWindow : Window
    {
        private SolidColorBrush dailyEventBrush = new SolidColorBrush(Color.FromRgb(255, 240, 240));
        private SolidColorBrush dailyEventPayedBrush = new SolidColorBrush(Color.FromRgb(240, 255, 240));

        public BookingRefVm Model => (BookingRefVm)Resources["ViewModel"];

        public BookingWindow()
        {
            InitializeComponent();
            DailyScheduler.SelectedDate = Model.CurrentDate;

            Model.NextDayCommand = new Command<object>(async () =>
            {
                DailyScheduler.NextPage();
                await LoadSchedule();
            }, (x) => true);

            Model.PrevDayCommand = new Command<object>(async () =>
            {
                DailyScheduler.PrevPage();                
                await LoadSchedule();
            }, x => DailyScheduler.SelectedDate >= DateTime.Now);

            Model.AddDailyScheduledIntervalCommand = new Command<object>(async () =>
            {
                var res = ShowScheduleEditor();
                if (res.Item1)
                {
                    await CreateSchedulerEvent(res.Item2);
                }

            }, (x) => true);

            Model.UpdateDailyScheduledIntervalCommand = new Command<object>(() => { }, (x) => false);

            Model.DeleteDailyScheduledIntervalCommand = new Command<object>(async () =>
            {
                DailyScheduler.DeleteEvent(Model.SelectedItem.EventGuid);
                await Model.DeleteSelectedItemCommand.ExecuteAsync(null);
                Model.SelectedItem = null;

            }, (x) => Model.SelectedItem != null);            

        }

        private async Task LoadSchedule()
        {
            Model.CurrentDate = DailyScheduler.SelectedDate.TruncateToDayStart();
            await Model.RefreshDataCommand.ExecuteAsync(null);
            DailyScheduler.DeleteAllEvents();

            foreach (var booking in Model.Items)
            {
                var e = new Event() { Color = booking.BookingPayment.IsPaid ? dailyEventPayedBrush : dailyEventBrush };
                e.UpdateFromBookingDataUi(booking);                
                DailyScheduler.AddEvent(e);
            }
        }

        private async Task CreateSchedulerEvent(BookingDtoUi booking)
        {
            var e = new Event() { Color = booking.BookingPayment.IsPaid ? dailyEventPayedBrush : dailyEventBrush };
            e.UpdateFromBookingDataUi(booking);
            e.MergeToScheduleDtoData(ref booking);
            
            await Model.AddItemCommand.ExecuteAsync(booking);

            DailyScheduler.AddEvent(e);
        }

        private (bool, BookingDtoUi) ShowScheduleEditor()
        {
            var beginTime = DailyScheduler.SelectedDate.TruncateToDayStart() + TimeSpan.FromHours(6);
            var endTime = DailyScheduler.SelectedDate.TruncateToDayStart() + TimeSpan.FromHours(21);
            var bookingData = new BookingDtoUi()
            {
                DateOn = DailyScheduler.SelectedDate.TruncateToDayStart(),
                BookingPayment = new BookingPaymentDto(),
                BeginTime = beginTime,
                EndTime = endTime
            };
            var editor = new BookingEditWindow(beginTime, endTime, bookingData) { Owner = this };            
                        
            var res = editor.ShowDialog() ?? false;            
            return (res, editor.Model.BookingData);
        }

        private void DailyScheduler_OnEventClick(object sender, Controls.WpfScheduler.Event e)
        {
            Model.SelectedItem = Model.Items.FirstOrDefault(x => x.EventGuid == e.Id);
        }

        private async void Window_LoadedAsync(object sender, RoutedEventArgs e)
        {
            await LoadSchedule();
        }
    }
}
