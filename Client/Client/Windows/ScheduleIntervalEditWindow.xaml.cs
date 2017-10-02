﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Client.ViewModels;
using RestApi;
using RestClient;

namespace Client
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class SchedulerIntervalEditWindow : Window
    {
        public SchedulerIntervalEditWindow()
        {
            InitializeComponent();                       
        }

        public ScheduleIntervalEditWindowVm Model => (ScheduleIntervalEditWindowVm)Resources["ViewModel"];

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Model.ErrorMessage = String.Empty;                       
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
