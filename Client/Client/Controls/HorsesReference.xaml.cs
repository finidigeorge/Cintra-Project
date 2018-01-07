using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            Model.ShowAvalabilityEditorCommand = new Command<object>(() => ShowScheduleEditor(), (x) => true);
        }

        private void ShowScheduleEditor()
        {
            var editor = new HorseEditWindow()
            {
                Owner = Application.Current.MainWindow,
            };

            editor.Model.HorseData = Model.SelectedItem;            
            editor.ShowDialog();
        }
    }


}
