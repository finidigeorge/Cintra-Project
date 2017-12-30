using Client.Commands;
using Client.ViewModels;
using Common;
using Common.DtoMapping;
using Common.Extentions;
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
            Model.AddUnavalabilityInterval = new Command<object>(ShowAddIntervalDialog);
        }

        private void ShowAddIntervalDialog()
        {
            var editor = new HorseEditScheduleWindow()
            {
                DataContext = new HorseScheduleDataDtoUi()
                {
                    StartDate = DateTime.Now.TruncateToDayStart(), EndDate = DateTime.Now.TruncateToDayStart().AddDays(1),
                    UnavailabilityType = Shared.HorsesUnavailabilityEnum.DayOff
                }
            };


            if ((editor.ShowDialog() ?? false) == true)
            {
                Model.HorsesScheduleModel.AddItemCommand.Execute((HorseScheduleDataDtoUi)editor.DataContext);
            }
        }

        private void DeleteInterval()
        {
            MessageBoxResult result = CustomMessageBox.Show(Messages.DELETE_RECODRD_CONFIRM_MSG, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Model.HorsesScheduleModel.DeleteSelectedItemCommand.Execute(null);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
