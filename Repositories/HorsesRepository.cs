using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using LinqToDB;
using Repositories.Interfaces;
using Shared.Attributes;
using Shared.Extentions;

namespace Repositories
{
    [PerScope]
    public class HorsesRepository: GenericPreservableRepository<Hors>
    {
        public override async Task<long> Create(Hors entity, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                bool isNew = false;
                if (entity.Id == 0)
                {
                    isNew = true;
                }

                var id = await base.Create(entity, db);

                //assign all services to the new horse
                if (isNew)
                {
                    var serviceToHorsesLinks = db.ServiceToHorsesLink.Where(x => x.HorseId == entity.Id).Select(x => new ServiceToHorsesLink() { HorseId = id, ServiceId = x.ServiceId });
                    foreach (var c in serviceToHorsesLinks)
                        await db.InsertWithIdentityAsync(c);
                }

                return id;
            },
            dbContext
            );
        }

        public override async Task<List<Hors>> GetByParams(Func<Hors, bool> where, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                return await Task.FromResult(
                    db.Horses                        
                        .LoadWith(x => x.HorsesScheduleData)
                        .LoadWith(x => x.ServiceToHorsesLinks)
                        .Where(where).Where(x => x.IsDeleted == false).ToList()
                );
            }, dbContext);
        }        
    }
}
