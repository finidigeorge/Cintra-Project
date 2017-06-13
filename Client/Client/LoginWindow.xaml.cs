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
using Client.ViewModels;
using RestApi;
using RestClient;

namespace Client
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();                       
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var context = DataContext as AuthVm;
            if (context != null)
            {
                await context.LoginCommand.ExecuteAsync(passwordBox);
                if (context.IsAuthenticated)
                    Close();
            }
        }
    }
}
