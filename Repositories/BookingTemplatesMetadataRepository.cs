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
using System.Threading;
using System.Threading.Tasks;

namespace Repositories
{
    [PerScope]
    public class BookingTemplatesMetadataRepository : GenericRepository<BookingsTemplateMetadata>, IBookingTemplatesMetadataRepository
    {
        private readonly IBookingRepository _bookingRepository = new BookingRepository();
        private readonly BookingTemplatesRepository _bookingTemplatesRepository = new BookingTemplatesRepository();
        private static readonly SemaphoreSlim _lockObject = new SemaphoreSlim(1, 1);

        public async Task GenerateAllPermanentBookings(DateTime onDate, CintraDB dbContext = null)
        {
            await RunWithinTransaction(async (db) =>
            {                
                try
                {
                    await _lockObject.WaitAsync();

                    var metadataList =
                        from m in db.BookingsTemplateMetadata
                        join t in db.BookingTemplates on m.Id equals t.TemplateMetadataId
                        where onDate >= m.StartDate && m.EndDate == null && t.DayOfWeek == onDate.ToEuropeanDayNumber() && !t.IsDeleted
                        select m;                    

                    foreach (var m in metadataList)
                    {                        
                        if (m.IsFortnightly)
                        {
                            await GenerateFortnightEvents(onDate, db, m.Id);                            
                        }
                        else
                        {
                            await GenerateWeekEvents(onDate, db, m.Id);                            
                        }
                    }
                }
                finally
                {
                    _lockObject.Release();
                }

                return null;

            }, dbContext);
        }

        private async Task GenerateWeekEvents(DateTime onDate, CintraDB db, long metadataId)
        {
            var alreadyGenerated = await db.Bookings.AnyAsync(x => x.DateOn == onDate && x.TemplateMetadataId == metadataId);

            if (!alreadyGenerated)
            {
                var templates = await _bookingTemplatesRepository.GetByParams(x => x.TemplateMetadataId == metadataId, db);

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

        private async Task GenerateFortnightEvents(DateTime onDate, CintraDB db, long metadataId)
        {
            var alreadyGenerated = await db.Bookings.AnyAsync(x => x.DateOn == onDate && x.TemplateMetadataId == metadataId);

            if (!alreadyGenerated)
            {
                var templates = await _bookingTemplatesRepository.GetByParams(x => x.TemplateMetadataId == metadataId, db);

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
