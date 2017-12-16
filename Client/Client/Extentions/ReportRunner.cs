using Client.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Client.Extentions
{
    public static class ReportRunner
    {
        public static readonly string viewerDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Reports");
        public static readonly string viewerPath = Path.Combine(viewerDir, "RdlReader.exe");
        public static void RunReport(IReportVm reportVm)        
        {
            var parametersJoin = String.Join("&", reportVm.GetReportParameters().Select(x => $"{x.Name}={x.Value}"));
            var runCmd = $"-r \"{Path.Combine(viewerDir, $"{reportVm.ReportName}.rdl")}\" -p \"{parametersJoin}\"";
            System.Diagnostics.Process.Start(viewerPath, runCmd);
        }
    }
}
