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
    public class HorsesController: BaseController<Hors, HorseDto>
    {
        public HorsesController(IGenericRepository<Hors> repository, ILoggerFactory loggerFactory) : base(repository, loggerFactory)
        {
        }
    }
}
