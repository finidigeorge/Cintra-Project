using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using Shared.Extentions;

namespace Client.ViewModels.Reports
{
    public class HorsesWorkloadReportVm: BaseReportVm
    {
        private Dictionary<int, string> ReportIntervals = new Dictionary<int, string>() { { 0, "Daily" }, { 1, "Weekly" }, { 2, "Monthly" } };
        public override string ReportName => "HorsesWorkload" + ReportIntervals[ReportInterval];

        public DateTime StartDate { get; set; } = DateTime.Now.TruncateToDayStart();
        public DateTime EndDate { get; set; } = DateTime.Now.TruncateToDayStart();

        public int ReportInterval { get; set; }

        public override IReadOnlyCollection<(string Name, string Value)> GetReportParameters()
        {
            return new List<(string Name, string Value)>()
            {                                
                (Name: "StartDate", Value: StartDate.ToString("dd/MM/yyyy")),
                (Name: "EndDate", Value: EndDate.ToString("dd/MM/yyyy")),
            };
        }

        protected override string RunValidation()
        {
            if (StartDate > EndDate)
            {
                return "Selected date interval is incorrect";
            }

            return base.RunValidation();
        }

        [DependsOn(nameof(StartDate), nameof(EndDate))]
        public override string Error => RunValidation();

        [DependsOn(nameof(Error))]
        public bool IsValid => String.IsNullOrEmpty(Error);

    }
}
