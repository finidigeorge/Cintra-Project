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
    public class BookingTemplatesRepository: GenericPreservableRepository<BookingTemplates>, IBookingTemplateRepository
    {
        public override async Task<long> Create(BookingTemplates entity, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                if (entity.Id == 0)
                    entity.Id = (long)(await db.InsertWithIdentityAsync(entity));
                else
                {
                    await db.BookingTemplatesToClientsLink.DeleteAsync(x => x.BookingTemplateId == entity.Id);
                    await db.BookingTemplatesToCoachesLink.DeleteAsync(x => x.BookingTemplateId == entity.Id);
                    await db.BookingTemplatesToHorsesLink.DeleteAsync(x => x.BookingTemplateId == entity.Id);
                    await db.UpdateAsync(entity);
                }

                foreach (var c in entity.BookingTemplatesToClientsLinks)
                {
                    c.BookingTemplateId = entity.Id;
                    await db.InsertWithIdentityAsync(c);
                }

                foreach (var c in entity.BookingTemplatesToCoachesLinks)
                {
                    c.BookingTemplateId = entity.Id;
                    await db.InsertWithIdentityAsync(c);
                }

                foreach (var c in entity.BookingTemplatesToHorsesLinks)
                {
                    c.BookingTemplateId = entity.Id;
                    await db.InsertWithIdentityAsync(c);
                }

                return entity.Id;
            }, dbContext);
        }

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
