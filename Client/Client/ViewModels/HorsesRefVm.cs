using Client.Commands;
using Client.ViewModels.Filter;
using Common.DtoMapping;
using PropertyChanged;
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
            Client = new HorsesClient();
            Filter = new HorseSearchFilter();
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
