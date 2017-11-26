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
        public BookingRefVm()
        {
            Client = RestClientFactory.GetClient<BookingDto>();
        }

        //UI Event wrappers commands
        public ICommand NextDayCommand { get; set; }
        public ICommand PrevDayCommand { get; set; }
        public ICommand AddDailyScheduledIntervalCommand { get; set; }
        public ICommand UpdateDailyScheduledIntervalCommand { get; set; }
        public ICommand DeleteDailyScheduledIntervalCommand { get; set; }
    }
}
