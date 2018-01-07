using Client.ViewModels.Interfaces;
using Shared.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Reports
{
    public class ClientLessonReportVm: BaseReportVm
    {
        public ClientsRefVm ClientsModel { get; set; } = new ClientsRefVm();
        public override string ReportName { get; } = "ClientLessons";

        public DateTime StartDate { get; set; } = DateTime.Now.TruncateToDayStart();
        public DateTime EndDate { get; set; } = DateTime.Now.TruncateToDayStart();        

        public override IReadOnlyCollection<(string Name, string Value)> GetReportParameters()
        {
            return new List<(string Name, string Value)>()
            {
                (Name: "ClientName", Value: ClientsModel.SelectedItem?.Name),
                (Name: "ClientId", Value: ClientsModel.SelectedItem?.Id.ToString()),
                (Name: "StartDate", Value: StartDate.ToString("dd/MM/yyyy")),
                (Name: "EndDate", Value: EndDate.ToString("dd/MM/yyyy")),
            };
        }
    }
}
