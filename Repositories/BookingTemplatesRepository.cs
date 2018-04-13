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
                        .LoadWith(x => x.BookingTemplatesToClientsLinks)
                        .LoadWith(x => x.BookingTemplatesToClientsLinks.First().Client)
                        .LoadWith(x => x.BookingTemplatesToCoachesLinks)
                        .LoadWith(x => x.BookingTemplatesToCoachesLinks.First().Coach)
                        .LoadWith(x => x.BookingTemplatesToCoachesLinks.First().Coach.Schedules)
                        .LoadWith(x => x.BookingTemplatesToCoachesLinks.First().Coach.Schedules.First().SchedulesData)
                        .LoadWith(x => x.Service)
                        .LoadWith(x => x.Service.ServiceToCoachesLinks)
                        .LoadWith(x => x.Service.ServiceToHorsesLinks)
                        .LoadWith(x => x.BookingTemplatesToHorsesLinks)
                        .LoadWith(x => x.BookingTemplatesToHorsesLinks.First().Hor)
                        .LoadWith(x => x.BookingTemplatesToHorsesLinks.First().Hor.HorsesScheduleData)
                        .Where(where).Where(x => x.IsDeleted == false).ToList()
                );                

            }, dbContext);
        }
    }
}
