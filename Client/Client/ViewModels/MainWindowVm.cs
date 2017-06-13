﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Client.Commands;
using RestApi;
using RestClient;

namespace Client.ViewModels
{
    public class MainWindowVm
    {
        public ICommand ShowLoginDialogCommand { get; }
        public ICommand ShowExitDialogCommand { get; }

        public MainWindowVm()
        {
            ShowLoginDialogCommand = new Command(LoginCommandAction, true);
            ShowExitDialogCommand = new Command(ExitAppCommandAction, true);
        }

        private void LoginCommandAction()
        {
            var loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
        }

        private void ExitAppCommandAction()
        {
            MessageBoxResult result = MessageBox.Show("Do you want to exit?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}