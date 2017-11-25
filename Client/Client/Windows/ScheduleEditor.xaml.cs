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

namespace Client.Windows
{
    /// <summary>
    /// Interaction logic for ScheduleEditor.xaml
    /// </summary>
    public partial class ScheduleEditor : Window
    {
        private SolidColorBrush dailyEventBrush = new SolidColorBrush(Color.FromRgb(52, 168, 255));
        private SolidColorBrush weeklyEventBrush = new SolidColorBrush(Colors.LightSeaGreen);

        public SchedulesRefVm Model => (SchedulesRefVm)Resources["ViewModel"];

        public ScheduleEditor()
        {
            InitializeComponent();
            ReferenceVmHelper.SetupUiCommands(Model, ItemsDataGrid);

            #region Day Scheduler
            Model.NextDayCommand = new Command<object>(() =>
            {
                DailyScheduler.NextPage();
            }, (x) => true);

            Model.PrevDayCommand = new Command<object>(() =>
            {
                DailyScheduler.PrevPage();
            }, (x) => true);

            Model.AddDailyScheduledIntervalCommand = new Command<object>(async  () =>
            {
                var editorResult = ShowDailyScheduleEditor();
                if (editorResult.Item1)
                {
                    var e = new Event() { Color = dailyEventBrush };
                    e.UpdateFromScheduleDtoData(editorResult.Item2);                   
                    e.MergeToScheduleDtoData(ref editorResult.Item2);
                    editorResult.Item2.IntervalId = Shared.ScheduleIntervalEnum.Daily;
                    await Model.ScheduleDailyDataModel.AddItemCommand.ExecuteAsync(editorResult.Item2);                                        
                    
                    DailyScheduler.AddEvent(e);
                }

            }, (x) => Model.SelectedItem != null);

            Model.UpdateDailyScheduledIntervalCommand = new Command<object>(() => { }, (x) => false);

            Model.DeleteDailyScheduledIntervalCommand = new Command<object>(() =>
            {                
                var item = Model.ScheduleDailyDataModel.SelectedItem;
                Model.ScheduleDailyDataModel.BeginDeleteItemCommand.Execute(item);
                DailyScheduler.DeleteEvent(item.EventGuid);
                Model.ScheduleDailyDataModel.SelectedItem = null;

            }, (x) => Model.ScheduleDailyDataModel.CanDeleteSelectedItem);

            #endregion

            #region Week Scheduler

            Model.NextWeekCommand = new Command<object>(() =>
            {
                WeeklyScheduler.NextPage();
            }, (x) => true);

            Model.PrevWeekCommand = new Command<object>(() =>
            {
                WeeklyScheduler.PrevPage();
            }, (x) => true);

            Model.AddWeeklyScheduledIntervalCommand = new Command<object>(async () =>
            {
                var editorResult = ShowDailyScheduleEditor();
                if (editorResult.Item1)
                {
                    var e = new Event() { Color = weeklyEventBrush };
                    e.UpdateFromScheduleDtoData(editorResult.Item2);
                    e.MergeToScheduleDtoData(ref editorResult.Item2);
                    editorResult.Item2.DayNumber = (int)editorResult.Item2.DateOn.Value.DayOfWeek;
                    editorResult.Item2.IntervalId = Shared.ScheduleIntervalEnum.Weekly;
                    await Model.ScheduleWeeklyDataModel.AddItemCommand.ExecuteAsync(editorResult.Item2);

                    WeeklyScheduler.AddEvent(e);
                }

            }, (x) => Model.SelectedItem != null);

            Model.UpdateWeeklyScheduledIntervalCommand = new Command<object>(() => { }, (x) => false);

            Model.DeleteWeeklyScheduledIntervalCommand = new Command<object>(() =>
            {
                var item = Model.ScheduleWeeklyDataModel.SelectedItem;
                Model.ScheduleWeeklyDataModel.BeginDeleteItemCommand.Execute(item);
                WeeklyScheduler.DeleteEvent(item.EventGuid);
                Model.ScheduleWeeklyDataModel.SelectedItem = null;

            }, (x) => Model.ScheduleWeeklyDataModel.CanDeleteSelectedItem);

            #endregion

            Model.OnSelectedItemChanged += OnSelectedScheduleChanged;
            
        }

        private async void OnSelectedScheduleChanged(object sender, ScheduleDtoUi s)
        {
            DailyScheduler.DeleteAllEvents();
            WeeklyScheduler.DeleteAllEvents();

            await Model.ScheduleDailyDataModel.RefreshDataCommand.ExecuteAsync(s);
            Model.ScheduleDailyDataModel.SelectedItem = null;
            Model.ScheduleWeeklyDataModel.SelectedItem = null;

            foreach (var item in Model.ScheduleDailyDataModel.Items.Where(x => x.IntervalId == Shared.ScheduleIntervalEnum.Daily))
            {
                var e = new Event() { Color = dailyEventBrush };
                e.UpdateFromScheduleDtoData(item);
                DailyScheduler.AddEvent(e);
            }

            foreach (var item in Model.ScheduleWeeklyDataModel.Items.Where(x => x.IntervalId == Shared.ScheduleIntervalEnum.Weekly))
            {
                var e = new Event() { Color = dailyEventBrush };
                e.UpdateFromScheduleDtoData(item);
                e.Start = WeeklyScheduler.WeekScheduler.FirstDay.AddDays((double)item.DayNumber);
                WeeklyScheduler.AddEvent(e);
            }


        }

        private (bool, ScheduleDataDtoUi) ShowDailyScheduleEditor()
        {
            var beginTime = DailyScheduler.SelectedDate.TruncateToDayStart() + TimeSpan.FromHours(6);
            var endTime = DailyScheduler.SelectedDate.TruncateToDayStart() + TimeSpan.FromHours(21);
            var editor = new SchedulerIntervalEditWindow(beginTime, endTime)
            {
                DataContext = new ScheduleDataDtoUi()
                {
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
    }
}
