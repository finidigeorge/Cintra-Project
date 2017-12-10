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
    /// Interaction logic for PaymentTypesReference.xaml
    /// </summary>
    public partial class PaymentTypesReference : UserControl
    {
        public PaymentTypesRefVm Model => (PaymentTypesRefVm)Resources["ViewModel"];

        public PaymentTypesReference()
        {
            InitializeComponent();
            ReferenceVmHelper.SetupUiCommands(Model, ItemsDataGrid);
        }
    }
}
