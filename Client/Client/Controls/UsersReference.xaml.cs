using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Client.Commands;
using Client.Extentions;
using Client.ViewModels;
using Common.DtoMapping;

namespace Client.Controls
{
    /// <summary>
    /// Interaction logic for UsersReference.xaml
    /// </summary>
    public partial class UsersReference : UserControl
    {
        public UsersRefVm Model => (UsersRefVm)Resources["ViewModel"];

        public ICommand BeginEditItemCommand { get; set; }

        public UsersReference()
        {
            InitializeComponent();
            ReferenceVmHelper.SetupUiCommands(Model, ItemsDataGrid);
        }        
    }
}
