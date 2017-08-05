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
        public static void EditItemEventHandler(this DataGrid value, object sender, RoutedEventArgs e)
        {
            var selectedIndex = value.SelectedIndex;

            //value.SelectionUnit = DataGridSelectionUnit.Cell;
            value.CurrentCell = new DataGridCellInfo(value.Items[selectedIndex], value.Columns[0]);
            value.BeginEdit();
        }        
    }
}
