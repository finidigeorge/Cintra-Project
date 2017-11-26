using Shared;
using Shared.Dto;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClient
{
    public class BookingsClient : BaseRestApiClient<BookingDto>, IBookingController
    {
        public BookingsClient() : base(enKnownControllers.BookingsController)
        {
        }


        public async Task<List<BookingDto>> GetAllFiltered(long beginDate, long endDate)
        {
            return await SendRequest<List<BookingDto>>($"api/{ControllerName}/GetAllFiltered/{beginDate}/{endDate}");
        }        
    }
}
