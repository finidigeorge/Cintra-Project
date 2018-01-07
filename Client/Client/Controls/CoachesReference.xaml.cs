using System.Windows;
using System.Windows.Controls;
using Client.Commands;
using Client.Extentions;
using Client.ViewModels;
using Client.Windows;

namespace Client.Controls
{

    public partial class CoachesReference : UserControl
    {
        public CoachesRefVm Model => (CoachesRefVm)Resources["ViewModel"];

        public CoachesReference()
        {
            InitializeComponent();
            ReferenceVmHelper.SetupUiCommands(Model, ItemsDataGrid, columnIndex: 2);
            Model.DisplayEditItemScheduleCommand = new Command<object>(ShowScheduleEditor, (x) => Model.CanEditSelectedItem);
        }

        private void ShowScheduleEditor()
        {
            var editor = new ScheduleEditor()
            {
                Owner = Application.Current.MainWindow,
            };

            editor.Model.Coach = Model.SelectedItem;
            editor.Model.DataSource = Model.SelectedItem.Schedules;
            editor.Model.RefreshDataCommand.ExecuteAsync(null);

            editor.ShowDialog();
        }
    }
}
