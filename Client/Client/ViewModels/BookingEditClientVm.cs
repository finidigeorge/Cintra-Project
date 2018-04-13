using Client.Commands;
using Common.DtoMapping;
using RestClient;
using Shared.Dto;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class BookingEditClientVm : BaseVm
    {
        public Guid Id { get; private set; } = new Guid();
        private IBaseController<ClientDto> clientsClient = RestClientFactory.GetClient<ClientDto>();
        private readonly BookingEditWindowVm _parentVm;
        public ClientsRefVm ClientsModel { get; set; } = new ClientsRefVm();

        public ClientDtoUi Client { get; set; } = new ClientDtoUi();

        public IAsyncCommand GetClientsCommand { get => ClientsModel.RefreshDataCommand; }
        public ICommand AddClientCommand { get => _parentVm.AddClientCommand; }
        public ICommand DeleteClientCommand { get; set; }

        public BookingEditClientVm(BookingEditWindowVm parentVm)
        {
            _parentVm = parentVm;

            ClientsModel.OnSelectedItemChanged += async (sender, client) =>
            {
                Client = client;                
            };

            DeleteClientCommand = new AsyncCommand<object>(async (param) =>
            {
                await _parentVm.DeleteClientCommand.ExecuteAsync(Id);
            }, (x) => _parentVm.CanDeleteClient);
        }
    }
}
