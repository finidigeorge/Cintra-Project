using Client.ViewModels;
using Common.DtoMapping;
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
    /// Interaction logic for BookingEditWindow.xaml
    /// </summary>
    public partial class BookingEditWindow : Window
    {
        public BookingEditWindowVm Model => (BookingEditWindowVm)Resources["ViewModel"];

        public BookingEditWindow()
        {
            InitializeComponent();
        }

        public BookingEditWindow(DateTime beginTime, DateTime endTime, BookingDtoUi bookindData)
        {
            InitializeComponent();
            Model.BookingData = bookindData;
            BeginTimePicker.Model.CurrentTime = beginTime;
            EndTimePicker.Model.CurrentTime = endTime;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Model.BookingData.BeginTime = BeginTimePicker.Model.CurrentTime;
            Model.BookingData.EndTime = EndTimePicker.Model.CurrentTime;

            DialogResult = true;
            Close();
        }        
    }
}
