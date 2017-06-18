using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Client.Controls
{
    public static class DataGridHelper
    {
        public static void GrabFocus<T>(this DataGrid dataGrid, IList<T> items)
        {
            if (items?.Any() == true)
                dataGrid.SelectedItem = items[0];

            dataGrid.Focus();
            if (dataGrid.SelectedCells.Count == 0)
            {
                dataGrid.CurrentCell =
                    new DataGridCellInfo(dataGrid.Items[0], dataGrid.Columns[0]);
            }
        }
    }
}
