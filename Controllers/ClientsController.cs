using DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controllers
{
    [Authorize]
    [Route("/api/[controller]/values")]
    public class ClientsController : BaseController<Client, ClientDto>
    {
        public ClientsController(IGenericRepository<Client> repository, ILoggerFactory loggerFactory) : base(repository, loggerFactory)
        {
        }
    }
}
