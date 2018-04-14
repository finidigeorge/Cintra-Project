﻿using Client.Commands;
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
    public class BookingEditClientVm : BaseVm
    {
        public Guid Id { get; private set; } = new Guid();
        private IBaseController<ClientDto> clientsClient = RestClientFactory.GetClient<ClientDto>();
        private readonly BookingEditWindowVm _parentVm;
        public ClientsRefVm Model { get; set; } = new ClientsRefVm();

        public IAsyncCommand GetClientsCommand { get => Model.RefreshDataCommand; }
        public ICommand AddClientCommand { get => _parentVm.AddClientCommand; }
        public ICommand DeleteClientCommand { get; set; }

        public BookingEditClientVm(BookingEditWindowVm parentVm, ClientDtoUi clientDto)
        {
            _parentVm = parentVm;
            Model.RefreshDataCommand.Execute(null);
            Model.SelectedItem = clientDto;

            Model.OnSelectedItemChanged += async (sender, client) =>
            {                
                _parentVm.SyncClientsCommand.Execute(null);
                _parentVm.RunClientValidations();
            };

            DeleteClientCommand = new AsyncCommand<object>(async (param) =>
            {
                await _parentVm.DeleteClientCommand.ExecuteAsync(Id);
            }, (x) => _parentVm.CanDeleteClient);            
        }
    }
}
