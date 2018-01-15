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

namespace Repositories
{
    [PerScope]
    public class BookingRepository : GenericPreservableRepository<Booking>, IBookingRepository
    {
        private async Task<List<Booking>> AccessFilter(List<Booking> bookings, CintraDB db)
        {
            foreach (var b in bookings)
            {
                if (b.EndTime.TruncateToDayStart() >= DateTime.Now.TruncateToDayStart())
                {
                    b.BookingPayments = b.BookingPayments.Where(x => x.IsDeleted == false);
                    b.Service = b.Service.IsDeleted ? null : b.Service;
                    b.Hor = b.Hor.IsDeleted || !(await IsHorseAvialableForBooking(b.Hor, b)) ? null : b.Hor;
                    b.Coach = b.Coach.IsDeleted || !(await IsCoachAvialableForBooking(b.Coach, b)) ? null : b.Coach;
                }
            }

            return bookings;
        }

        private List<SchedulesData> GetCoachSchedulesData(DateTime onDate, Schedule activeSchedule)
        {
            var result = new List<SchedulesData>();
            result.AddRange(activeSchedule.SchedulesData.Where(x => x.DateOn == onDate));
 
            result.AddRange(activeSchedule.SchedulesData
                    .Where(x => x.DayNumber != null && x.DayNumber == (long)onDate.DayOfWeek)
                    .Select(x => 
                    {
                        var res = x;
                        res.BeginTime = onDate.AddHours(x.BeginTime.Hour).AddMinutes(x.BeginTime.Minute);
                        res.EndTime = onDate.AddHours(x.EndTime.Hour).AddMinutes(x.EndTime.Minute);

                        return x;
                    })
                );
            return result;
        }

