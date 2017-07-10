using System;
using System.Collections.Generic;
using System.Text;
using DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Shared.Dto;

namespace Controllers
{
    [Authorize]
    [Route("/api/[controller]/values")]
    public class CoachesController: BaseController<Coach, CoachDto>
    {
        public CoachesController(ICoachesRepository repository) : base(repository)
        {
        }
    }
}
