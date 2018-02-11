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
using System.Windows.Shapes;

namespace Client.Windows
{
    /// <summary>
    /// Interaction logic for BookingDeleteWindow.xaml
    /// </summary>
    public partial class BookingDeleteWindow : Window
    {
        public BookingDeleteWindowVm Model => (BookingDeleteWindowVm)Resources["ViewModel"];

        public BookingDeleteWindow()
        {
            InitializeComponent();
            Model.DeleteSelectedBooking = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
