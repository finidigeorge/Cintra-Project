using System.Windows.Controls;
using Client.Extentions;
using Client.ViewModels;

namespace Client.Controls
{

    public partial class CoachesReference : UserControl
    {
        public CoachesRefVm Model => (CoachesRefVm)Resources["ViewModel"];

        public CoachesReference()
        {
            InitializeComponent();
            ReferenceVmHelper.SetupUiCommands(Model, ItemsDataGrid);
        }        
    }
}
