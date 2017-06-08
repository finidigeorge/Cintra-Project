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
using Client.ViewModels;
using RestApi;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            
            //dgUserRoles.ItemsSource = .Result;            
            
        }

        private async void OnTabSelectionChanged(Object sender, SelectionChangedEventArgs args)
        {
            var tc = sender as TabControl;
            if (tc != null)
            {
                var item = (TabItem)tc.SelectedItem;
                if (item.IsSelected)
                {
                    if (item.Name == "UserRoles")
                    {
                        var vm = new UserRolesVm();
                        await vm.GetRolesCommand.ExecuteAsync(null);
                        dgUserRoles.ItemsSource = vm.UserRoles;
                    }
                }
            }
        }
    }
}
