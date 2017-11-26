using Client.Commands;
using Client.ViewModels;
using Common.DtoMapping;
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
        private SolidColorBrush dailyEventBrush = new SolidColorBrush(Colors.WhiteSmoke);

        public BookingRefVm Model => (BookingRefVm)Resources["ViewModel"];

        public BookingWindow()
        {
            InitializeComponent();

            Model.NextDayCommand = new Command<object>(() =>
            {
                DailyScheduler.NextPage();
            }, (x) => true);

            Model.PrevDayCommand = new Command<object>(() =>
            {
                DailyScheduler.PrevPage();
            }, x => DailyScheduler.SelectedDate >= DateTime.Now);

            Model.AddDailyScheduledIntervalCommand = new Command<object>(async () =>
            {
                

            }, (x) => Model.SelectedItem != null);

            Model.UpdateDailyScheduledIntervalCommand = new Command<object>(() => { }, (x) => false);

            Model.DeleteDailyScheduledIntervalCommand = new Command<object>(() =>
            {
                

            }, (x) => Model.SelectedItem != null);

            Model.OnSelectedItemChanged += OnSelectedScheduleChanged;
        }

        private async void OnSelectedScheduleChanged(object sender, BookingDtoUi s)
        {
            DailyScheduler.DeleteAllEvents();                                  
        }

        private void DailyScheduler_OnEventClick(object sender, Controls.WpfScheduler.Event e)
        {
            Model.SelectedItem = Model.Items.FirstOrDefault(x => x.EventGuid == e.Id);
        }
    }
}
