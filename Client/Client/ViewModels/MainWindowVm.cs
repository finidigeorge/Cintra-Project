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
        public ICommand ShowBookingWindowCommand { get; set; }
        public ICommand ShowLoginDialogCommand { get; set; }
        public ICommand ShowChangePasswordDialogCommand { get; set; }
        public ICommand ShowExitDialogCommand { get; set; }
        public ICommand RunClientHistoryReportCommand { get; set; }

        public AuthVm AuthVm { get; }
        public Dictionary<string, IEditableSelectableReference<object>> TabsDictionary = new Dictionary<string, IEditableSelectableReference<object>>();        

        public MainWindowVm()
        {            
            AuthVm = new AuthVm(new AuthClient(), new UserRolesClient());
        }              

        
    }
}
