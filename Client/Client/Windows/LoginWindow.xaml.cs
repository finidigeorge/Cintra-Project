using System.Windows;
using Client.ViewModels;

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
                await context.LoginCommand.ExecuteAsync(passwordBox.Password);
                if (context.IsAuthenticated)
                    Close();
            }
        }
    }
}
