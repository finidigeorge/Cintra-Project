using Common.DtoMapping;
using RestClient;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    
    public class PaymentTypesRefVm : BaseReferenceVm<PaymentTypeDto, PaymentTypeDtoUi>
    {
        public PaymentTypesRefVm()
        {
            Client = RestClientFactory.GetClient<PaymentTypeDto>();
        }
    }    
}
