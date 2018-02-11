using DataModels;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class BookingTemplatesRepository: GenericPreservableRepository<BookingTemplates>    
    {
        public override async Task<List<BookingTemplates>> GetByParams(Func<BookingTemplates, bool> where, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                return await Task.FromResult(
                    db.BookingTemplates                                                
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

            }, dbContext);
        }
    }
}
