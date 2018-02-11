using DataModels;
using LinqToDB;
using Repositories.Interfaces;
using Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    [PerScope]
    public class BookingPaymentsRepository: GenericPreservableRepository<BookingPayments>, IBookingPaymentsRepository
    {
        public override async Task<List<BookingPayments>> GetByParams(Func<BookingPayments, bool> where, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                return await Task.FromResult(
                    db.BookingPayments
                        .LoadWith(x => x.PaymentOptions)                        
                        .Where(where).Where(x => x.IsDeleted == false).ToList()
                );
            }, dbContext);
        }

        public async Task SynchronizeWithBooking(long bookingId, BookingPayments payment, CintraDB dbContext = null)
        {
            await RunWithinTransaction(async (db) =>
            {
                if (payment == null)
                {
                    db.BookingPayments.Where(p => p.BookingId == bookingId).Set(x => x.IsDeleted, true).Update();
                    return null;
                }


                db.BookingPayments.Where(p => p.BookingId == bookingId && p.Id != payment.Id).Set(x => x.IsDeleted, true).Update();
                payment.BookingId = bookingId;
                await Create(payment, db);
                return null;

            }, dbContext);

        }
    }
}
