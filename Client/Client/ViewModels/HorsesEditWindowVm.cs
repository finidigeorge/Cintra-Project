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
        public ObservableCollection<SelectableItem> Coaches { get; set; } = new ObservableCollection<SelectableItem>();

        public IAsyncCommand AddUnavalabilityInterval { get; set; }
        public ICommand DeleteUnavalabilityInterval { get; set; }

        public HorseSchedulesRefVm HorsesScheduleModel { get; set; } = new HorseSchedulesRefVm();

        bool _isAllCoachesAssigned;
        
        public bool IsAllCoachesAssigned
        {
            get { return _isAllCoachesAssigned; }
            set {
                _isAllCoachesAssigned = value;
                if (value)
                {
                    HorseData?.AllowedCoaches?.Clear();
                    foreach (var c in Coaches) c.IsSelected = false;
                }                
            }
        }

        public bool IsAssignedCoachesListEnabled => !IsAllCoachesAssigned;

        private HorseDtoUi _horseData;
        public HorseDtoUi HorseData
        {
            get => _horseData;
            set
            {
                Set(ref _horseData, value, nameof(HorseData));
                RefreshAllModels();
                IsAllCoachesAssigned = !HorseData?.AllowedCoaches?.Any() ?? true;                
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
        private CoachesRefVm CoachesModel { get; set; } = new CoachesRefVm();

        private async void RefreshAllModels()
        {
            await HorsesScheduleModel.RefreshDataCommand.ExecuteAsync(null);

            await CoachesModel.RefreshDataCommand.ExecuteAsync(null);
            Coaches.Clear();
            foreach (var h in CoachesModel.Items)
                Coaches.Add(new SelectableItem() { Id = h.Id, Name = h.Name, IsSelected = _horseData.AllowedCoaches.Any(x => x.Id == h.Id) });
        }
        public List<CoachDtoUi> GetSelectedCoached()
        {
            var selectedCoaches = new HashSet<long>((Coaches.Where(x => x.IsSelected).Select(x => x.Id)));
            return CoachesModel.Items.Where(x => selectedCoaches.Contains(x.Id)).ToList();
        }
    }
}

