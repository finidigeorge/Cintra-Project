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

        Task<CheckResultDto> HasCoachNotOverlappedBooking(BookingDto entity);

        Task<CheckResultDto> HasHorseNotOverlappedBooking(BookingDto entity);

        Task<CheckResultDto> HasHorseRequiredBreak(BookingDto entity);

        Task<CheckResultDto> HasHorseWorkedLessThanAllowed(BookingDto entity);

        Task<CheckResultDto> HasHorseScheduleFitBooking(BookingDto entity);
        Task<CheckResultDto> HasCoachScheduleFitBooking(BookingDto entity);

        Task InsertAll(List<BookingDto> entityList);
    }
}
