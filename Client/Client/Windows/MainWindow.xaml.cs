﻿using System;
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
            var tc = sender as TabControl;
            if (tc != null)
            {
                var item = (TabItem)tc.SelectedItem;
                if (item.IsSelected && selectedTab != item.Name)
                {
                    selectedTab = ((TabItem)tc.SelectedItem).Name;

                    //TODO add proper events handling
                    if (CheckAndWarnAuth())
                    {                        
                        if (item.Name == "UserRoles")
                        {
                            await UserRolesRefView.Model.RefreshDataCommand.ExecuteAsync(null);                                                        
                        }

                        if (item.Name == "Users")
                        {
                            await UsersRefView.Model.RefreshDataCommand.ExecuteAsync(null);                            
                            
                        }

                        if (item.Name == "Horses")
                        {
                            await HorsesRefView.Model.RefreshDataCommand.ExecuteAsync(null);                                                        
                        }

                        if (item.Name == "Services")
                        {
                            await ServicesRefView.Model.RefreshDataCommand.ExecuteAsync(null);                                                        
                        }

                        if (item.Name == "Coaches")
                        {
                            await CoachesRefView.Model.RefreshDataCommand.ExecuteAsync(null);                                                        
                        }
                    }
                }
            }
        }
    }
}