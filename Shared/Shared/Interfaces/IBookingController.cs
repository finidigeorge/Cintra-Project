using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces
{
    public interface IBookingController
    {
        Task<List<BookingDto>> GetAllFiltered(long beginDate, long endDate);
    }
}
