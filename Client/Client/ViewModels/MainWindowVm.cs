using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Client.Commands;
using Client.ViewModels.Interfaces;
using Common;
using RestApi;
using RestClient;
using WPFCustomMessageBox;
using Client.Windows;
using System.Threading;
using Client.Windows.Reports;

namespace Client.ViewModels
{
    public class MainWindowVm: BaseVm
    {
        public ICommand ShowBookingWindowCommand { get; }
        public ICommand ShowLoginDialogCommand { get; }
        public ICommand ShowChangePasswordDialogCommand { get; }
        public ICommand ShowExitDialogCommand { get; }
        public ICommand RunClientHistoryReportCommand { get; }

        public AuthVm AuthVm { get; }
        public Dictionary<string, IEditableSelectableReference<object>> TabsDictionary = new Dictionary<string, IEditableSelectableReference<object>>();

        bool bookingWindowIsRunning = false;

        public MainWindowVm()
        {
            ShowBookingWindowCommand = new Command<object>(() =>
            {                
                var thread = new Thread(() =>
                {
                    var bookingWindow = new BookingWindow();
                    bookingWindowIsRunning = true;                    
                    bookingWindow.Show();

                    bookingWindow.Closed += (sender2, e2) => { bookingWindow.Dispatcher.InvokeShutdown(); bookingWindowIsRunning = false; };

                    System.Windows.Threading.Dispatcher.Run();
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();                                                

            }, x => AuthVm.IsAuthenticated && !bookingWindowIsRunning);

            ShowLoginDialogCommand = new Command<object>(() =>
            {
                new LoginWindow() {DataContext = AuthVm}.ShowDialog(); 
                
            }, x => true);

            ShowChangePasswordDialogCommand = new Command<object>(() =>
                {
                    new ChangePasswordWindow() {DataContext = new ChangePasswordVm()}.ShowDialog();
                }, x => true);

            ShowExitDialogCommand = new Command<object>(ExitAppCommandAction, x => true);

            RunClientHistoryReportCommand = new Command<object>(() =>
            {
                new ClientLessonsReportWindow().ShowDialog();
            }, x => AuthVm.IsAuthenticated && AuthVm.IsAdmin);


            AuthVm = new AuthVm(new AuthClient(), new UserRolesClient());
        }              

        private void ExitAppCommandAction()
        {
            MessageBoxResult result = CustomMessageBox.Show(Messages.EXIT_APP_MSG, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
