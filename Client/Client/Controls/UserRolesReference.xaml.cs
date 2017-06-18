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

namespace Client.Controls
{
    /// <summary>
    /// Interaction logic for UserRolesReference.xaml
    /// </summary>
    public partial class UserRolesReference : UserControl
    {
        public UserRolesRefVm Model => (UserRolesRefVm)Resources["ViewModel"];

        public UserRolesReference()
        {
            InitializeComponent();            
        }

        public void OnActivated()
        {
            ItemsDataGrid.GrabFocus(Model.Items);
        }        
    }
}