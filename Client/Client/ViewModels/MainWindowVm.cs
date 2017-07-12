﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Client.Commands;
using Client.ViewModels.Interfaces;
using Common;
using RestApi;
using RestClient;
using WPFCustomMessageBox;

namespace Client.ViewModels
{
    public class MainWindowVm
    {
        public ICommand ShowLoginDialogCommand { get; }
        public ICommand ShowChangePasswordDialogCommand { get; }
        public ICommand ShowExitDialogCommand { get; }

        public AuthVm AuthVm { get; }
        public Dictionary<string, IEditableSelectableReference<object>> TabsDictionary = new Dictionary<string, IEditableSelectableReference<object>>();

        public MainWindowVm()
        {
            ShowLoginDialogCommand = new Command(() =>
            {
                new LoginWindow() {DataContext = AuthVm}.ShowDialog(); 
                
            }, true);

            ShowChangePasswordDialogCommand = new Command(() =>
                {
                    new ChangePasswordWindow() {DataContext = new ChangePasswordVm()}.ShowDialog();
                }, true);

            ShowExitDialogCommand = new Command(ExitAppCommandAction, true);            


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
