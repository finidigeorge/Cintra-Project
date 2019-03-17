using DataModels;
using LinqToDB;
using LinqToDB.Data;
using Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repositories
{
    [PerScope]
    public class HorsesRepository : GenericPreservableRepository<Hors>
    {
        public override async Task<long> Create(Hors entity, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                bool isNew = false;
                if (entity.Id == 0)
                    isNew = true;

                var id = await base.Create(entity, db);

                if (isNew)
                {
                    var serviceToHorsesLinks = db.GetTable<ServiceToHorsesLink>().Where(x => x.HorseId == entity.Id)
                        .Select(x => new ServiceToHorsesLink() { HorseId = id, ServiceId = x.ServiceId }).ToList();
                    foreach (var c in serviceToHorsesLinks)
                        await db.InsertWithIdentityAsync(c);
                }

                //update links
                await db.GetTable<HorseToCoachesLink>()
                        .Where(x => x.HorseId == entity.Id)
                        .DeleteAsync();

                foreach (var c in entity.HorseToCoachesLinks)
                    await db.InsertWithIdentityAsync(c);


                return id;
            },
            dbContext
            );
        }

        public override async Task<List<Hors>> GetByParams(Expression<Func<Hors, bool>> where, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                return await
                    db.GetTable<Hors>()
                        .LoadWith(x => x.HorsesScheduleData)
                        .LoadWith(x => x.ServiceToHorsesLinks)
                        .LoadWith(x => x.HorseToCoachesLinks)
                        .LoadWith(x => x.HorseToCoachesLinks.First().Coach)
                        .Where(where).Where(x => x.IsDeleted == false)
                        .OrderBy(x => x.Nickname)
                        .ToListAsync();                
            }, dbContext);
        }
    }
}
