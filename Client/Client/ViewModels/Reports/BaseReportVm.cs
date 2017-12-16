using Client.Commands;
using Client.Extentions;
using Client.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.ViewModels.Reports
{
    public class BaseReportVm : BaseVm, IReportVm
    {
        public ICommand RunReportCommand { get; set; } 
        public BaseReportVm()
        {
            RunReportCommand = new Command<object>(() =>
            {
                RunReport();
            }, x => true);

        }

        public virtual string ReportName => throw new NotImplementedException();

        public virtual IReadOnlyCollection<(string Name, string Value)> GetReportParameters()
        {
            throw new NotImplementedException();
        }

        public virtual void RunReport()
        {
            ReportRunner.RunReport(this);
        }
    }
}
