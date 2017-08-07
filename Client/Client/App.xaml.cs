using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Client.Security;
using Serilog;

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

            Log.Logger = new LoggerConfiguration()                
                .WriteTo.RollingFile(Path.Combine(ConfigurationManager.AppSettings["LogFolder"], $"log-{DateTime.Now:yyyy-MM-dd}.txt"))
                .CreateLogger();

            base.OnStartup(e);
        }
    }
}
