using DataModels;
using LinqToDB;
using LinqToDB.Data;
using Repositories.Interfaces;
using Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    [PerScope]
    public class BookingPaymentsRepository: GenericPreservableRepository<BookingPayments>, IBookingPaymentsRepository
    {
        public override async Task<List<BookingPayments>> GetByParams(Expression<Func<BookingPayments, bool>> where, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                return await
                    db.GetTable<BookingPayments>()
                        .LoadWith(x => x.PaymentOptions)
                        .Where(where).Where(x => x.IsDeleted == false)
                        .ToListAsync();                
            }, dbContext);
        }

        public async Task SynchronizeWithBooking(long bookingId, BookingPayments payment, DataConnection dbContext = null)
        {
            await RunWithinTransaction(async (db) =>
            {
                if (payment == null)
                {
                    db.GetTable<BookingPayments>().Where(p => p.BookingId == bookingId).Set(x => x.IsDeleted, true).Update();
                    return null;
                }

                db.GetTable<BookingPayments>().Where(p => p.BookingId == bookingId && p.Id != payment.Id).Set(x => x.IsDeleted, true).Update();
                

                payment.BookingId = bookingId;
                await Create(payment, db);
                
                return null;
            }, dbContext);

        }
    }
}
