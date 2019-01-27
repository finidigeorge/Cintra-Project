using Client.Commands;
using Client.ViewModels;
using Common;
using Common.DtoMapping;
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
    /// Interaction logic for HorseEditWindow.xaml
    /// </summary>
    public partial class HorseEditWindow : Window
    {
        public HorsesEditWindowVm Model => (HorsesEditWindowVm)Resources["ViewModel"];

        public HorseEditWindow()
        {
            InitializeComponent();            
            
            Model.AddUnavalabilityInterval = new AsyncCommand<object>(async (param) => await ShowAddIntervalDialog());
            Model.DeleteUnavalabilityInterval = new AsyncCommand<object>(async (param) => await DeleteInterval());
        }

        private async Task ShowAddIntervalDialog()
        {
            var editor = new HorseEditScheduleWindow()
            {
                DataContext = new HorseScheduleDataDtoUi()
                {
                    StartDate = DateTime.Now.TruncateToDayStart(), EndDate = DateTime.Now.TruncateToDayStart().AddDays(1).AddSeconds(-1),
                    UnavailabilityType = Shared.HorsesUnavailabilityEnum.DayOff,
                    HorseId = Model.HorseData.Id,                   
                }
            };


            if ((editor.ShowDialog() ?? false) == true)
            {
                var data = (HorseScheduleDataDtoUi)editor.DataContext;
                if (data.IsDayOfWeek)
                {
                    data.StartDate = null;
                    data.EndDate = null;
                }

                if (data.IsDateInterval)
                {
                    data.DayOfWeek = null;                    
                }

                await Model.HorsesScheduleModel.AddItemCommand.ExecuteAsync(data);
            }
        }

        private async Task DeleteInterval()
        {
            MessageBoxResult result = CustomMessageBox.Show(Messages.DELETE_RECODRD_CONFIRM_MSG, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await Model.HorsesScheduleModel.DeleteSelectedItemCommand.ExecuteAsync(null);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
