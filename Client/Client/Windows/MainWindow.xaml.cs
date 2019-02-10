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

        private BookingWindow bookingWindow;

        public MainWindow()
        {
            InitializeComponent();

            Model.ShowBookingWindowCommand = new Command<object>((param) =>
            {                
                bookingWindow = new BookingWindow();
                bookingWindowIsRunning = true;
                bookingWindow.Closed += (sender2, e2) => { bookingWindowIsRunning = false; };
                bookingWindow.Show();                
                           
            }, x => Model.AuthVm.IsAuthenticated && !bookingWindowIsRunning);

            Model.ShowLoginDialogCommand = new Command<object>((param) =>
            {
                new LoginWindow() { DataContext = Model.AuthVm, Owner = this }.ShowDialog();

                if (Model.AuthVm.IsAuthenticated)
                {
                    HorsesRefView.Model.RefreshDataCommand.Execute(null);
                    Model.ShowBookingWindowCommand.Execute(null);
                }

            }, x => true);

            Model.ShowChangePasswordDialogCommand = new Command<object>((param) =>
            {
                new ChangePasswordWindow() { DataContext = new ChangePasswordVm(), Owner = this }.ShowDialog();
            }, x => true);

            Model.ShowExitDialogCommand = new Command<object>((param) => ExitAppCommandAction(), x => true);

            Model.RunClientHistoryReportCommand = new Command<object>((param) =>
            {
                new ClientLessonsReportWindow() { Owner = this }.ShowDialog();
            }, x => Model.AuthVm.IsAuthenticated && Model.AuthVm.IsAdmin);

            Model.RunHorsesWorkloadReportCommand = new Command<object>((param) =>
            {
                new HorsesWorkloadReportWindow() { Owner = this }.ShowDialog();
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
                try
                {
                    if (bookingWindow != null)
                        bookingWindow.Dispatcher.BeginInvoke((Action)bookingWindow.Close);
                }
                catch (Exception)
                {                    
                }
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ExitAppCommandAction();

            e.Cancel = true;
        }
    }
}
