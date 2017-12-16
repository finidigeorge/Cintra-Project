using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Client.ViewModels;
using Common;
using WPFCustomMessageBox;
using Client.Commands;
using System.Threading;
using Client.Windows;
using Client.Windows.Reports;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowVm Model => (MainWindowVm)Resources["ViewModel"];

        bool bookingWindowIsRunning = false;
        private string selectedTab = "Horses";

        public MainWindow()
        {
            InitializeComponent();

            Model.ShowBookingWindowCommand = new Command<object>(() =>
            {
                var thread = new Thread(() =>
                {
                    var bookingWindow = new BookingWindow() { Owner = this };
                    bookingWindowIsRunning = true;
                    bookingWindow.Show();

                    bookingWindow.Closed += (sender2, e2) => { bookingWindow.Dispatcher.InvokeShutdown(); bookingWindowIsRunning = false; };

                    System.Windows.Threading.Dispatcher.Run();
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();

            }, x => Model.AuthVm.IsAuthenticated && !bookingWindowIsRunning);

            Model.ShowLoginDialogCommand = new Command<object>(() =>
            {
                new LoginWindow() { DataContext = Model.AuthVm, Owner = this }.ShowDialog();

            }, x => true);

            Model.ShowChangePasswordDialogCommand = new Command<object>(() =>
            {
                new ChangePasswordWindow() { DataContext = new ChangePasswordVm(), Owner = this }.ShowDialog();
            }, x => true);

            Model.ShowExitDialogCommand = new Command<object>(ExitAppCommandAction, x => true);

            Model.RunClientHistoryReportCommand = new Command<object>(() =>
            {
                new ClientLessonsReportWindow() { Owner = this }.ShowDialog();
            }, x => Model.AuthVm.IsAuthenticated && Model.AuthVm.IsAdmin);
        }

        private bool CheckAndWarnAuth()
        {
            var result = Model.AuthVm.IsAuthenticated;

            if (!result)
                CustomMessageBox.Show(Messages.YOU_NEED_TO_LOGIN_MSG, "Attention", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            return result;
        }

        private void ExitAppCommandAction()
        {
            MessageBoxResult result = CustomMessageBox.Show(Messages.EXIT_APP_MSG, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }        

        private async void OnTabSelectionChanged(Object sender, SelectionChangedEventArgs args)
        {
            var tabCommands = new Dictionary<string, IAsyncCommand>() 
                {
                    { "Coaches", CoachesRefView.Model.RefreshDataCommand },
                    { "Clients", ClientsRefView.Model.RefreshDataCommand },                    
                    { "Horses", HorsesRefView.Model.RefreshDataCommand },
                    { "PaymentTypes", PaymentTypesRefView.Model.RefreshDataCommand },
                    { "Services", ServicesRefView.Model.RefreshDataCommand },                                        
                    { "Users", UsersRefView.Model.RefreshDataCommand },
                    //{ "UserRoles", UserRolesRefView.Model.RefreshDataCommand },
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
