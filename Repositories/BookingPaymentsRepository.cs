using DataModels;
using LinqToDB;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class BookingPaymentsRepository: GenericPreservableRepository<BookingPayments>, IBookingPaymentsRepository
    {
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
