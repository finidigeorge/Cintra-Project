using DataModels;
using LinqToDB;
using Repositories.Interfaces;
using Shared;
using Shared.Attributes;
using Shared.Dto;
using Shared.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using LinqToDB.Data;

namespace Repositories
{
    [PerScope]
    public class BookingRepository : GenericPreservableRepository<Booking>, IBookingRepository
    {
        public override async Task<long> Create(Booking entity, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                                
                if (entity.Id == 0)
                {
                    entity.Id = (long)(await db.InsertWithIdentityAsync(entity));
                        
                }
                else
                {
                        
                    await db.GetTable<BookingsToClientsLink>().DeleteAsync(x => x.BookingId == entity.Id);
                    await db.GetTable<BookingsToCoachesLink>().DeleteAsync(x => x.BookingId == entity.Id);
                    await db.GetTable<BookingsToHorsesLink>().DeleteAsync(x => x.BookingId == entity.Id);
                        
                    await db.UpdateAsync(entity);
                }


                foreach (var c in entity.BookingsToClientsLinks)
                {
                    c.BookingId = entity.Id;
                    await db.InsertWithIdentityAsync(c);
                }

                foreach (var c in entity.BookingsToCoachesLinks)
                {
                    c.BookingId = entity.Id;
                    await db.InsertWithIdentityAsync(c);
                }

                foreach (var c in entity.BookingsToHorsesLinks)
                {
                    c.BookingId = entity.Id;
                    await db.InsertWithIdentityAsync(c);
                }                                    

                return entity.Id;
            }, dbContext);
        }

        private List<Booking> AccessFilter(List<Booking> bookings)
        {            
            foreach (var b in bookings)
            {                
                b.BookingPayments = b.BookingPayments.Where(x => x.IsDeleted == false).ToList();
                b.Service = b.Service.IsDeleted ? null : b.Service;
                b.BookingsToHorsesLinks = b.BookingsToHorsesLinks.Where(x => !x.Hor.IsDeleted).ToList();
                b.BookingsToCoachesLinks = b.BookingsToCoachesLinks.Where(x => !x.Coach.IsDeleted).ToList();

                if (b.BookingsTemplateMetadata?.BookingTemplates != null)
                    b.BookingsTemplateMetadata.BookingTemplates = b.BookingsTemplateMetadata.BookingTemplates.Where(x => x.IsDeleted == false).ToList();                
            }

            return bookings;
        }        

        public async Task<CheckResultDto> HasCoachNotOverlappedBooking(Coach coach, Booking bookingData, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                var result = new CheckResultDto() { Result = true };

                var overlappedBooking =
                    (await db.GetTable<Booking>()
                        .LoadWith(x => x.Service)
                        .LoadWith(x => x.BookingsToCoachesLinks)
                        .Where(
                            x => x.IsDeleted == false &&
                                 x.BookingsToCoachesLinks.Any(c => c.CoachId == coach.Id) &&
                                 x.DateOn == bookingData.DateOn &&
                                 x.Id != bookingData.Id
                         ).ToListAsync())
                    .FirstOrDefault(x => DateTimeExtentions.IsOverlap(bookingData.BeginTime, bookingData.EndTime, x.BeginTime, x.EndTime));

                if (overlappedBooking != null)
                {
                    result.Result = false;
                    result.ErrorMessage = $"Coach has another booking during the selected time interval: {overlappedBooking.BeginTime.ToString("hh:mm tt")} - {overlappedBooking.EndTime.ToString("hh:mm tt")}, Service: {overlappedBooking.Service.Name}";
                }

                return result;


            }, dbContext);
        }

        public async Task<CheckResultDto> HasCoachScheduleFitBooking(Coach coach, Booking bookingData, DataConnection dbContext = null)
        {
            return await RunWithinTransaction((db) =>
            {               
                var result = new CheckResultDto();
                var activeSchedule = coach.Schedules?.FirstOrDefault(x => x.IsDeleted == false && x.IsActive);
                var bookedDayOfWeek = DateTimeExtentions.ToEuropeanDayNumber(bookingData.DateOn);

                if (activeSchedule != null && activeSchedule.SchedulesData != null)
                {
                    //day schedule check
                    var dayScheduleChecks = 
                        activeSchedule.SchedulesData.Any(
                            x => x.IsDeleted == false &&
                            (bookingData.DateOn == x.DateOn && bookingData.BeginTime >= x.BeginTime && bookingData.EndTime <= x.EndTime) && x.IsAvialable
                        );

                    //week schedule check
                    var weekScheduleChecks =
                        activeSchedule.SchedulesData.Any(
                            x => x.IsDeleted == false &&
                            (bookedDayOfWeek == x.DayNumber && bookingData.BeginTime >= DateTimeExtentions.SetTime(bookingData.BeginTime, x.BeginTime) &&
                                bookingData.EndTime <= DateTimeExtentions.SetTime(bookingData.EndTime, x.EndTime)) && x.IsAvialable
                        );

                    result.Result = weekScheduleChecks || dayScheduleChecks;
                }

                if (!result.Result)
                {
                    result.ErrorMessage = "Coach is currently unavailable (check coach's schedule)";
                }

                return Task.FromResult<dynamic>(result);

            }, dbContext);
        }

        public async Task<CheckResultDto> HasCoachScheduleFitBreaks(Coach coach, Booking bookingData, DataConnection dbContext = null)
        {
            return await RunWithinTransaction((db) =>
            {
                var result = new CheckResultDto();
                var activeSchedule = coach.Schedules?.FirstOrDefault(x => x.IsDeleted == false && x.IsActive);
                var bookedDayOfWeek = DateTimeExtentions.ToEuropeanDayNumber(bookingData.DateOn);

                if (activeSchedule != null && activeSchedule.SchedulesData != null)
                {
                    //day schedule check
                    var dayScheduleCheck =
                         activeSchedule.SchedulesData.FirstOrDefault(
                            x => x.IsDeleted == false &&
                            DateTimeExtentions.IsOverlap(bookingData.BeginTime, bookingData.EndTime, x.BeginTime, x.EndTime) && !x.IsAvialable
                        );

                    //week schedule check
                    var weekScheduleCheck =
                         activeSchedule.SchedulesData.FirstOrDefault(
                            x => x.IsDeleted == false && bookedDayOfWeek == x.DayNumber &&
                            DateTimeExtentions.IsOverlap(bookingData.BeginTime, bookingData.EndTime, DateTimeExtentions.SetTime(bookingData.BeginTime, x.EndTime), DateTimeExtentions.SetTime(bookingData.EndTime, x.EndTime))
                            && !x.IsAvialable
                        );

                    result.Result = (dayScheduleCheck == null) && (weekScheduleCheck == null);


                    if (!result.Result)
                    {
                        result.ErrorMessage = $"Coach is currently might be unavailable (has scheduled a break)";
                    }

                }

                return Task.FromResult<dynamic>(result);

            }, dbContext);
        }

        private async Task<bool> IsCoachAvialableForBooking(Coach coach, Booking bookingData, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {                
                bool hasOverlappedBookings =
                    !(await HasCoachNotOverlappedBooking(coach, bookingData, db)).Result;

                bool eligibleForService =
                    bookingData.Service.ServiceToCoachesLinks.Any(x => x.CoachId == coach.Id);

                bool passedScheduleCheck = false;
                var activeSchedule = coach.Schedules?.FirstOrDefault(x => x.IsDeleted == false && x.IsActive);

                var bookedDayOfWeek = DateTimeExtentions.ToEuropeanDayNumber(bookingData.DateOn);

                if (activeSchedule != null && activeSchedule.SchedulesData != null) {
                    passedScheduleCheck =
                        //day schedule
                        activeSchedule.SchedulesData.Any(
                            x => x.IsDeleted == false &&
                            DateTimeExtentions.IsOverlap(bookingData.BeginTime, bookingData.EndTime, x.BeginTime, x.EndTime) && x.IsAvialable) ||
                        //week schedule
                        activeSchedule.SchedulesData.Any(x => 
                            (bookedDayOfWeek == x.DayNumber && bookingData.BeginTime >= DateTimeExtentions.SetTime(bookingData.BeginTime, x.BeginTime) &&
                                bookingData.EndTime <= DateTimeExtentions.SetTime(bookingData.EndTime, x.EndTime) && x.IsAvialable));
                }
                return !hasOverlappedBookings && eligibleForService && passedScheduleCheck;

            }, dbContext);
        }

        public async Task<CheckResultDto> HasHorseNotOverlappedBooking(Hors horse, Booking bookingData, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                var result = new CheckResultDto() { Result = true };

                var overlappedBooking =
                (await db.GetTable<Booking>()
                    .LoadWith(x => x.BookingsToHorsesLinks)                    
                    .Where(
                        x => x.IsDeleted == false &&
                                x.BookingsToHorsesLinks.Any(h => h.HorseId == horse.Id) &&
                                x.DateOn == bookingData.DateOn &&
                                x.Id != bookingData.Id
                        ).ToListAsync())
                    .FirstOrDefault(x => DateTimeExtentions.IsOverlap(bookingData.BeginTime, bookingData.EndTime, x.BeginTime, x.EndTime));

                if (overlappedBooking != null)
                {
                    result.Result = false;
                    result.ErrorMessage = $"Horse has another booking during the selected time interval: {overlappedBooking.BeginTime.ToString("hh:mm tt")} - {overlappedBooking.EndTime.ToString("hh:mm tt")}";
                }

                return result;
            }, dbContext);
        }

        public async Task<CheckResultDto> HasHorseWorkedLessThanAllowed(Hors horse, Booking bookingData, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                var result = new CheckResultDto() { Result = true };

                var workedMinutes =
                (await db.GetTable<Booking>()
                    .Where(
                        x => x.IsDeleted == false &&
                                x.BookingsToHorsesLinks.Any(h => h.HorseId == horse.Id) &&
                                x.DateOn == bookingData.DateOn &&
                                x.Id != bookingData.Id
                        ).ToListAsync())
                    .Sum(x => x.EndTime.Subtract(x.BeginTime).Minutes);

                if ((workedMinutes / 60) > horse.MaxWorkingHours) 
                {
                    var workedTimeStr = $"{workedMinutes / 60} h {workedMinutes % 60} min";

                    result.Result = false;
                    result.ErrorMessage = $"Horse already has booked lessons (total time is {workedTimeStr}) which is more than maximum allowed time ({horse.MaxWorkingHours} h)";
                }

                return result;
            }, dbContext);
        }

        public async Task<CheckResultDto> HasHorseRequiredBreak(Hors horse, Booking bookingData, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                var result = new CheckResultDto() { Result = true };
                var lastBooking =
                    (await
                        db.GetTable<Booking>()
                        .Where(x => x.IsDeleted == false &&
                              x.BookingsToHorsesLinks.Any(h => h.HorseId == horse.Id) &&
                              x.DateOn == bookingData.DateOn &&
                              x.BeginTime < bookingData.BeginTime                        
                        )
                        .OrderByDescending(x => x.BeginTime)
                        .FirstOrDefaultAsync()
                    );

                if (lastBooking != null)
                {
                    var timediff = bookingData.BeginTime - lastBooking.EndTime;
                    if (timediff < Constants.HorseBreakTime)
                    {
                        result.Result = false;
                        result.ErrorMessage = $"Horse has not enoulgh time for rest since the last booking (finished: {lastBooking.EndTime.ToString("hh:mm tt")})";
                    }
                }

                return result;

            }, dbContext);
        }

        public async Task<CheckResultDto> HasHorseScheduleFitBooking(Hors horse, Booking bookingData, DataConnection dbContext = null)
        {
            return await RunWithinTransaction((db) =>
            {
                bool passedScheduleCheck = !(horse.HorsesScheduleData?.Any() ?? false);

                if (!passedScheduleCheck) {

                    //day schedule check
                    var dayScheduleChecks =
                        horse.HorsesScheduleData.Any(x => x.IsDeleted == false && x.StartDate.HasValue && x.EndDate.HasValue &&
                            DateTimeExtentions.IsOverlap(bookingData.BeginTime, bookingData.EndTime, x.StartDate.Value, x.EndDate.Value));

                    var bookedDayOfWeek = DateTimeExtentions.ToEuropeanDayNumber(bookingData.DateOn);

                    //week schedule check
                    var weekScheduleChecks =
                        horse.HorsesScheduleData.Any(x => x.IsDeleted == false && x.DayOfWeek.HasValue && x.DayOfWeek == bookedDayOfWeek
                            );

                    //Horses schedule can have only not working intervals
                    passedScheduleCheck = !(weekScheduleChecks || dayScheduleChecks);
                }

                var result = new CheckResultDto() { Result = passedScheduleCheck };

                if (!passedScheduleCheck)
                {
                    result.ErrorMessage = "Horse is currently unavailable (check horse's schedule)";
                }

                return Task.FromResult<dynamic>(result);
            }, dbContext);
        }

        public async Task<bool> IsHorseAvialableForBooking(Hors horse, Booking bookingData, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                bool hasOverlappedBookings =
                    !(await HasHorseNotOverlappedBooking(horse, bookingData, db)).Result;

                bool eligibleForService =
                    bookingData.Service.ServiceToHorsesLinks.Any(x => x.HorseId == horse.Id);

                bool passedScheduleCheck = (await HasHorseScheduleFitBooking(horse, bookingData)).Result;

                return !hasOverlappedBookings && eligibleForService && passedScheduleCheck;
            }, dbContext);
        }                

        public override async Task<List<Booking>> GetByParams(Expression<Func<Booking, bool>> where, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                var result = await
                    db.GetTable<Booking>()
                        .LoadWith(x => x.BookingsTemplateMetadata)
                        .LoadWith(x => x.BookingsTemplateMetadata.BookingTemplates)
                        .LoadWith(x => x.BookingPayments)
                        .LoadWith(x => x.BookingPayments.First().PaymentType)
                        .LoadWith(x => x.BookingsToClientsLinks)
                        .LoadWith(x => x.BookingsToClientsLinks.First().Client)
                        .LoadWith(x => x.BookingsToCoachesLinks)
                        .LoadWith(x => x.BookingsToCoachesLinks.First().Coach)
                        .LoadWith(x => x.BookingsToCoachesLinks.First().Coach.Schedules)
                        .LoadWith(x => x.BookingsToCoachesLinks.First().Coach.Schedules.First().SchedulesData)
                        .LoadWith(x => x.Service)
                        .LoadWith(x => x.Service.ServiceToCoachesLinks)
                        .LoadWith(x => x.Service.ServiceToHorsesLinks)
                        .LoadWith(x => x.BookingsToHorsesLinks)
                        .LoadWith(x => x.BookingsToHorsesLinks.First().Hor)
                        .LoadWith(x => x.BookingsToHorsesLinks.First().Hor.HorsesScheduleData)
                        .Where(x => x.IsDeleted == false)
                        .Where(where)
                        .OrderBy(x => x.BeginTime)
                        .ToListAsync();                

                return AccessFilter(result);

            }, dbContext);
        }


        public override async Task Delete(long id, DataConnection dbContext = null)
        {
            await RunWithinTransaction(async (db) =>
            {
                await db.GetTable<BookingPayments>().Where(p => p.BookingId == id).Set(x => x.IsDeleted, true).UpdateAsync();                                    

                await base.Delete(id, db);

                return null;

            }, dbContext);
        }

        private void Append(StringBuilder sb, string message)
        {
            if (sb.Length > 0 && !string.IsNullOrEmpty(message))
                sb.AppendFormat(", {0}", message);
            else
                sb.Append(message);            
        }

        public async Task<String> RunValidations(Booking booking, bool ErrorsValidation = true, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                var error = new StringBuilder();

                if (ErrorsValidation)
                {
                    foreach (var coach in booking.BookingsToCoachesLinks.Select(x => x.Coach).ToList())
                    {
                        Append(error, (await HasCoachNotOverlappedBooking(coach, booking, db)).ToString());
                        Append(error, (await HasCoachScheduleFitBooking(coach, booking, db)).ToString());
                    }

                    foreach (var horse in booking.BookingsToHorsesLinks.Select(x => x.Hor).ToList())
                    {
                        Append(error, (await HasHorseNotOverlappedBooking(horse, booking, db)).ToString());
                        Append(error, (await HasHorseScheduleFitBooking(horse, booking, db)).ToString());
                    }
                }
                //just Warnings
                else
                {
                    foreach (var horse in booking.BookingsToHorsesLinks.Select(x => x.Hor).ToList())
                    {
                        Append(error, (await HasHorseRequiredBreak(horse, booking, db)).ToString());
                        Append(error, (await HasHorseWorkedLessThanAllowed(horse, booking, db)).ToString());                        
                    }

                    foreach (var coach in booking.BookingsToCoachesLinks.Select(x => x.Coach).ToList())
                    {
                        Append(error, (await HasCoachScheduleFitBreaks(coach, booking, db)).ToString());                        
                    }
                }

                return error.ToString();
            }, dbContext);
        }


    }
}

