using DataModels;
using LinqToDB;
using Repositories.Interfaces;
using Shared.Attributes;
using Shared.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    [PerScope]
    public class BookingRepository : GenericPreservableRepository<Booking>
    {
        private async Task<List<Booking>> AccessFilter(List<Booking> bookings, CintraDB db)
        {
            foreach (var b in bookings)
            {
                if (b.EndTime.TruncateToDayStart() >= DateTime.Now.TruncateToDayStart())
                {
                    b.BookingPayments = b.BookingPayments.Where(x => x.IsDeleted = false);
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

        public async Task<bool> IsCoachAvialableForBooking(Coach coach, Booking bookingData, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {                
                bool hasOverlappedBookings =
                    (await db.Bookings
                        .Where(
                            x => x.IsDeleted == false &&
                                 x.CoachId == coach.Id &&
                                 x.DateOn == bookingData.DateOn &&
                                 x.Id != bookingData.Id
                         ).ToListAsync())
                    .Any(x => DateTimeExtentions.IsOverlap(bookingData.BeginTime, bookingData.EndTime, x.BeginTime, x.EndTime));

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

        public async Task<bool> IsHorseAvialableForBooking(Hors horse, Booking bookingData, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                bool hasOverlappedBookings =
                    (await db.Bookings
                        .Where(
                            x => x.IsDeleted == false &&
                                 x.HorseId == horse.Id &&
                                 x.DateOn == bookingData.DateOn &&
                                 x.Id != bookingData.Id
                    ).ToListAsync())
                    .Any(x => DateTimeExtentions.IsOverlap(bookingData.BeginTime, bookingData.EndTime, x.BeginTime, x.EndTime));

                bool eligibleForService =
                    bookingData.Service.ServiceToHorsesLinks.Any(x => x.HorseId == horse.Id);

                bool passedScheduleCheck = !(horse.HorsesScheduleData?.Any() ?? false)
                    && !(horse.HorsesScheduleData.Any(x => x.IsDeleted == false && DateTimeExtentions.IsOverlap(bookingData.BeginTime, bookingData.EndTime, x.StartDate, x.EndDate)));

                return !hasOverlappedBookings && eligibleForService && passedScheduleCheck;
            }, dbContext);
        }

        public override async Task<List<Booking>> GetByParams(Func<Booking, bool> where)
        {
            using (var db = new CintraDB())
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
            }
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

