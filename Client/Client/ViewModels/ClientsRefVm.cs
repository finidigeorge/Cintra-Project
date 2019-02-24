using Client.ViewModels.Filter;
using Common.DtoMapping;
using RestClient;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public class ClientsRefVm: BaseReferenceVm<ClientDto, ClientDtoUi>
    {
        public ClientsRefVm()
        {
            Client = RestClientFactory.GetClient<ClientDto>();
            Filter = new ClientSearchFilter();
        }

        public string FilterExpression
        {
            get => Filter.SearchExpression;
            set
            {
                Filter.SearchExpression = value;
                ItemsCollectionView.Refresh();
            }
        }
    }
}
