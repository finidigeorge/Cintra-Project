using Client.Extentions;
using Client.ViewModels;
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

namespace Client.Controls
{
    /// <summary>
    /// Interaction logic for ClientsReference.xaml
    /// </summary>
    public partial class ClientsReference : UserControl
    {
        public ClientsRefVm Model => (ClientsRefVm)Resources["ViewModel"];

        public ClientsReference()
        {
            InitializeComponent();
            ReferenceVmHelper.SetupUiCommands(Model, ItemsDataGrid, columnIndex: 1);
        }

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
        }
    }
}
