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
    public class HorsesScheduleDataController : BaseController<HorsesScheduleData, HorseScheduleDataDto>
    {
        public HorsesScheduleDataController(IGenericRepository<HorsesScheduleData> repository, ILoggerFactory loggerFactory) : base(repository, loggerFactory)
        {
        }

        [HttpGet("/api/[controller]/getByHorse/{scheduleId}")]
        public async Task<IList<HorseScheduleDataDto>> GetByHorse(long horseId)
        {
            return (await _repository.GetByParams(x => x.HorseId == horseId))
                .Select(ObjectMapper.Map<HorseScheduleDataDto>).ToList();
        }
    }
}
