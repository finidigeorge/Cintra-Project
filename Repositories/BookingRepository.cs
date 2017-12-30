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
        public override async Task<List<Booking>> GetByParams(Func<Booking, bool> where)
        {
            using (var db = new CintraDB())
            {
                return await Task.FromResult(
                    db.Bookings
                        .LoadWith(x => x.BookingPayments.Where(c => !c.IsDeleted))
                        .LoadWith(x => x.BookingPayments.First().PaymentType)
                        .LoadWith(x => x.Client)
                        .LoadWith(x => x.Coach)
                        .LoadWith(x => x.Service)
                        .LoadWith(x => x.Service.Bookings.Where(c => !c.IsDeleted))
                        .LoadWith(x => x.Service.ServiceToCoachesLinks.Where(c => !c.IsDeleted))
                        .LoadWith(x => x.Service.ServiceToHorsesLinks.Where(c => !c.IsDeleted))
                        .LoadWith(x => x.Hor)
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

