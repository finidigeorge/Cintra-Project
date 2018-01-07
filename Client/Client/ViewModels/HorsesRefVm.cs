using Common.DtoMapping;
using RestClient;
using Shared.Dto;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class HorsesRefVm : BaseReferenceVm<HorseDto, HorseDtoUi>
    {
        public ICommand ShowAvalabilityEditorCommand { get; set; }

        public HorsesRefVm()
        {
            Client = RestClientFactory.GetClient<HorseDto>();            
        }        
    }

}
