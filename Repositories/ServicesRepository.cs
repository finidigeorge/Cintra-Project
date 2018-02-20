using System;
using System.Collections.Generic;
using System.Text;
using DataModels;
using Repositories.Interfaces;
using Shared.Attributes;
using System.Threading.Tasks;
using LinqToDB;
using System.Linq;

namespace Repositories
{
    [PerScope]
    public class ServicesRepository : GenericPreservableRepository<Service>
    {
        public override async Task<long> Create(Service entity, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {


                bool isNew = false;
                if (entity.Id == 0)
                {
                    isNew = true;                
                }
                
                var serviceId = await base.Create(entity, db);

                //assign all coaches to the new service
                if (isNew)
                {
                    entity.ServiceToCoachesLinks = db.Coaches.Where(x => !x.IsDeleted).Select(x => new ServiceToCoachesLink() { CoachId = x.Id, ServiceId = serviceId });
                }

                await db.ServiceToCoachesLink
                    .Where(x => x.ServiceId == entity.Id)
                    .DeleteAsync();

                foreach (var c in entity.ServiceToCoachesLinks)
                    await db.InsertWithIdentityAsync(c);


                await db.ServiceToHorsesLink
                    .Where(x => x.ServiceId == entity.Id)
                    .DeleteAsync();

                foreach (var c in entity.ServiceToHorsesLinks)
                    await db.InsertWithIdentityAsync(c);

                return serviceId;
            },
            dbContext
            );
        }

        public override async Task<List<Service>> GetByParams(Func<Service, bool> where, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                return await Task.FromResult(
                    db.Services                        
                        .LoadWith(x => x.ServiceToCoachesLinks)
                        .LoadWith(x => x.ServiceToCoachesLinks.First().Coach)
                        .LoadWith(x => x.ServiceToHorsesLinks)
                        .LoadWith(x => x.ServiceToHorsesLinks.First().Hor)
                        .Where(where).Where(x => x.IsDeleted == false).ToList()
                );
            }, dbContext);
        }
    }
}
