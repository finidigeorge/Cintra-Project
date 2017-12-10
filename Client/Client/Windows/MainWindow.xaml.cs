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
using Common;
using RestApi;
using RestClient;
using WPFCustomMessageBox;
using Client.Commands;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowVm Model => (MainWindowVm)Resources["ViewModel"];

        public MainWindow()
        {
            InitializeComponent();            
        }

        private bool CheckAndWarnAuth()
        {
            var result = Model.AuthVm.IsAuthenticated;

            if (!result)
                CustomMessageBox.Show(Messages.YOU_NEED_TO_LOGIN_MSG, "Attention", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            return result;
        }

        private string selectedTab = "Horses";

        private async void OnTabSelectionChanged(Object sender, SelectionChangedEventArgs args)
        {
            var tabCommands = new Dictionary<string, IAsyncCommand>() 
                {
                    { "Coaches", CoachesRefView.Model.RefreshDataCommand },
                    { "Clients", ClientsRefView.Model.RefreshDataCommand },                    
                    { "Horses", HorsesRefView.Model.RefreshDataCommand },
                    { "PaymentTypes", PaymentTypesRefView.Model.RefreshDataCommand },
                    { "Services", ServicesRefView.Model.RefreshDataCommand },                    
                    { "UserRoles", UserRolesRefView.Model.RefreshDataCommand },
                    { "Users", UsersRefView.Model.RefreshDataCommand },
                };

            if (sender is TabControl tc)
            {
                var item = (TabItem)tc.SelectedItem;
                if (item.IsSelected && selectedTab != item.Name)
                {
                    selectedTab = ((TabItem)tc.SelectedItem).Name;

                    if (CheckAndWarnAuth())
                    {
                        await tabCommands[selectedTab].ExecuteAsync(null);
                    }
                }
            }
        }
    }
}
