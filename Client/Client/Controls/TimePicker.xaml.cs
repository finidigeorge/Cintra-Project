using Bindables;
using Client.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Client.Controls
{
    /// <summary>
    /// Interaction logic for ucDateTimeUpDown.xaml
    /// adopted from http://helpyourselfhere.blogspot.com.au/2015/03/wpf-simple-timepicker-user-control-for.html
    /// </summary>    
    public partial class TimePicker : UserControl, INotifyPropertyChanged
    {
        public TimePicker()
        {
            InitializeComponent();
            Model.AmPmTypes.Add("AM");
            Model.AmPmTypes.Add("PM");            
        }

        public TimePickerVm Model => (TimePickerVm) Resources["ViewModel"];        

        #region Methods

        private void MinutesUpButton_OnClick(object sender, RoutedEventArgs e)
        {
            Model.CurrentTime = Model.CurrentTime.AddMinutes(Model.MinsInterval);            
        }

        private void MinutesDownButton_OnClick(object sender, RoutedEventArgs e)
        {
            Model.CurrentTime = Model.CurrentTime.AddMinutes(-Model.MinsInterval);            
        }

        private void HourUpButton_OnClick(object sender, RoutedEventArgs e)
        {
            Model.CurrentTime = Model.CurrentTime.AddHours(Model.HoursInterval);            
        }

        private void HourDownButton_OnClick(object sender, RoutedEventArgs e)
        {
            Model.CurrentTime = Model.CurrentTime.AddHours(-Model.HoursInterval);            
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox) sender).Text.Any(x => !char.IsDigit(x)))
            {
                Dispatcher.BeginInvoke(new Action(() => ((TextBox) sender).Undo()));
            }
        }

        private void AddHoursTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemPlus || e.Key == Key.Add)
            {
                Model.CurrentTime = Model.CurrentTime.AddHours(Model.HoursInterval);
            }
            if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
            {
                Model.CurrentTime = Model.CurrentTime.AddHours(-Model.HoursInterval);
            }
        }

        private void AddMinutesTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemPlus || e.Key == Key.Add)
            {
                Model.CurrentTime = Model.CurrentTime.AddMinutes(Model.MinsInterval);
            }
            if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
            {
                Model.CurrentTime = Model.CurrentTime.AddMinutes(-Model.MinsInterval);
            }
        }
    }
}