        public async Task<CheckResultDto> HasCoachNotOverlappedBooking(Coach coach, Booking bookingData, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                var result = new CheckResultDto() { Result = true };

                var overlappedBooking =
                    (await db.Bookings
                        .LoadWith(x => x.Client)
                        .LoadWith(x => x.Hor)
                        .Where(
                            x => x.IsDeleted == false &&
                                 x.CoachId == coach.Id &&
                                 x.DateOn == bookingData.DateOn &&
                                 x.Id != bookingData.Id
                         ).ToListAsync())
                    .FirstOrDefault(x => DateTimeExtentions.IsOverlap(bookingData.BeginTime, bookingData.EndTime, x.BeginTime, x.EndTime));

                if (overlappedBooking != null)
                {
                    result.Result = false;
                    result.ErrorMessage = $"Coach has another booking during the selected time interval (Client: {overlappedBooking}, Horse: {overlappedBooking.Hor.Nickname}, Time: {overlappedBooking.BeginTime.ToString("hh:mm tt")} - {overlappedBooking.EndTime.ToString("hh:mm tt")})";
                }

                return result;


            }, dbContext);
        }

        public async Task<CheckResultDto> HasCoachScheduleFitBooking(Coach coach, Booking bookingData, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {               
                var result = new CheckResultDto();
                var activeSchedule = coach.Schedules?.FirstOrDefault(x => x.IsDeleted == false && x.IsActive);

                if (activeSchedule != null && activeSchedule.SchedulesData != null)
                {
                    result.Result = activeSchedule.SchedulesData.Any(
                        x => x.IsDeleted == false &&
                        (bookingData.BeginTime >= x.BeginTime && bookingData.EndTime <= x.EndTime) && x.IsAvialable
                    ) &&
                     !activeSchedule.SchedulesData.Any(
                        x => x.IsDeleted == false &&
                        DateTimeExtentions.IsOverlap(bookingData.BeginTime, bookingData.EndTime, x.BeginTime, x.EndTime) && !x.IsAvialable
                    );
                }

                if (!result.Result)
                {
                    result.ErrorMessage = "Coach is currently unavailable (check coach's schedule)";
                }

                return result;

            }, dbContext);
        }

        private async Task<bool> IsCoachAvialableForBooking(Coach coach, Booking bookingData, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {                
                bool hasOverlappedBookings =
                    !(await HasCoachNotOverlappedBooking(coach, bookingData, db)).Result;

                bool eligibleForService =
                    bookingData.Service.ServiceToCoachesLinks.Any(x => x.CoachId == coach.Id);

                bool passedScheduleCheck = false;
                var activeSchedule = coach.Schedules?.FirstOrDefault(x => x.IsDeleted == false && x.IsActive);

                if (activeSchedule != null && activeSchedule.SchedulesData != null) {
                    passedScheduleCheck = activeSchedule.SchedulesData.Any(
                        x => x.IsDeleted == false && 
                        DateTimeExtentions.IsOverlap(bookingData.BeginTime, bookingData.EndTime, x.BeginTime, x.EndTime) &&
                        x.IsAvialable
                    );
                }
                return !hasOverlappedBookings && eligibleForService && passedScheduleCheck;

            }, dbContext);
        }

        public async Task<CheckResultDto> HasHorseNotOverlappedBooking(Hors horse, Booking bookingData, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                var result = new CheckResultDto() { Result = true };

                var overlappedBooking =
                (await db.Bookings
                    .LoadWith(x => x.Client)
                    .LoadWith(x => x.Coach)
                    .Where(
                        x => x.IsDeleted == false &&
                                x.HorseId == horse.Id &&
                                x.DateOn == bookingData.DateOn &&
                                x.Id != bookingData.Id
                        ).ToListAsync())
                    .FirstOrDefault(x => DateTimeExtentions.IsOverlap(bookingData.BeginTime, bookingData.EndTime, x.BeginTime, x.EndTime));

                if (overlappedBooking != null)
                {
                    result.Result = false;
                    result.ErrorMessage = $"Horse has another booking during the selected time interval (Coach: {overlappedBooking.Coach.Name}, Client: {overlappedBooking}, Time: {overlappedBooking.BeginTime.ToString("hh:mm tt")} - {overlappedBooking.EndTime.ToString("hh:mm tt")})";
                }

                return result;
            }, dbContext);
        }

        public async Task<CheckResultDto> HasHorseWorkedLessThanAllowed(Hors horse, Booking bookingData, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                var result = new CheckResultDto() { Result = true };

                var workedMinutes =
                (await db.Bookings
                    .Where(
                        x => x.IsDeleted == false &&
                                x.HorseId == horse.Id &&
                                x.DateOn == bookingData.DateOn &&
                                x.Id != bookingData.Id
                        ).ToListAsync())
                    .Sum(x => x.EndTime.Subtract(x.BeginTime).Minutes);

                if ((workedMinutes / 60) > horse.MaxWorkingHours) 
                {
                    var workedTimeStr = $"{workedMinutes / 60} h {workedMinutes % 60} min";

                    result.Result = false;
                    result.ErrorMessage = $"Horse already has booked lessions (total time is {workedTimeStr}) which is more than maximum allowed time ({horse.MaxWorkingHours} h)";
                }

                return result;
            }, dbContext);
        }

        public async Task<CheckResultDto> HasHorseRequiredBreak(Hors horse, Booking bookingData, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                var result = new CheckResultDto() { Result = true };
                var lastBooking =
                    (await
                        db.Bookings                                                
                        .Where(x => x.IsDeleted == false &&
                              x.HorseId == horse.Id &&
                              x.DateOn == bookingData.DateOn &&
                              x.BeginTime < bookingData.BeginTime                        
                        )
                        .OrderByDescending(x => x.BeginTime)
                        .FirstOrDefaultAsync()
                    );

                if (lastBooking != null)
                {
                    var timediff = bookingData.BeginTime - lastBooking.BeginTime;
                    if (timediff < Constants.HorseBreakTime)
                    {
                        result.Result = false;
                        result.ErrorMessage = $"Horse has not enoulgh time for rest since the last booking (finished: {lastBooking.EndTime.ToString("hh:mm tt")})";
                    }
                }

                return result;

            }, dbContext);
        }

        public async Task<CheckResultDto> HasHorseScheduleFitBooking(Hors horse, Booking bookingData, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {                
                bool passedScheduleCheck = !(horse.HorsesScheduleData?.Any() ?? false)
                    || !(horse.HorsesScheduleData.Any(x => x.IsDeleted == false && DateTimeExtentions.IsOverlap(bookingData.BeginTime, bookingData.EndTime, x.StartDate, x.EndDate)));

                var result = new CheckResultDto() { Result = passedScheduleCheck };

                if (!passedScheduleCheck)
                {
                    result.ErrorMessage = "Horse is currently unavailable (check horse's schedule)";
                }

                return result;
            }, dbContext);
        }

        public async Task<bool> IsHorseAvialableForBooking(Hors horse, Booking bookingData, CintraDB dbContext = null)
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

        public override async Task<List<Booking>> GetByParams(Func<Booking, bool> where, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                var result =  await Task.FromResult(
                    db.Bookings
                        .LoadWith(x => x.BookingPayments)
                        .LoadWith(x => x.BookingPayments.First().PaymentType)
                        .LoadWith(x => x.Client)
                        .LoadWith(x => x.Coach)
                        .LoadWith(x => x.Coach.Schedules)
                        .LoadWith(x => x.Coach.Schedules.First().SchedulesData)
                        .LoadWith(x => x.Service)                        
                        .LoadWith(x => x.Service.ServiceToCoachesLinks)
                        .LoadWith(x => x.Service.ServiceToHorsesLinks)
                        .LoadWith(x => x.Hor)
                        .LoadWith(x => x.Hor.HorsesScheduleData)
                        .Where(where).Where(x => x.IsDeleted == false).ToList()
                );

                return await AccessFilter(result, db);

            }, dbContext);
        }

        public override async Task Delete(long id, CintraDB dbContext = null)
        {
            await RunWithinTransaction(async (db) =>
            {
                db.BookingPayments.Where(p => p.BookingId == id).Set(x => x.IsDeleted, true).Update();
                await base.Delete(id, db);

                return null;

            }, dbContext);
        }

        
    }
}

