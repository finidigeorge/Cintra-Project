using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Client.Commands;
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
            BeginEditItemCommand = new Command<bool>(() =>
            {
                EditItemEventHandler(this, null);
            }, x => true);
        }

        public void NewItemEventHandler(object sender, RoutedEventArgs e)
        {
            var newItem = new UserDtoUi();
            Model.Items.Add(newItem);
            ItemsDataGrid.ScrollIntoView(newItem);
            ItemsDataGrid.SelectedItem = newItem;
            var selectedIndex = ItemsDataGrid.SelectedIndex;

            ItemsDataGrid.SelectionUnit = DataGridSelectionUnit.Cell;
            ItemsDataGrid.CurrentCell = new DataGridCellInfo(ItemsDataGrid.Items[selectedIndex], ItemsDataGrid.Columns[0]);
            ItemsDataGrid.BeginEdit();            
        }

        

        public void EditItemEventHandler(object sender, RoutedEventArgs e)
        {            
            var selectedIndex = ItemsDataGrid.SelectedIndex;

            ItemsDataGrid.SelectionUnit = DataGridSelectionUnit.Cell;
            ItemsDataGrid.CurrentCell = new DataGridCellInfo(ItemsDataGrid.Items[selectedIndex], ItemsDataGrid.Columns[0]);
            ItemsDataGrid.BeginEdit();
        }        
    }
}
