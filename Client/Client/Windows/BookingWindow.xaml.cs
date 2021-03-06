﻿using Client.Commands;
using Client.Controls.WpfScheduler;
using Client.ViewModels;
using Common.DtoMapping;
using Shared.Extentions;
using Mapping;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Client.Windows.Helpers;

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
        public bool IsFortnightly { get; set; }        
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

            var refreshCommand = Model.RefreshDataCommand;
            Model.RefreshDataCommand = new AsyncCommand<object>(async (obj) =>
            {
                try
                {
                    Model.IsLoading = true;
                    await refreshCommand.ExecuteAsync(null);
                }
                finally
                {
                    Model.IsLoading = false;
                }
            }, (x) => true);

            Model.NextDayCommand = new Command<object>((param) =>
            {
                Model.CurrentDate = Model.CurrentDate.AddDays(1);                
            }, (x) => true);

            Model.PrevDayCommand = new Command<object>((param) =>
            {
                Model.CurrentDate = Model.CurrentDate.AddDays(-1);                
            }, x => true);

            Model.AddDailyScheduledIntervalCommand = new AsyncCommand<object>(async (param) =>
            {
                var res = ShowScheduleEditor();
                await CreateNewBooking(res);

            }, (x) => Model.HasAdminRights());            

            Model.UpdateDailyScheduledIntervalCommand = new AsyncCommand<object>(async (param) => 
            {
                var res = ShowScheduleEditor(Model.SelectedItem);
                if (res.IsBooked)
                {
                    await Model.UpdateItemCommand.ExecuteAsync(res.Booking);
                    await LoadSchedule();
                }
            }, (x) => Model.SelectedItem != null && Model.HasAdminRights());

            Model.DeleteDailyScheduledIntervalCommand = new AsyncCommand<object>(async (param) =>
            {                
                if (Model.SelectedItem.BookingTemplateMetadata != null)
                { 
                    var dlg = new BookingDeleteWindow() { Owner = this };
                    dlg.Model.RecurringStartDate = Model.CurrentDate.AddDays(1);

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
                    await Model.BeginDeleteItemCommand.ExecuteAsync(null);                    
                }

                Model.SelectedItem = null;
                await LoadSchedule();

            }, (x) => Model.SelectedItem != null && Model.HasAdminRights());

            InitGroupings();
        }

        private async Task CreateNewBooking(BookingData res)
        {
            if (res.IsBooked)
            {
                if (res.HasRecurrentBookings && res.IsPermanent && res.RecurrentBookings.Any())
                {
                    res.Booking.BookingTemplateMetadata = new BookingTemplateMetadataDto()
                    {
                        StartDate = res.RecurentDateStart,                        
                        IsFortnightly = res.IsFortnightly,                        
                        BookingTemplates = MergePermanentEventData(res)
                    };
                }

                await Model.AddItemCommand.ExecuteAsync(res.Booking);

                if (res.HasRecurrentBookings && !res.IsPermanent && res.RecurrentBookings.Any())
                {
                    var items = GenerateBookingEventData(res);
                    await Model.InsertAll(items);
                }
            }
        }

        private List<BookingDto> GenerateBookingEventData(BookingData data)
        {
            var result = new List<BookingDto>();
            int fortNightCoeff = data.IsFortnightly ? 2 : 1;
            for (int w= 0; w < data.RecurrentWeekNumber; w++)
            {
                foreach (var e in data.RecurrentBookings)
                {
                    var item = ObjectMapper.Map<BookingDto>(data.Booking);
                    item.BeginTime = e.Start.AddDays(7 * fortNightCoeff * w);
                    item.EndTime = e.End.AddDays(7 * fortNightCoeff * w);
                    
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
                e.MergeToScheduleDtoData(ref item);
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
            var IsEditMode = true;

            BookingDtoUi _bookingData;
            if (bookingData == null)
            {
                var beginTime = begin.HasValue ? begin.Value : Model.CurrentDate.TruncateToDayStart() + TimeSpan.FromHours(DateTime.Now.TruncateToCurrentHourStart().Hour);
                var endTime = end.HasValue ? end.Value : beginTime.AddHours(1);

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

            var editor = new BookingEditWindow(_bookingData, IsEditMode) { Owner = this };
            
            // wait for the dialog window to be closed
            var helper = new CustomShowModalHelper(editor);
            helper.ShowAndWait();                        

            return new BookingData()
            {
                IsBooked = helper.GetResult(),
                IsFortnightly = editor.Model.IsFortnightly,                
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
                await CreateNewBooking(res);
            }
        }
    }
}
