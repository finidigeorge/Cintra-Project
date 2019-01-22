using System;
using System.Collections.Generic;
using System.Text;
using DataModels;
using Repositories.Interfaces;
using Shared.Attributes;
using System.Threading.Tasks;
using LinqToDB;
using System.Linq;
using DbLayer.Extentions;
using System.Linq.Expressions;
using LinqToDB.Data;

namespace Repositories
{
    [PerScope]
    public class ServicesRepository : GenericPreservableRepository<Service>
    {
        public override async Task<long> Create(Service entity, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                var serviceId = await base.Create(entity, db);

                await db.GetTable<ServiceToCoachesLink>()
                        .Where(x => x.ServiceId == entity.Id)
                        .DeleteAsyncWithLock();

                foreach (var c in entity.ServiceToCoachesLinks)
                    await db.InsertWithIdentityAsyncWithLock(c);


                await db.GetTable<ServiceToCoachesLink>()
                    .Where(x => x.ServiceId == entity.Id)
                    .DeleteAsyncWithLock();

                foreach (var c in entity.ServiceToHorsesLinks)
                    await db.InsertWithIdentityAsyncWithLock(c);

                return serviceId;

            },
            dbContext
            );
        }

        public override async Task<List<Service>> GetByParams(Expression<Func<Service, bool>> where, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                return await Task.FromResult(
                    db.GetTable<Service>()
                        .LoadWith(x => x.ServiceToCoachesLinks)
                        .LoadWith(x => x.ServiceToCoachesLinks.First().Coach)
                        .LoadWith(x => x.ServiceToHorsesLinks)
                        .LoadWith(x => x.ServiceToHorsesLinks.First().Hor)
                        .Where(where)
                        .Where(x => x.IsDeleted == false)
                        .OrderBy(x => x.Name)
                        .ToList()
                );
            }, dbContext);
        }
    }
}
