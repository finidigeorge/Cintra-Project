using Client.Extentions;
using Common.DtoMapping;
using RestClient;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class BookingRefVm : BaseReferenceVm<BookingDto, BookingDtoUi>
    {
        public int NumDaysToLoad { get; set; } = 1;

        public DateTime CurrentDate { get; set; } = DateTime.Now.TruncateToDayStart();

        private BookingsClient _client { get => (BookingsClient)Client; }

        public BookingRefVm()
        {
            Client = new BookingsClient();            
        }

        protected override async Task<IList<BookingDto>> GetItems()
        {
            return await _client.GetAllFiltered(CurrentDate.ToBinary(), CurrentDate.AddDays(NumDaysToLoad).ToBinary());
        }

        //UI Event wrappers commands
        public ICommand NextDayCommand { get; set; }
        public ICommand PrevDayCommand { get; set; }
        public ICommand AddDailyScheduledIntervalCommand { get; set; }
        public ICommand UpdateDailyScheduledIntervalCommand { get; set; }
        public ICommand DeleteDailyScheduledIntervalCommand { get; set; }
    }
}
