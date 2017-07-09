using System.Windows.Controls;
using Client.ViewModels;

namespace Client.Controls
{
    /// <summary>
    /// Interaction logic for UsersReference.xaml
    /// </summary>
    public partial class UsersReference : UserControl
    {
        public UsersRefVm Model => (UsersRefVm)Resources["ViewModel"];

        public UsersReference()
        {
            InitializeComponent();            
        }        
    }
}
