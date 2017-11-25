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
        public ICommand AddDailyScheduledIntervalCommand { get; set; }
        public ICommand UpdateDailyScheduledIntervalCommand { get; set; }
        public ICommand DeleteDailyScheduledIntervalCommand { get; set; }

        public ICommand NextWeekCommand { get; set; }
        public ICommand PrevWeekCommand { get; set; }
        public ICommand AddWeeklyScheduledIntervalCommand { get; set; }
        public ICommand UpdateWeeklyScheduledIntervalCommand { get; set; }
        public ICommand DeleteWeeklyScheduledIntervalCommand { get; set; }

        public IList<ScheduleDto> DataSource { get; set; } = new List<ScheduleDto>();
        public CoachDtoUi Coach { get; set; }

        public SchedulesDataRefVm ScheduleDailyDataModel { get; set; } = new SchedulesDataRefVm();
        public SchedulesDataRefVm ScheduleWeeklyDataModel { get; set; } = new SchedulesDataRefVm();

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
