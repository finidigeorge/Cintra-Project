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

        Task<CheckResultDto> HasCoachesNotOverlappedBooking(BookingDto entity);
        Task<CheckResultDto> HasCoachesScheduleFitBooking(BookingDto entity);

        Task<CheckResultDto> HasHorsesNotOverlappedBooking(BookingDto entity);

        Task<CheckResultDto> HasHorsesRequiredBreak(BookingDto entity);

        Task<CheckResultDto> HasHorsesWorkedLessThanAllowed(BookingDto entity);

        Task<CheckResultDto> HasHorsesScheduleFitBooking(BookingDto entity);

        Task CancelAllBookings(long metadataId, long FromDate);

        Task InsertAll(List<BookingDto> entityList);
    }
}
