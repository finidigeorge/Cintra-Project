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
    public class BookingRepository: GenericPreservableRepository<Booking>
    {
        private readonly IBookingPaymentsRepository _paymentsRepository;

        public BookingRepository(IBookingPaymentsRepository paymentsRepository)
        {
            _paymentsRepository = paymentsRepository;
        }

        public override async Task<List<Booking>> GetByParams(Func<Booking, bool> where)
        {
            using (var db = new CintraDB())
            {
                return await Task.FromResult(
                    db.Bookings
                        .LoadWith(x => x.bookingpayments)
                        .LoadWith(x => x.client)
                        .LoadWith(x => x.coach)
                        .LoadWith(x => x.service)
                        .LoadWith(x => x.hors)
                        .Where(where).Where(x => x.IsDeleted == false).ToList()
                );
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

