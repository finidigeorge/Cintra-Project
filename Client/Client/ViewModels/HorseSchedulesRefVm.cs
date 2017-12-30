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
    public class HorseSchedulesRefVm: BaseReferenceVm<HorseScheduleDataDto, HorseScheduleDataDtoUi>
    {
        public HorsesScheduleDataClient _client { get => (HorsesScheduleDataClient)Client; }

        public HorseSchedulesRefVm()
        {
            Client = new HorsesScheduleDataClient();
        }
    }
}
