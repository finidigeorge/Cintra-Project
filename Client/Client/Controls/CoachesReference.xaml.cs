using System.Windows.Controls;
using Client.ViewModels;

namespace Client.Controls
{

    public partial class CoachesReference : UserControl
    {
        public CoachesRefVm Model => (CoachesRefVm)Resources["ViewModel"];

        public CoachesReference()
        {
            InitializeComponent();            
        }        
    }
}
