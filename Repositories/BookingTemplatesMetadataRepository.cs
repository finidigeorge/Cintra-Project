using DataModels;
using LinqToDB;
using LinqToDB.Data;
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
        private readonly IBookingPaymentsRepository _paymentsRepository = new BookingPaymentsRepository();
        private readonly IBookingTemplateRepository _bookingTemplatesRepository = new BookingTemplatesRepository();

        private static readonly SemaphoreSlim _lockObject = new SemaphoreSlim(1, 1);

        public async Task GenerateAllPermanentBookings(DateTime onDate, DataConnection dbContext = null)
        {
            await RunWithinTransaction(async (db) =>
            {                
                try
                {
                    await _lockObject.WaitAsync();

                    var metadataList =
                        from m in db.GetTable<BookingsTemplateMetadata>()
                        join t in db.GetTable<BookingTemplates>() on m.Id equals t.TemplateMetadataId
                        where onDate >= m.StartDate && m.EndDate == null && t.DayOfWeek == onDate.ToEuropeanDayNumber() && !t.IsDeleted
                        select m;                    

                    foreach (var m in metadataList)
                    {                        
                        if (m.IsFortnightly)
                        {
                            await GenerateFortnightEvents(onDate, db, m);                            
                        }
                        else
                        {
                            await GenerateWeekEvents(onDate, db, m);                            
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

        private async Task<Booking> UpdateFromPrevBooking(BookingsTemplateMetadata metadata, DateTime onDate, DataConnection db, Booking booking)
        {
            var daysToSubtract = -7;

            if (metadata.IsFortnightly)
                daysToSubtract = -14;

            var prevWeekDate = onDate.AddDays(daysToSubtract);
            var prevBooking = await db.GetTable<Booking>()
                .FirstOrDefaultAsync(x => x.DateOn == prevWeekDate && x.TemplateMetadataId == metadata.Id && !x.IsDeleted);

            if (prevBooking != null)
            {
                prevBooking = await _bookingRepository.GetById(prevBooking.Id, db);

                booking.ServiceId = prevBooking.ServiceId;                
                booking.BookingsToHorsesLinks = prevBooking.BookingsToHorsesLinks.Select(x => new BookingsToHorsesLink() { Booking = booking, Hor = x.Hor, HorseId = x.HorseId }).ToList();
                var payment = booking.BookingPayments?.FirstOrDefault();
                if (payment != null)
                {
                    var prevPayment = prevBooking.BookingPayments?.FirstOrDefault();
                    if (prevPayment != null)
                    {
                        payment.PaymentTypeId = prevPayment.PaymentTypeId;
                        payment.PaymentType = prevPayment.PaymentType;                        
                    }
                }
                
            }

            return booking;
        }

        private async Task GenerateWeekEvents(DateTime onDate, DataConnection db, BookingsTemplateMetadata metadata)
        {
            var alreadyGenerated = await db.GetTable<Booking>().AnyAsync(x => x.DateOn == onDate && x.TemplateMetadataId == metadata.Id);

            if (!alreadyGenerated)
            {
                var templates = await _bookingTemplatesRepository.GetByParams(x => x.TemplateMetadataId == metadata.Id &&
                    !x.IsDeleted && x.DayOfWeek == onDate.ToEuropeanDayNumber(), db);

                foreach (var template in templates)
                {
                    var t = ObjectMapper.Map<Booking>(template);
                    t.DateOn = onDate;
                    t.BeginTime = onDate.Add(new TimeSpan(t.BeginTime.Hour, t.BeginTime.Minute, 0));
                    t.EndTime = onDate.Add(new TimeSpan(t.EndTime.Hour, t.EndTime.Minute, 0));

                    t = await UpdateFromPrevBooking(metadata, onDate, db, t);

                    var bookingId = await _bookingRepository.Create(t, db);
                    await _paymentsRepository.SynchronizeWithBooking(bookingId, ObjectMapper.Map<BookingPayments>(t.BookingPayments?.FirstOrDefault()), db);
                }
            }
        }

        private async Task GenerateFortnightEvents(DateTime onDate, DataConnection db, BookingsTemplateMetadata metadata)
        {
            var alreadyGenerated = await db.GetTable<Booking>().AnyAsync(x => x.DateOn == onDate && x.TemplateMetadataId == metadata.Id);

            if (!alreadyGenerated)
            {
                var templates = await _bookingTemplatesRepository.GetByParams(x => x.TemplateMetadataId == metadata.Id && 
                    !x.IsDeleted && x.DayOfWeek == onDate.ToEuropeanDayNumber(), db);

                foreach (var template in
                    //first week events
                    templates
                        .Where(x => x.IsFirstWeek)
                        .ToList()
                        .Where(x => ((onDate.TruncateToWeekStart() - x.BeginTime.TruncateToWeekStart()).Days % 14) == 0)
                        .Union
                        (
                        //second week events
                        templates
                            .Where(x => !x.IsFirstWeek).ToList()
                            .Where(x => ((onDate.TruncateToWeekStart() - x.BeginTime.TruncateToWeekStart()).Days % 14) == 0)
                        ))                        
                {
                    var t = ObjectMapper.Map<Booking>(template);
                    t.DateOn = onDate;
                    t.BeginTime = onDate.Add(new TimeSpan(t.BeginTime.Hour, t.BeginTime.Minute, 0));
                    t.EndTime = onDate.Add(new TimeSpan(t.EndTime.Hour, t.EndTime.Minute, 0));
                    t = await UpdateFromPrevBooking(metadata, onDate, db, t);

                    var bookingId = await _bookingRepository.Create(t, db);
                    await _paymentsRepository.SynchronizeWithBooking(bookingId, ObjectMapper.Map<BookingPayments>(t.BookingPayments?.FirstOrDefault()), db);
                }
            }
        }

        public async Task CancelAllBookings(long metadataId, DateTime onDate, DataConnection dbContext = null)
        {
            await RunWithinTransaction(async (db) =>
            {
                await db.GetTable<BookingsTemplateMetadata>()
                    .Where(x => x.Id == metadataId)
                    .Set(x => x.EndDate, onDate)
                    .UpdateAsync();

                await db.GetTable<Booking>()
                    .Where(x => x.TemplateMetadataId == metadataId && x.DateOn >= onDate)
                    .Set(x => x.IsDeleted, true)
                    .UpdateAsync();

                return null;

            }, dbContext);
        }
    }
}
