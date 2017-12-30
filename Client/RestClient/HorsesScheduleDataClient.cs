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
    public class HorsesScheduleDataClient : BaseRestApiClient<HorseScheduleDataDto>, IHorseScheduleDataController
    {
        public HorsesScheduleDataClient(): base(enKnownControllers.HorsesScheduleDataController) { }

        public async Task<IList<HorseScheduleDataDto>> GetByHorse(long horseId)
        {
            return await SendRequest<List<HorseScheduleDataDto>>($"api/{ControllerName}/GetByHorse/{horseId}");
        }
    }
}
