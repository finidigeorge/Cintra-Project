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
    /// Interaction logic for HorseEditScheduleWindow.xaml
    /// </summary>
    public partial class HorseEditScheduleWindow : Window
    {
        public HorseScheduleDataDtoUi Model => (HorseScheduleDataDtoUi)DataContext;

        public HorseEditScheduleWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
