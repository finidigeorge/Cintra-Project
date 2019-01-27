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
using Client.Commands;
using Client.Controls.WpfScheduler;
using Client.Extentions;
using Client.ViewModels;
using Common.DtoMapping;
using Shared.Dto;
using Shared;
using Shared.Extentions;
using System.Threading;

namespace Client.Windows
{
    /// <summary>
    /// Interaction logic for ScheduleEditor.xaml
    /// </summary>
    public partial class ScheduleEditor : Window
    {
        private SolidColorBrush eventNotWorkingBrush = new SolidColorBrush(Colors.WhiteSmoke);
        private SolidColorBrush eventWorkingBrush = new SolidColorBrush(Colors.LightGreen);

        public SchedulesRefVm Model => (SchedulesRefVm)Resources["ViewModel"];

        public ScheduleEditor()
        {
            InitializeComponent();            

            #region Day Scheduler
            Model.NextDayCommand = new Command<object>((param) =>
            {
                DailyScheduler.NextPage();
            }, (x) => true);

            Model.PrevDayCommand = new Command<object>((param) =>
            {
                DailyScheduler.PrevPage();
            }, (x) => true);

            Model.AddDailyScheduledIntervalCommand = new AsyncCommand<object>(async (param) =>
            {
                var editorResult = ShowScheduleEditor(ScheduleIntervalEnum.Daily);
                if (editorResult.Item1)
                {
                    var e = GetEvent(editorResult.Item2);
                    e.UpdateFromScheduleDtoData(editorResult.Item2);                   
                    e.MergeToScheduleDtoData(ref editorResult.Item2);
                    editorResult.Item2.IntervalId = ScheduleIntervalEnum.Daily;
                    await Model.ScheduleDailyDataModel.AddItemCommand.ExecuteAsync(editorResult.Item2);                                        
                    
                    DailyScheduler.AddEvent(e);
                }

            }, (x) => Model.SelectedItem != null);

            Model.UpdateDailyScheduledIntervalCommand = new AsyncCommand<object>(async (param) => { await Task.FromResult(new object()); }, (x) => false);

            Model.DeleteDailyScheduledIntervalCommand = new AsyncCommand<object>(async (param) =>
            {                
                var item = Model.ScheduleDailyDataModel.SelectedItem;
                await Model.ScheduleDailyDataModel.BeginDeleteItemCommand.ExecuteAsync(item);
                DailyScheduler.DeleteEvent(item.EventGuid);
                Model.ScheduleDailyDataModel.SelectedItem = null;

            }, (x) => Model.ScheduleDailyDataModel.CanDeleteSelectedItem);

            #endregion

            #region Week Scheduler

            Model.NextWeekCommand = new Command<object>((param) =>
            {
                WeeklyScheduler.NextPage();
            }, (x) => true);

            Model.PrevWeekCommand = new Command<object>((param) =>
            {
                WeeklyScheduler.PrevPage();
            }, (x) => true);

            Model.AddWeeklyScheduledIntervalCommand = new AsyncCommand<object>(async (param) =>
            {
                var editorResult = ShowScheduleEditor(ScheduleIntervalEnum.Weekly);
                if (editorResult.Item1)
                {
                    var e = GetEvent(editorResult.Item2);
                    e.UpdateFromScheduleDtoData(editorResult.Item2);
                    e.MergeToScheduleDtoData(ref editorResult.Item2);

                    editorResult.Item2.DayNumber = editorResult.Item2.DayNumber;
                    editorResult.Item2.DateOn = null;
                    editorResult.Item2.IntervalId = ScheduleIntervalEnum.Weekly;
                    await Model.ScheduleWeeklyDataModel.AddItemCommand.ExecuteAsync(editorResult.Item2);

                    AddWeeklyEvent(editorResult.Item2, e);
                }

            }, (x) => Model.SelectedItem != null);

            Model.UpdateWeeklyScheduledIntervalCommand = new AsyncCommand<object>(async (param) => { await Task.FromResult(new object()); }, (x) => false);

            Model.DeleteWeeklyScheduledIntervalCommand = new AsyncCommand<object>(async (param) =>
            {
                var item = Model.ScheduleWeeklyDataModel.SelectedItem;
                await Model.ScheduleWeeklyDataModel.BeginDeleteItemCommand.ExecuteAsync(item);
                WeeklyScheduler.DeleteEvent(item.EventGuid);
                Model.ScheduleWeeklyDataModel.SelectedItem = null;

            }, (x) => Model.ScheduleWeeklyDataModel.CanDeleteSelectedItem);

            #endregion            

            Model.OnSelectedItemChanged += OnSelectedScheduleChanged;
            
            ReferenceVmHelper.SetupUiCommands(Model, ItemsDataGrid, columnIndex: 2, readOnlyMode: false);                                  
        }

        private async void OnSelectedScheduleChanged(object sender, ScheduleDtoUi s)
        {
            DailyScheduler.DeleteAllEvents();
            WeeklyScheduler.DeleteAllEvents();

            await Model.ScheduleDailyDataModel.RefreshDataCommand.ExecuteAsync(s);
            await Model.ScheduleWeeklyDataModel.RefreshDataCommand.ExecuteAsync(s);

            Model.ScheduleDailyDataModel.SelectedItem = null;
            Model.ScheduleWeeklyDataModel.SelectedItem = null;

            foreach (var item in Model.ScheduleDailyDataModel.Items.Where(x => x.IntervalId == ScheduleIntervalEnum.Daily))
            {
                var e = GetEvent(item);
                e.UpdateFromScheduleDtoData(item);
                DailyScheduler.AddEvent(e);
            }

            foreach (var item in Model.ScheduleWeeklyDataModel.Items.Where(x => x.IntervalId == ScheduleIntervalEnum.Weekly))
            {
                AddWeeklyEvent(item, GetEvent(item));
            }


        }

        private Event GetEvent(ScheduleDataDtoUi item)
        {
            return new Event() { Color = item.IsAvialable ? eventWorkingBrush : eventNotWorkingBrush };
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
            var beginTime = DailyScheduler.SelectedDate.TruncateToDayStart() + TimeSpan.FromHours(DateTime.Now.TruncateToCurrentHourStart().Hour);
            var endTime = DailyScheduler.SelectedDate.TruncateToDayStart() + TimeSpan.FromHours(DateTime.Now.TruncateToCurrentHourEnd().Hour);
            var editor = new SchedulerIntervalEditWindow(beginTime, endTime, mode)
            {
                DataContext = new ScheduleDataDtoUi()
                {
                    BeginTime = beginTime,
                    EndTime = endTime,
                    DateOn = DailyScheduler.SelectedDate.TruncateToDayStart(),                    
                    IsAvialable = true,
                    AvailabilityDescription = "Coaching"
                },
                Owner = this
            };

            var res = editor.ShowDialog() ?? false;
            editor.Model.ScheduleId = Model.SelectedItem.Id;
            return (res, editor.Model);
        }        

        private void DailyScheduler_OnEventClick(object sender, Event e)
        {
            Model.ScheduleDailyDataModel.SelectedItem = Model.ScheduleDailyDataModel.Items.FirstOrDefault(x => x.EventGuid == e.Id);
        }

        private void WeeklyScheduler_OnEventClick(object sender, Event e)
        {
            Model.ScheduleWeeklyDataModel.SelectedItem = Model.ScheduleWeeklyDataModel.Items.FirstOrDefault(x => x.EventGuid == e.Id);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Model.SelectedItem = Model.Items.FirstOrDefault();
            if (Model.SelectedItem != null)
                ItemsDataGrid.SelectedIndex = 0;
        }
    }
}
