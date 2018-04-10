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

        Task<CheckResultDto> HasHorseNotOverlappedBooking(BookingDto entity);

        Task<CheckResultDto> HasHorseRequiredBreak(BookingDto entity);

        Task<CheckResultDto> HasHorseWorkedLessThanAllowed(BookingDto entity);

        Task<CheckResultDto> HasHorseScheduleFitBooking(BookingDto entity);

        Task CancelAllBookings(long metadataId, long FromDate);

        Task InsertAll(List<BookingDto> entityList);
    }
}
