using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Client.Commands;
using Common.DtoMapping;
using RestClient;
using Shared.Dto;

namespace Client.ViewModels
{
    public class SchedulesRefVm : BaseReferenceVm<ScheduleDto, ScheduleDtoUi>
    {
        //UI Event wrappers commands
        public ICommand NextDayCommand { get; set; }
        public ICommand PrevDayCommand { get; set; }
        public ICommand AddScheduledIntervalCommand { get; set; }
        public ICommand DeleteScheduledIntervalCommand { get; set; }

        public IList<ScheduleDto> DataSource { get; set; } = new List<ScheduleDto>();
        public CoachDtoUi Coach { get; set; }

        protected override async Task<IList<ScheduleDto>> GetItems()
        {
            return await Task.FromResult(DataSource);
        }

        protected override void BeforeAddItemHandler(ScheduleDtoUi item)
        {
            item.CoachId = Coach.Id;
        }

        public SchedulesRefVm()
        {
            Client = RestClientFactory.GetClient<ScheduleDto>();            
        }
    }
}
