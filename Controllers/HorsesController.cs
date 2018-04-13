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
using Shared.Dto;
using Shared.Interfaces;

namespace Controllers
{
    [Authorize]
    [Route("/api/[controller]/values")]
    public class HorsesController: BaseController<Hors, HorseDto>, IHorseController
    {
        public HorsesController(IGenericRepository<Hors> repository, ILoggerFactory loggerFactory) : base(repository, loggerFactory)
        {

        }

        [HttpGet("/api/[controller]/GetAllByService/{serviceId}")]
        public async Task<List<HorseDto>> GetAllByService(long serviceId)
        {
            return await ((HorsesRepository)_repository).RunWithinTransaction(async (db) =>
            {
                var result = await _repository.GetByParams(x => x.ServiceToHorsesLinks.Any(l => l.ServiceId == serviceId), db);                
                return result.Select(x => ObjectMapper.Map<HorseDto>(x)).ToList(); 
            });
        }
    }
}
