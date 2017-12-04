﻿using Client.Extentions;
using Client.ViewModels;
using Common.DtoMapping;
using Common.Extentions;
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
using System.Windows.Shapes;

namespace Client.Windows
{
    /// <summary>
    /// Interaction logic for BookingEditWindow.xaml
    /// </summary>
    public partial class BookingEditWindow : Window
    {
        public BookingEditWindowVm Model => (BookingEditWindowVm)Resources["ViewModel"];

        public BookingEditWindow()
        {
            InitializeComponent();            
        }

        public BookingEditWindow(DateTime beginTime, DateTime endTime, BookingDtoUi bookindData)
        {
            InitializeComponent();
            Model.BookingData = bookindData;
            BeginTimePicker.Model.CurrentTime = beginTime;
            EndTimePicker.Model.CurrentTime = endTime;

            BeginTimePicker.Model.PropertyChanged += (sender, args) => {
                if (args.PropertyName == "CurrentTime")
                {
                    Model.BookingData.BeginTime = Model.BookingData.DateOn + (((TimePickerVm)sender).CurrentTime - ((TimePickerVm)sender).CurrentTime.TruncateToDayStart()); 
                }
            };

            EndTimePicker.Model.PropertyChanged += (sender, args) => {
                if (args.PropertyName == "CurrentTime")
                {
                    Model.BookingData.EndTime = Model.BookingData.DateOn + (((TimePickerVm)sender).CurrentTime - ((TimePickerVm)sender).CurrentTime.TruncateToDayStart());
                }
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {            
            DialogResult = true;
            Close();
        }        
    }
}
