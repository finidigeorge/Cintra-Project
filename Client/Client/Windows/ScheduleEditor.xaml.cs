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

namespace Client.Windows
{
    /// <summary>
    /// Interaction logic for ScheduleEditor.xaml
    /// </summary>
    public partial class ScheduleEditor : Window
    {
        private SolidColorBrush dailyEventBrush = new SolidColorBrush(Color.FromRgb(52, 168, 255));

        public SchedulesRefVm Model => (SchedulesRefVm)Resources["ViewModel"];

        public ScheduleEditor()
        {
            InitializeComponent();
            ReferenceVmHelper.SetupUiCommands(Model, ItemsDataGrid);            

            Model.NextDayCommand = new Command<object>(() =>
            {
                DailyScheduler.NextPage();
            }, (x) => true);

            Model.PrevDayCommand = new Command<object>(() =>
            {
                DailyScheduler.PrevPage();
            }, (x) => true);

            Model.AddDailyScheduledIntervalCommand = new Command<object>(() =>
            {
                var editorResult = ShowDailyScheduleEditor();
                if (editorResult.Item1)
                {
                    var e = new Event() { Color = dailyEventBrush };
                    e.UpdateFromScheduleDtoData(editorResult.Item2);
                    e.MergeToScheduleDtoData(ref editorResult.Item2);
                    Model.ScheduleDataModel.AddItemCommand.Execute(editorResult.Item2);
                                        
                    DailyScheduler.AddEvent(e);
                }

            }, (x) => Model.SelectedItem != null);

            Model.UpdateDailyScheduledIntervalCommand = new Command<object>(() => { }, (x) => false);

            Model.DeleteDailyScheduledIntervalCommand = new Command<object>(async () =>
            {
                await Model.ScheduleDataModel.DeleteItemCommand.ExecuteAsync(Model.ScheduleDataModel.SelectedItem);
                DailyScheduler.DeleteEvent(Model.ScheduleDataModel.SelectedItem.EventGuid);
                Model.ScheduleDataModel.SelectedItem = null;

            }, (x) => Model.ScheduleDataModel.SelectedItem != null);
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

        private void DailyScheduler_OnOnScheduleDoubleClick(object sender, DateTime e)
        {
            //DailyScheduler.AddEvent(new Event() { Start = e + new TimeSpan(10, 0, 0), End = e + new TimeSpan(11, 30, 0), Subject = "Test !!!!", Color = new SolidColorBrush(Color.FromRgb(52, 168, 255)) });
        }

        private void DailyScheduler_OnOnEventDoubleClick(object sender, Event e)
        {
            /*
            Model.DeleteDailyScheduledIntervalCommand.Execute(Model.ScheduleDataModel.Items.First(x => x.EventGuid == e.Id));
            DailyScheduler.DeleteEvent(e.Id);
            */
        }

        private void DailyScheduler_OnEventClick(object sender, Event e)
        {
            Model.ScheduleDataModel.SelectedItem = Model.ScheduleDataModel.Items.FirstOrDefault(x => x.EventGuid == e.Id);
        }
    }
}
