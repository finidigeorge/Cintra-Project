using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.DtoMapping;
using RestClient;
using Shared.Dto;

namespace Client.ViewModels
{
    public class SchedulesDataRefVm : BaseReferenceVm<ScheduleDataDto, ScheduleDataDtoUi>
    {
        public SchedulesDataRefVm()
        {
            Client = RestClientFactory.GetClient<ScheduleDataDto>();            
        }

        protected override void BeforeAddItemHandler(ScheduleDataDtoUi item)
        {
            Items.Add(item);
        }
    }
}
