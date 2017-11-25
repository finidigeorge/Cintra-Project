using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Shared.Dto;
using Shared.Interfaces;
using Mapping;
using System.Linq;

namespace Controllers
{
    [Authorize]
    [Route("/api/[controller]/values")]
    public class SchedulesDataController : BaseController<SchedulesData, ScheduleDataDto>, IScheduleDataController
    {
        public SchedulesDataController(IGenericRepository<SchedulesData> repository, ILoggerFactory loggerFactory) : base(repository, loggerFactory)
        {
        }

        [HttpGet("/api/[controller]/getBySchedule/{scheduleId}")]
        public async Task<IList<ScheduleDataDto>> GetBySchedule(long scheduleId)
        {
            return (await _repository.GetByParams(x => x.ScheduleId == scheduleId))
                .Select(ObjectMapper.Map<ScheduleDataDto>).ToList();
        }
    }
}
