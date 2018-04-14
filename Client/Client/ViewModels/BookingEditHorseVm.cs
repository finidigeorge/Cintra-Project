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
        public HorsesRefVm Model { get; set; } = new HorsesRefVm();

        public IAsyncCommand GetHorsesCommand { get => Model.RefreshDataCommand; }
        public ICommand AddHorseCommand { get => _parentVm.AddHorseCommand; }
        public ICommand DeleteHorseCommand { get; set; }

        public BookingEditHorseVm(BookingEditWindowVm parentVm, HorseDtoUi horseDto)
        {
            _parentVm = parentVm;
            Model.RefreshDataCommand.Execute(null);
            Model.SelectedItem = horseDto;

            Model.OnSelectedItemChanged += async (sender, horse) =>
            {                
                _parentVm.SyncHorsesCommand.Execute(null);
                await _parentVm.RunHorseValidations();
            };

            Model.GetItemsCommand = new AsyncCommand<object>(async (x) =>
            {
                if (_parentVm?.BookingData?.Service == null)
                {
                    Model.Items?.Clear();
                    return;
                }

                long selectedItemId = 0;
                if (Model.SelectedItem != null)
                    selectedItemId = Model.SelectedItem.Id;

                if (Model.Items == null)
                    Model.Items = new ObservableCollection<HorseDtoUi>();
                else
                    Model.Items.Clear();

                if (_parentVm.BookingData.Service != null)
                    foreach (var item in (await horsesClient.GetAllByService(_parentVm.BookingData.Service.Id)).ToList<HorseDto, HorseDtoUi>())
                        Model.Items.Add(item);

                if (selectedItemId != 0)
                {
                    Model.SelectedItem = Model.Items.FirstOrDefault(i => i.Id == selectedItemId);
                }
            });

            DeleteHorseCommand = new AsyncCommand<object>(async (param) =>
            {
                await _parentVm.DeleteHorseCommand.ExecuteAsync(Id);
            }, (x) => _parentVm.CanDeleteHorse);
        }
    }
}
