using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Interfaces
{
    public interface IReportController
    {
        string RunReport(ReportParametersDto parameters);        
    }
}
