using DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;

namespace Controllers
{
    [Authorize]
    [Route("/api/[controller]/values")]
    public class PaymentTypesController : BaseController<PaymentTypes, PaymentTypeDto>
    {
        public PaymentTypesController(IGenericRepository<PaymentTypes> repository, ILoggerFactory loggerFactory) : base(repository, loggerFactory)
        {
        }
    }
}
