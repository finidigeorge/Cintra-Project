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
            Model.BeginEditItemCommand = new Command<object>(() =>
            {
                ItemsDataGrid.EditItemEventHandler(Model.SelectedItem, null);
            }, (x) => Model.CanEditSelectedItem);

            Model.BeginAddItemCommand = new Command<object>(() =>
            {
                ItemsDataGrid.SelectedItem = Model.AddEmptyItem();
                ItemsDataGrid.ScrollIntoView(ItemsDataGrid.SelectedItem);

                Model.BeginEditItemCommand.Execute(ItemsDataGrid);
            }, (x) => Model.CanAddItem);            
        }        
    }
}
