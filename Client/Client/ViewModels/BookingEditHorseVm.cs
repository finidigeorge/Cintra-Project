using Client.Commands;
using Common;
using Common.DtoMapping;
using RestClient;
using Shared.Dto;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Client.ViewModels
{
    public class BookingEditHorseVm : BaseVm
    {
        public Guid Id { get; private set; } = new Guid();
        private HorsesClient horsesClient = new HorsesClient();
        private readonly BookingEditWindowVm _parentVm;
        public HorsesRefVm HorsesModel { get; set; } = new HorsesRefVm();

        public HorseDtoUi Horse { get; set; } = new HorseDtoUi();
        
        public IAsyncCommand GetHorsesCommand { get => HorsesModel.RefreshDataCommand; }
        public ICommand AddHorseCommand { get => _parentVm.AddHorseCommand; }
        public ICommand DeleteHorseCommand { get; set; }

        public BookingEditHorseVm(BookingEditWindowVm parentVm)
        {
            _parentVm = parentVm;

            HorsesModel.OnSelectedItemChanged += async (sender, horse) =>
            {
                Horse = horse;
                await _parentVm.RunHorseValidations();
            };

            HorsesModel.GetItemsCommand = new AsyncCommand<object>(async (x) =>
            {
                long selectedItemId = 0;
                if (HorsesModel.SelectedItem != null)
                    selectedItemId = HorsesModel.SelectedItem.Id;

                if (HorsesModel.Items == null)
                    HorsesModel.Items = new ObservableCollection<HorseDtoUi>();
                else
                    HorsesModel.Items.Clear();

                if (_parentVm.BookingData.Service != null)
                    foreach (var item in (await horsesClient.GetAllByService(_parentVm.BookingData.Service.Id)).ToList<HorseDto, HorseDtoUi>())
                        HorsesModel.Items.Add(item);

                if (selectedItemId != 0)
                {
                    HorsesModel.SelectedItem = HorsesModel.Items.FirstOrDefault(i => i.Id == selectedItemId);
                }
            });

            DeleteHorseCommand = new AsyncCommand<object>(async (param) =>
            {
                await _parentVm.DeleteHorseCommand.ExecuteAsync(Id);
            }, (x) => _parentVm.CanDeleteHorse);
            
        }
    }
}
