using Shared;
using Shared.Dto;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClient
{
    public class ScheduleDataDtoClient : BaseRestApiClient<ScheduleDataDto>, IScheduleDataController
    {
        public ScheduleDataDtoClient() : base(enKnownControllers.SchedulesDataController)
        {
        }


        public async Task<IList<ScheduleDataDto>> GetBySchedule(long scheduleId)
        {
            return await SendRequest<List<ScheduleDataDto>>($"api/{ControllerName}/getBySchedule/{scheduleId}");
        }
    }
}   
