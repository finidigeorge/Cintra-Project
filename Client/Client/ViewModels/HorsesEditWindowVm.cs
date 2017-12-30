using Client.Commands;
using Common.DtoMapping;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class HorsesEditWindowVm: BaseVm
    {
        public ICommand AddUnavalabilityInterval { get; set; }
        public ICommand DeleteUnavalabilityInterval { get; set; }

        public HorseSchedulesRefVm HorsesScheduleModel { get; set; } = new HorseSchedulesRefVm();

        private HorseDtoUi _horseData;
        public HorseDtoUi HorseData
        {
            get => _horseData;
            set
            {
                Set(ref _horseData, value, nameof(HorseData));
                RefreshAllModels();
            }
        }

        public HorsesEditWindowVm()
        {
            HorsesScheduleModel.RefreshDataCommand = new AsyncCommand<object>(async (x) => 
                {
                    await HorsesScheduleModel._client.GetByHorse(_horseData.Id);
                }, x => true
            );
        }

        private async void RefreshAllModels()
        {
            await HorsesScheduleModel.RefreshDataCommand.ExecuteAsync(null);            
        }
    }
}

