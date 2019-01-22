using DataModels;
using DbLayer.Extentions;
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
    public class BookingTemplatesRepository : GenericPreservableRepository<BookingTemplates>, IBookingTemplateRepository
    {
        public override async Task<long> Create(BookingTemplates entity, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                if (entity.Id == 0)
                    entity.Id = (long)(await db.InsertWithIdentityAsyncWithLock(entity));

                else
                {
                    await db.GetTable<BookingTemplatesToClientsLink>().DeleteAsyncWithLock(x => x.BookingTemplateId == entity.Id);
                    await db.GetTable<BookingTemplatesToCoachesLink>().DeleteAsyncWithLock(x => x.BookingTemplateId == entity.Id);
                    await db.GetTable<BookingTemplatesToHorsesLink>().DeleteAsyncWithLock(x => x.BookingTemplateId == entity.Id);

                    await db.UpdateAsyncWithLock(entity);
                }


                foreach (var c in entity.BookingTemplatesToClientsLinks)
                {
                    c.BookingTemplateId = entity.Id;
                    await db.InsertWithIdentityAsyncWithLock(c);
                }

                foreach (var c in entity.BookingTemplatesToCoachesLinks)
                {
                    c.BookingTemplateId = entity.Id;
                    await db.InsertWithIdentityAsyncWithLock(c);
                }

                foreach (var c in entity.BookingTemplatesToHorsesLinks)
                {
                    c.BookingTemplateId = entity.Id;
                    await db.InsertWithIdentityAsyncWithLock(c);
                }



                return entity.Id;
            }, dbContext);
        }

        public override async Task<List<BookingTemplates>> GetByParams(Expression<Func<BookingTemplates, bool>> where, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                return await
                    db.GetTable<BookingTemplates>()
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
                        .Where(where)
                        .Where(x => x.IsDeleted == false)
                        .ToListAsync()
                    ;

            }, dbContext);
        }
    }
}
