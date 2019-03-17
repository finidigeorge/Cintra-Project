using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Client.Commands;
using Client.Extentions;
using Client.ViewModels;
using Client.Windows;
using Common.DtoMapping;

namespace Client.Controls
{
    /// <summary>
    /// Interaction logic for UserRolesReference.xaml
    /// </summary>
    public partial class HorsesReference : UserControl
    {
    
        public HorsesRefVm Model => (HorsesRefVm)Resources["ViewModel"];        

        public HorsesReference()
        {
            InitializeComponent();
            ReferenceVmHelper.SetupUiCommands(Model, ItemsDataGrid, columnIndex: 2);   
            Model.ShowAvalabilityEditorCommand = new AsyncCommand<object>(async(param) => await ShowScheduleEditor(), (x) => true);
        }

        private async Task ShowScheduleEditor()
        {
            var editor = new HorseEditWindow()
            {
                Owner = Application.Current.MainWindow,
            };

            editor.Model.HorseData = Model.SelectedItem;
            editor.ShowDialog();
            Model.SelectedItem.AllowedCoaches.Clear();
            Model.SelectedItem.AllowedCoaches.AddRange(editor.Model.GetSelectedCoached());                

            await Model.UpdateSelectedItemCommand.ExecuteAsync(Model.SelectedItem);            
        }

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
        }

        private void SearchTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape) SearchTextBox.Text = string.Empty;            
        }
    }


}
