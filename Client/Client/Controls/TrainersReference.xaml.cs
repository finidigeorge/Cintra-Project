using System.Windows.Controls;
using Client.ViewModels;

namespace Client.Controls
{

    public partial class TrainersReference : UserControl
    {
        public TrainersRefVm Model => (TrainersRefVm)Resources["ViewModel"];

        public TrainersReference()
        {
            InitializeComponent();            
        }

        public void OnActivated()
        {
            ItemsDataGrid.GrabFocus(Model.Items);
        }        
    }
}
