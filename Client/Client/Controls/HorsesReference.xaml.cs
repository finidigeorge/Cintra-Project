using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client.ViewModels;
using Shared.Dto;

namespace Client.Controls
{
    /// <summary>
    /// Interaction logic for UserRolesReference.xaml
    /// </summary>
    public partial class HorsesReference : UserControl
    {
        public HorsesRefVm Model => (HorsesRefVm)Resources["ViewModel"];

        /*private bool isEditing;
        private bool isInserting;
        */

        public HorsesReference()
        {
            InitializeComponent();            
        }

        /*
        private void ItemsDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {
                if (e.EditAction == DataGridEditAction.Commit && e.Row.DataContext != null)
                {
                    if (isInserting)
                        Model.AddItemCommand.ExecuteAsync(e.Row.DataContext);
                    if (isEditing)
                        Model.EditItemCommand.ExecuteAsync(e.Row.DataContext);
                }
            }
            finally
            {
                isEditing = false;
                isInserting = false;
            }

        }

        private void ItemsDataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            isInserting = true;
        }

        private void ItemsDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            isEditing = true;
        }*/
    }
}
