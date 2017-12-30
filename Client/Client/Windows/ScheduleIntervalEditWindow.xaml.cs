using System;
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
using Common.DtoMapping;
using RestApi;
using RestClient;
using System.ComponentModel;
using Shared;

namespace Client
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class SchedulerIntervalEditWindow : Window
    {
        public ScheduleDataDtoUi Model => (ScheduleDataDtoUi)DataContext;

        private ScheduleIntervalEnum mode = ScheduleIntervalEnum.Weekly;

        public bool IsDatePickerVisible => mode == ScheduleIntervalEnum.Daily;
        public bool IsWeekDropdownVisible => mode == ScheduleIntervalEnum.Weekly;

        public SchedulerIntervalEditWindow()
        {
            InitializeComponent();            
        }

        public SchedulerIntervalEditWindow(DateTime beginTime, DateTime endTime, ScheduleIntervalEnum mode)
        {
            InitializeComponent();
            BeginTimePicker.Model.CurrentTime = beginTime;
            EndTimePicker.Model.CurrentTime = endTime;

            BeginTimePicker.Model.PropertyChanged += (sender, args) => {
                if (args.PropertyName == "CurrentTime")
                {
                    Model.BeginTime = ((TimePickerVm)sender).CurrentTime;
                }
            };

            EndTimePicker.Model.PropertyChanged += (sender, args) => {
                if (args.PropertyName == "CurrentTime")
                {
                    Model.EndTime = ((TimePickerVm)sender).CurrentTime;
                }
            };

            this.mode = mode;
            SetVisibility();
        }

        private void SetVisibility()
        {
            if (!IsDatePickerVisible)
            {
                datePickerLabel.Visibility = Visibility.Hidden;
                datePicker.Visibility = Visibility.Hidden;
            }

            if (!IsWeekDropdownVisible)
            {
                dayPickerLabel.Visibility = Visibility.Hidden;
                dayPicker.Visibility = Visibility.Hidden;
            }
        }        

        private void Button_Click(object sender, RoutedEventArgs e)
        {            
            DialogResult = true;
            Close();
        }
    }
}
