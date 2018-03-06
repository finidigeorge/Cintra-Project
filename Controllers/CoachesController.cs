using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories;
using Repositories.Interfaces;
using Shared;
using Shared.Dto;
using Shared.Interfaces;

namespace Controllers
{
    [Authorize]
    [Route("/api/[controller]/values")]
    public class CoachesController: BaseController<Coach, CoachDto>, ICoachController
    {
        public CoachesController(IGenericRepository<Coach> repository, ILoggerFactory loggerFactory) : base(repository, loggerFactory)
        {            
        }

        [HttpGet("/api/[controller]/GetAllByService/{serviceId}/{onlyAssignedCoaches}")]
        public async Task<List<CoachDto>> GetAllByService(long serviceId, bool onlyAssignedCoaches)
        {
            return await((CoachesRepository)_repository).RunWithinTransaction(async (db) =>
            {
                IEnumerable<Coach> result = null;
                if (onlyAssignedCoaches)
                    result = await _repository.GetByParams(x => x.ServiceToCoachesLinks.Any(l => l.ServiceId == serviceId), db);
                else
                    result = (await _repository.GetByParams(x => x.ServiceToCoachesLinks.Any(l => l.ServiceId == serviceId), db))
                        .Union(await _repository.GetByParams(x => x.CoachRoleId == (long)CoachRolesEnum.Coach, db));

                return result.Select(x => ObjectMapper.Map<CoachDto>(x)).ToList(); ;
            });
        }
    }
}
