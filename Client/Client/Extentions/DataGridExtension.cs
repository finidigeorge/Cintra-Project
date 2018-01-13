using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Client.Extentions
{
    public static class DataGridExtension
    {
        public static void EditItemEventHandler(this DataGrid grid, object sender, RoutedEventArgs e, int columnIndex)
        {
            var selectedIndex = grid.SelectedIndex;

            //grid.SelectionUnit = DataGridSelectionUnit.Cell;
            grid.CurrentCell = new DataGridCellInfo(grid.Items[selectedIndex], grid.Columns[columnIndex]);
            grid.BeginEdit();
        }        
    }
}
