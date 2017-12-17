﻿using System;
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
                var serviceId = await base.Create(entity, db);

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

        public override async Task<List<Service>> GetByParams(Func<Service, bool> where)
        {
            using (var db = new CintraDB())
            {
                return await Task.FromResult(
                    db.Services                        
                        .LoadWith(x => x.ServiceToCoachesLinks)
                        .LoadWith(x => x.ServiceToCoachesLinks.First().Coach)
                        .LoadWith(x => x.ServiceToHorsesLinks)
                        .LoadWith(x => x.ServiceToHorsesLinks.First().Hor)
                        .Where(where).Where(x => x.IsDeleted == false).ToList()
                );
            }
        }
    }
}
