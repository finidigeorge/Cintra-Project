using DataModels;
using DbLayer.Extentions;
using LinqToDB;
using Mapping;
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
    public class BookingTemplatesMetadataRepository : GenericRepository<BookingsTemplateMetadata>, IBookingTemplatesMetadataRepository
    {
        private readonly IBookingRepository _bookingRepository = new BookingRepository();
        private readonly BookingTemplatesRepository _bookingTemplatesRepository = new BookingTemplatesRepository();

        public async Task GenerateAllPermanentBookings(DateTime onDate, CintraDB dbContext = null)
        {
            await RunWithinTransaction(async (db) =>
            {
                var metadataList = db.BookingsTemplateMetadata.Where(x => onDate >= x.StartDate && x.EndDate == null);

                foreach (var m in metadataList)
                {
                    var templates = await _bookingTemplatesRepository.GetByParams(x => x.TemplateMetadataId == m.Id, db);
                    var hasBooking = false;

                    if (m.IsFortnightly)
                    {
                        var daysDiff = (onDate.TruncateToWeekStart() - m.StartDate.TruncateToWeekStart()).Days;
                        hasBooking = templates.Any(x => !x.IsDeleted && x.DayOfWeek == onDate.ToEuropeanDayNumber());

                        if (hasBooking)
                        {
                            var alreadyGenerated = await db.Bookings.AnyAsync(x => x.DateOn == onDate && !x.IsDeleted && x.TemplateMetadataId == m.Id);

                            if (!alreadyGenerated)
                            {
                                foreach (var t in
                                    //first week events
                                    templates
                                        .Where(x => !x.IsDeleted && x.DayOfWeek == onDate.ToEuropeanDayNumber() && x.IsFirstWeek)
                                        .ToList()
                                        .Where(x => ((onDate.TruncateToWeekStart() - x.BeginTime.TruncateToWeekStart()).Days % 14) == 0)
                                        .Union
                                        (
                                        //second week events
                                        templates
                                            .Where(x => !x.IsDeleted && x.DayOfWeek == onDate.ToEuropeanDayNumber() && !x.IsFirstWeek).ToList()
                                            .Where(x => ((onDate.TruncateToWeekStart() - x.BeginTime.TruncateToWeekStart()).Days % 14) == 0)
                                        )
                                        .Select(x => ObjectMapper.Map<Booking>(x)))
                                {
                                    t.DateOn = onDate;
                                    t.BeginTime = onDate.Add(new TimeSpan(t.BeginTime.Hour, t.BeginTime.Minute, 0));
                                    t.EndTime = onDate.Add(new TimeSpan(t.EndTime.Hour, t.EndTime.Minute, 0));
                                    await _bookingRepository.Create(t, db);
                                }
                            }

                        }
                    }
                    else
                    {
                        hasBooking = templates.Any(x => !x.IsDeleted && x.DayOfWeek == onDate.ToEuropeanDayNumber());

                        if (hasBooking)
                        {
                            var alreadyGenerated = await db.Bookings.AnyAsync(x => x.DateOn == onDate && !x.IsDeleted && x.TemplateMetadataId == m.Id);

                            if (!alreadyGenerated)
                            {
                                foreach (var t in templates.Where(x => !x.IsDeleted && x.DayOfWeek == onDate.ToEuropeanDayNumber()).ToList()
                                    .Select(x => ObjectMapper.Map<Booking>(x)))
                                {
                                    t.DateOn = onDate;
                                    t.BeginTime = onDate.Add(new TimeSpan(t.BeginTime.Hour, t.BeginTime.Minute, 0));
                                    t.EndTime = onDate.Add(new TimeSpan(t.EndTime.Hour, t.EndTime.Minute, 0));
                                    await _bookingRepository.Create(t, db);
                                }
                            }

                        }
                    }

                }

                return null;

            }, dbContext);
        }

        public async Task CancelAllBookings(long metadataId, DateTime onDate, CintraDB dbContext = null)
        {
            await RunWithinTransaction(async (db) =>
            {
                await db.BookingsTemplateMetadata
                    .Where(x => x.Id == metadataId)
                    .Set(x => x.EndDate, onDate)
                    .UpdateAsyncWithLock();

                await db.Bookings
                    .Where(x => x.TemplateMetadataId == metadataId && x.DateOn >= onDate)
                    .Set(x => x.IsDeleted, true)
                    .UpdateAsyncWithLock();

                return null;

            }, dbContext);
        }
    }
}
