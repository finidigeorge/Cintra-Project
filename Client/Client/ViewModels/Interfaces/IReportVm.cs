using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Interfaces
{
    public interface IReportVm
    {
        IReadOnlyCollection<(string Name, string Value)> GetReportParameters();
        void RunReport();
        string ReportName { get; }
    }
}
