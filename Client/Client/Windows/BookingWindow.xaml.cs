using Client.Commands;
using Client.Controls.WpfScheduler;
using Client.Extentions;
using Client.ViewModels;
using Common.DtoMapping;
using Shared.Extentions;
using Mapping;
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
    class BookingData
    {
        public bool IsBooked { get; set; }
        public BookingDtoUi Booking { get; set; }
        public bool HasRecurrentBookings{ get; set; }
        public List<Event> RecurrentBookings { get; set; }
        public DateTime RecurentDateStart { get; set; }
        public int RecurrentWeekNumber { get; set; }
        public bool IsPermanent { get; set; }
    };

    public class Grouping
    {
        public string Name { get; set; }        
    }

    /// <summary>
    /// Interaction logic for BookingWindow.xaml
    /// </summary>
    public partial class BookingWindow : Window
    {        
        public BookingRefVm Model => (BookingRefVm)Resources["ViewModel"];

        public BookingWindow()
        {
            InitializeComponent();            
            Model.PropertyChanged += async (s, e) => 
            {
                if (e.PropertyName == nameof(Model.CurrentDate))
                {
                    await LoadSchedule();
                }
            };

            Model.CurrentDate = DateTime.Now.TruncateToDayStart();

            Model.NextDayCommand = new Command<object>(() =>
            {
                Model.CurrentDate = Model.CurrentDate.AddDays(1);                
            }, (x) => true);

            Model.PrevDayCommand = new Command<object>(() =>
            {
                Model.CurrentDate = Model.CurrentDate.AddDays(-1);                
            }, x => true);

            Model.AddDailyScheduledIntervalCommand = new Command<object>(async () =>
            {
                var res = ShowScheduleEditor();
                await CreateBooking(res);

            }, (x) => true);

            Model.UpdateDailyScheduledIntervalCommand = new Command<object>(async () => 
            {
                var res = ShowScheduleEditor(Model.SelectedItem);
                if (res.IsBooked)
                {
                    await Model.UpdateItemCommand.ExecuteAsync(res.Booking);
                }
                else
                {
                    await LoadSchedule();
                }
            }, (x) => Model.SelectedItem != null);

            Model.DeleteDailyScheduledIntervalCommand = new Command<object>(async () =>
            {                
                if (Model.SelectedItem.BookingTemplateMetadata != null)
                { 
                    var dlg = new BookingDeleteWindow() { Owner = this };
                    var res = dlg.ShowDialog() ?? false;
                    if (res)
                    {
                        if (dlg.Model.DeleteRecurringBookings)
                        {
                            await Model.CancelAll(Model.SelectedItem.BookingTemplateMetadata.Id, dlg.Model.RecurringStartDate);
                        }

                        if (dlg.Model.DeleteSelectedBooking)
                        {
                            await Model.DeleteSelectedItemCommand.ExecuteAsync(Model.SelectedItem);
                        }
                    }
                }
                else
                {
                    Model.BeginDeleteItemCommand.Execute(null);                    
                }

                Model.SelectedItem = null;
                await LoadSchedule();

            }, (x) => Model.SelectedItem != null);

            InitGroupings();
        }

        private async Task CreateBooking(BookingData res)
        {
            if (res.IsBooked)
            {
                if (res.HasRecurrentBookings && res.IsPermanent && res.RecurrentBookings.Any())
                {
                    res.Booking.BookingTemplateMetadata = new BookingTemplateMetadataDto()
                    {
                        StartDate = res.RecurentDateStart,
                        BookingTemplates = MergePermanentEventData(res)
                    };
                }

                await Model.AddItemCommand.ExecuteAsync(res.Booking);

                if (res.HasRecurrentBookings && !res.IsPermanent && res.RecurrentBookings.Any())
                {
                    var items = MergeEventData(res);
                    await Model.InsertAll(items);
                }
            }
        }

        private List<BookingDto> MergeEventData(BookingData data)
        {
            var result = new List<BookingDto>();

            for (int w= 0; w < data.RecurrentWeekNumber; w++)
            {
                foreach (var e in data.RecurrentBookings)
                {
                    var item = ObjectMapper.Map<BookingDto>(data.Booking);
                    item.BeginTime = e.Start.AddDays(7 * w);
                    item.EndTime = e.End.AddDays(7 * w);

                    item.DateOn = item.BeginTime.TruncateToDayStart();
                    result.Add(item);
                }
            }
            return result;
        }

        private List<BookingDto> MergePermanentEventData(BookingData data)
        {
            var result = new List<BookingDto>();            
            foreach (var e in data.RecurrentBookings)
            {
                var item = ObjectMapper.Map<BookingDto>(data.Booking);                
                result.Add(item);
            }
            
            return result;
        }

        private void InitGroupings()
        {
            var groupDescription = new PropertyGroupDescription("BeginTimeRoundedFmtd");
            groupDescription.GroupNames.Add("06:00 AM");            
            groupDescription.GroupNames.Add("07:00 AM");            
            groupDescription.GroupNames.Add("08:00 AM");            
            groupDescription.GroupNames.Add("09:00 AM");            
            groupDescription.GroupNames.Add("10:00 AM");            
            groupDescription.GroupNames.Add("11:00 AM");            
            groupDescription.GroupNames.Add("12:00 PM");            
            groupDescription.GroupNames.Add("01:00 PM");            
            groupDescription.GroupNames.Add("02:00 PM");            
            groupDescription.GroupNames.Add("03:00 PM");            
            groupDescription.GroupNames.Add("04:00 PM");            
            groupDescription.GroupNames.Add("05:00 PM");            
            groupDescription.GroupNames.Add("06:00 PM");            
            groupDescription.GroupNames.Add("07:00 PM");            

            Model.ItemsCollectionView.GroupDescriptions.Add(groupDescription);            
        }

        private async Task LoadSchedule()
        {            
            await Model.RefreshDataCommand.ExecuteAsync(null);
        }


        private BookingData ShowScheduleEditor(BookingDtoUi bookingData = null, DateTime? begin = null, DateTime? end = null)
        {
            var beginTime = begin.HasValue ? begin.Value : Model.CurrentDate.TruncateToDayStart() + TimeSpan.FromHours(DateTime.Now.TruncateToCurrentHourStart().Hour);
            var endTime = end.HasValue ? end.Value : beginTime.AddHours(1);
            var IsEditMode = true;

            BookingDtoUi _bookingData;
            if (bookingData == null)
            {
                IsEditMode = false;
                _bookingData = new BookingDtoUi()
                {
                    DateOn = Model.CurrentDate.TruncateToDayStart(),
                    BookingPayment = new BookingPaymentDto(),
                    BeginTime = beginTime,
                    EndTime = endTime
                };
            }
            else
                _bookingData = bookingData;

            var editor = new BookingEditWindow(beginTime, endTime, _bookingData, IsEditMode) { Owner = this };            
                        
            var res = editor.ShowDialog() ?? false;            
            return new BookingData()
            {
                IsBooked = res,
                Booking = editor.Model.BookingData,
                HasRecurrentBookings = editor.Model.EnableRecurringApointments,
                RecurrentBookings = editor.Model.RecurrentScheduler.Events.ToList(),
                RecurentDateStart = editor.Model.RecurringStartDate,
                RecurrentWeekNumber = editor.Model.RecurringWeeksNumber,
                IsPermanent = editor.Model.IsPermanent
            };
        }        

        private async void Window_LoadedAsync(object sender, RoutedEventArgs e)
        {
            await LoadSchedule();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var group = sender as Button;
            if (group != null)
            {
                var dateOn = Model.CurrentDate.TruncateToDayStart();
                var item = ((CollectionViewGroup)group.DataContext).Name.ToString();

                var dateBegin = dateOn.Add(DateTime.ParseExact(item, "hh:mm tt", System.Globalization.CultureInfo.InvariantCulture).TimeOfDay);
                var dateEnd = dateBegin.AddHours(1);

                var res = ShowScheduleEditor(null, dateBegin, dateEnd);
                await CreateBooking(res);
            }
        }
    }
}
