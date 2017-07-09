using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using Shared.Dto;

namespace RestClient
{
    public class ServicesClient: BaseRestApiClient<ServiceDto>
    {
        public ServicesClient() : base(enKnownControllers.ServicesController)
        {
        }
    }
}
