using Client.Commands;
using Common.DtoMapping;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class SelectableItem : BaseVm
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ServiceEditWindowVm : BaseVm
    {
        public ObservableCollection<SelectableItem> Coaches { get; set; } = new ObservableCollection<SelectableItem>();
        public ObservableCollection<SelectableItem> Horses { get; set; } = new ObservableCollection<SelectableItem>();

        public ServiceEditWindowVm()
        {
            SelectAllHorsesCommand = new Command<object>(() => { foreach (var c in Horses) c.IsSelected = true; });
            UnselectAllHorsesCommand = new Command<object>(() => { foreach (var c in Horses) c.IsSelected = false; });

            SelectAllCoachesCommand = new Command<object>(() => { foreach (var c in Coaches) c.IsSelected = true; });
            UnselectAllCoachesCommand = new Command<object>(() => { foreach (var c in Coaches) c.IsSelected = false; });
        }    

        private ServiceDtoUi _serviceData;
        public ServiceDtoUi ServiceData
        {
            get => _serviceData;
            set
            {
                Set(ref _serviceData, value, nameof(ServiceData));
                RefreshAllModels();
            }
        }

        private CoachesRefVm CoachesModel { get; set; } = new CoachesRefVm();
        private HorsesRefVm HorsesModel { get; set; } = new HorsesRefVm();

        public ICommand UnselectAllHorsesCommand { get; set; }
        public ICommand SelectAllHorsesCommand { get; set; }

        public ICommand UnselectAllCoachesCommand { get; set; }
        public ICommand SelectAllCoachesCommand { get; set; }

        private async Task RefreshAllModels()
        {
            await HorsesModel.RefreshDataCommand.ExecuteAsync(null);
            Horses.Clear();
            foreach (var h in HorsesModel.Items)
                Horses.Add(new SelectableItem() { Id = h.Id, Name = h.NickName, IsSelected = _serviceData.Horses.Any(x => x.Id == h.Id) });

            await CoachesModel.RefreshDataCommand.ExecuteAsync(null);
            Coaches.Clear();
            foreach (var h in CoachesModel.Items)
                Coaches.Add(new SelectableItem() { Id = h.Id, Name = h.Name, IsSelected = _serviceData.Coaches.Any(x => x.Id == h.Id) });

        }

        public List<CoachDtoUi> GetSelectedCoached()
        {
            var selectedCoaches = new HashSet<long>((Coaches.Where(x => x.IsSelected).Select(x => x.Id)));
            return CoachesModel.Items.Where(x => selectedCoaches.Contains(x.Id)).ToList();
        }

        public List<HorseDtoUi> GetSelectedHorses() {
            var selectedHorses = new HashSet<long>((Horses.Where(x => x.IsSelected).Select(x => x.Id)));
            return HorsesModel.Items.Where(x => selectedHorses.Contains(x.Id)).ToList();
        }
    }
}

