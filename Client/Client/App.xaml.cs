using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Client.Security;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var principal = new UserPrincipal();
            AppDomain.CurrentDomain.SetThreadPrincipal(principal);

            base.OnStartup(e);
        }
    }
}
