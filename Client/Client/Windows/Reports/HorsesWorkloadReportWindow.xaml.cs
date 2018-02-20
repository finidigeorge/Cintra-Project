
using Client.ViewModels;
using Client.ViewModels.Reports;
using Common.DtoMapping;
using Shared.Extentions;
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
using System.Windows.Shapes;

namespace Client.Windows.Reports
{
    /// <summary>
    /// Interaction logic for BookingEditWindow.xaml
    /// </summary>
    public partial class HorsesWorkloadReportWindow : Window
    {
        public HorsesWorkloadReportVm Model => (HorsesWorkloadReportVm)Resources["ViewModel"];

        public HorsesWorkloadReportWindow()
        {
            InitializeComponent();            
        }                

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;            
        }
    }
}
