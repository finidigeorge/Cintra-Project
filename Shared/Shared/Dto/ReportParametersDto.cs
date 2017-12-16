using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dto
{
    public class ReportParametersDto
    {
        public string ReportName { get; set; }
        public Dictionary<string, dynamic> Parameters { get; set; }
    }
}
