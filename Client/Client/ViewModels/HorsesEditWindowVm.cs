using Client.Commands;
using Common;
using Common.DtoMapping;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class HorsesEditWindowVm: BaseVm
    {
        public IAsyncCommand AddUnavalabilityInterval { get; set; }
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
            HorsesScheduleModel.GetItemsCommand = new AsyncCommand<object>(async (x) => 
                {
                    long selectedItemId = 0;
                    if (HorsesScheduleModel.SelectedItem != null)
                        selectedItemId = HorsesScheduleModel.SelectedItem.Id;

                    if (HorsesScheduleModel.Items == null)
                        HorsesScheduleModel.Items = new ObservableCollection<HorseScheduleDataDtoUi>();
                    else
                        HorsesScheduleModel.Items.Clear();

                    foreach (var item in (await HorsesScheduleModel._client.GetByHorse(HorseData.Id)).ToList<HorseScheduleDataDto, HorseScheduleDataDtoUi>())
                        HorsesScheduleModel.Items.Add(item);

                    if (selectedItemId != 0)
                    {
                        HorsesScheduleModel.SelectedItem = HorsesScheduleModel.Items.FirstOrDefault(i => i.Id == selectedItemId);
                    }
                    ;
                }, (x) => HorsesScheduleModel.HasValidUser()
            );
        }

        private async void RefreshAllModels()
        {
            await HorsesScheduleModel.RefreshDataCommand.ExecuteAsync(null);            
        }
    }
}

