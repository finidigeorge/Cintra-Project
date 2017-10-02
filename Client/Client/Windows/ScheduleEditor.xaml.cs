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

namespace Client.Windows
{
    /// <summary>
    /// Interaction logic for ScheduleEditor.xaml
    /// </summary>
    public partial class ScheduleEditor : Window
    {
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

            Model.AddScheduledIntervalCommand = new Command<object>(() =>
            {
                if (ShowScheduleEditor())
                {
                    ///
                }

            }, (x) => true);
        }

        private bool ShowScheduleEditor()
        {
            var editor = new SchedulerIntervalEditWindow()
            {
                Owner = this,
            };            

            return editor.ShowDialog() ?? false;
        }

        private void DailyScheduler_OnOnScheduleDoubleClick(object sender, DateTime e)
        {
            DailyScheduler.AddEvent(new Event() {Start = e + new TimeSpan(10, 0, 0), End = e + new TimeSpan(11, 30, 0), Subject = "Test !!!!", Color = new SolidColorBrush(Color.FromRgb(52, 168, 255))});
        }

        private void DailyScheduler_OnOnEventDoubleClick(object sender, Event e)
        {
            DailyScheduler.DeleteEvent(e.Id);
        }
    }
}
