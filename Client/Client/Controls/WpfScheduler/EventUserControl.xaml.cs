using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Client.Controls.WpfScheduler
{
    /// <summary>
    /// Interaction logic for EventUserControl.xaml
    /// </summary>
    public partial class EventUserControl : UserControl
    {
        Event _e;

        public EventUserControl(Event e, bool showTime)
        {
            InitializeComponent();

            _e = e;

            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.Subject = e.Subject;
            this.BorderElement.Background = e.Color;
            if (!showTime || e.AllDay)
            {
                this.DisplayDateText.Visibility = System.Windows.Visibility.Hidden;
                this.DisplayDateText.Height = 0;
                this.DisplayDateText.Text = String.Format("{0} - {1}", e.Start.ToShortDateString(), e.End.ToShortDateString());
            }
            else
            {
                this.DisplayDateText.Text = String.Format("{0} - {1}", e.Start.ToString("hh:mm tt"), e.End.ToString("hh:mm tt"));
            }
            this.BorderElement.ToolTip = this.DisplayDateText.Text + Environment.NewLine + this.DisplayText.Text;

            if (e.IsSelected)
            {
                this.BorderElement.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                this.BorderElement.BorderThickness = new Thickness() { Bottom = 2, Left = 2, Right = 2, Top = 2};
            }
        }

        public Event Event {
            get { return _e; }
        }

        #region Subject
        public static readonly DependencyProperty SubjectProperty = 
            DependencyProperty.Register("Subject", typeof(string), typeof(EventUserControl),
            new FrameworkPropertyMetadata(AdjustSubject));

        public string Subject
        {
            get { return (string)GetValue(SubjectProperty); }
            set { SetValue(SubjectProperty, value); }
        }

        private static void AdjustSubject(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            (source as EventUserControl).DisplayText.Text = (string)e.NewValue;
        }
        #endregion
    }


    
}
