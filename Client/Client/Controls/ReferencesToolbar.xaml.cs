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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client.Commands;
using Bindables;

namespace Client.Controls
{
    /// <summary>
    /// Interaction logic for ReferencesToolbar.xaml
    /// Dependency properties implementation done by magic of Fody
    /// </summary>
    [DependencyProperty]
    public partial class ReferencesToolbar : UserControl
    {       
        public ReferencesToolbar()
        {
            InitializeComponent();
        }

        public ICommand NewCommand { get; set; }

        public ICommand DeleteCommand { get; set; }
    
        public object DeleteCommandParam { get; set; }

        public ICommand EditCommand { get; set; }

        public object EditCommandParam { get; set; }
        
    }
}
