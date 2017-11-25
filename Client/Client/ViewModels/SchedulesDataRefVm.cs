using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.DtoMapping;
using RestClient;
using Shared.Dto;
using Client.Commands;
using System.Collections.ObjectModel;
using Common;

namespace Client.ViewModels
{
    public class SchedulesDataRefVm : BaseReferenceVm<ScheduleDataDto, ScheduleDataDtoUi>
    {
        public SchedulesDataRefVm()
        {
            GetItemsCommand = new AsyncCommand<object>(async (x) =>
            {
                Items = new ObservableCollection<ScheduleDataDtoUi>();
                foreach (var item in (await GetItems((ScheduleDto)x)).ToList<ScheduleDataDto, ScheduleDataDtoUi>())
                    Items.Add(item);

                ItemsCollectionView?.Refresh();

            });

            Client = RestClientFactory.GetClient<ScheduleDataDto>();            
        }

        protected virtual async Task<IList<ScheduleDataDto>> GetItems(ScheduleDto schedule)
        {
            var result = await Client.GetAll();
            return result.Where(x => x.ScheduleId == schedule.Id).ToList();
        }

        protected override void BeforeAddItemHandler(ScheduleDataDtoUi item)
        {
            Items.Add(item);
        }
    }
}
