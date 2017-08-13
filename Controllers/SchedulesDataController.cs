using System;
using System.Collections.Generic;
using System.Text;
using DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Shared.Dto;

namespace Controllers
{
    [Authorize]
    [Route("/api/[controller]/values")]
    public class SchedulesDataController : BaseController<SchedulesData, ScheduleDataDto>
    {
        public SchedulesDataController(IGenericRepository<SchedulesData> repository, ILoggerFactory loggerFactory) : base(repository, loggerFactory)
        {
        }
    }
}
