using DataModels;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IBookingRepository: IGenericRepository<Booking>
    {
        Task<CheckResultDto> HasCoachNotOverlappedBooking(Coach coach, Booking bookingData, CintraDB dbContext = null);
        Task<CheckResultDto> HasCoachScheduleFitBooking(Coach coach, Booking bookingData, CintraDB dbContext = null);

        Task<CheckResultDto> HasHorseNotOverlappedBooking(Hors horse, Booking bookingData, CintraDB dbContext = null);
        Task<CheckResultDto> HasHorseRequiredBreak(Hors horse, Booking bookingData, CintraDB dbContext = null);
        Task<CheckResultDto> HasHorseWorkedLessThanAllowed(Hors horse, Booking bookingData, CintraDB dbContext = null);
        Task<CheckResultDto> HasHorseScheduleFitBooking(Hors horse, Booking bookingData, CintraDB dbContext = null);
    }
}
