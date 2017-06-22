using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client.Commands;
using Client.ViewModels.Interfaces;
using RestClient;
using Shared.Dto;

namespace Client.ViewModels
{
    public class ServicesRefVm : BaseReferenceVm<ServiceDto>
    {        
        public ServicesRefVm()
        {
            Client = new ServicesClient();
        }
    }

}
