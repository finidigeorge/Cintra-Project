using DataModels;
using LinqToDB.Data;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IBookingRepository: IGenericRepository<Booking>
    {
        Task<CheckResultDto> HasCoachNotOverlappedBooking(Coach coach, Booking bookingData, DataConnection dbContext = null);
        Task<CheckResultDto> HasCoachScheduleFitBooking(Coach coach, Booking bookingData, DataConnection dbContext = null);
        Task<CheckResultDto> HasCoachScheduleFitBreaks(Coach coach, Booking bookingData, DataConnection dbContext = null);

        Task<CheckResultDto> HasHorseNotOverlappedBooking(Hors horse, Booking bookingData, DataConnection dbContext = null);
        Task<CheckResultDto> HasHorseRequiredBreak(Hors horse, Booking bookingData, DataConnection dbContext = null);
        Task<CheckResultDto> HasHorseWorkedLessThanAllowed(Hors horse, Booking bookingData, DataConnection dbContext = null);
        Task<CheckResultDto> HasHorseScheduleFitBooking(Hors horse, Booking bookingData, DataConnection dbContext = null);
        Task<CheckResultDto> HasHorseAssignedToAtLeastOneOfCoaches(Hors horse, Booking bookingData, DataConnection dbContext = null);

        Task<String> RunValidations(Booking booking, bool isErrors = true, DataConnection dbContext = null);        
    }
}
