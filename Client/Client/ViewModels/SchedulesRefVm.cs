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
    public class SchedulesRefVm : BaseReferenceVm<ScheduleDto, ScheduleDtoUi>
    {
        public IList<ScheduleDto> DataSource { get; set; } = new List<ScheduleDto>();

        protected override async Task<IList<ScheduleDto>> GetItems()
        {
            return await Task.FromResult(DataSource);
        }

        public SchedulesRefVm()
        {
            Client = RestClientFactory.GetClient<ScheduleDto>();
        }
    }
}
