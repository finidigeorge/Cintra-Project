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
                if (res.Item1)
                {
                    await Model.AddItemCommand.ExecuteAsync(res.Item2);
                }

            }, (x) => true);

            Model.UpdateDailyScheduledIntervalCommand = new Command<object>(async () => 
            {
                var res = ShowScheduleEditor(Model.SelectedItem);
                if (res.Item1)
                {
                    await Model.UpdateItemCommand.ExecuteAsync(res.Item2);
                }
                else
                {
                    await LoadSchedule();
                }
            }, (x) => Model.SelectedItem != null);

            Model.DeleteDailyScheduledIntervalCommand = new Command<object>(async () =>
            {                
                Model.BeginDeleteItemCommand.Execute(null);
                Model.SelectedItem = null;
                await LoadSchedule();

            }, (x) => Model.SelectedItem != null);

            InitGroupings();
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


        private (bool, BookingDtoUi) ShowScheduleEditor(BookingDtoUi bookingData = null)
        {
            var beginTime = Model.CurrentDate.TruncateToDayStart() + TimeSpan.FromHours(DateTime.Now.TruncateToCurrentHourStart().Hour);
            var endTime = beginTime.AddHours(1);

            BookingDtoUi _bookingData;
            if (bookingData == null)
            {
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

            var editor = new BookingEditWindow(beginTime, endTime, _bookingData) { Owner = this };            
                        
            var res = editor.ShowDialog() ?? false;            
            return (res, editor.Model.BookingData);
        }        

        private async void Window_LoadedAsync(object sender, RoutedEventArgs e)
        {
            await LoadSchedule();
        }
    }
}
