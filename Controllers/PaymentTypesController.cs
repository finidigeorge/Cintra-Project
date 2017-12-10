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
    public class PaymentTypesController : BaseController<PaymentType, PaymentTypeDto>
    {
        public PaymentTypesController(IGenericRepository<PaymentType> repository, ILoggerFactory loggerFactory) : base(repository, loggerFactory)
        {
        }
    }
}
