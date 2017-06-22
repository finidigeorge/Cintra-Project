﻿using System;
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
    public class ServicesController: BaseController<Service, ServiceDto>
    {
        public ServicesController(IServicesRepository repository) : base(repository)
        {
        }
    }
}
