using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using LinqToDB;
using Shared.Dto.Interfaces;

namespace Repositories
{
    public class SchedulesRepository: GenericRepository<Schedule>
    {
        public override async Task<List<Schedule>> GetByParams(Func<Schedule, bool> where)
        {
            using (var db = new CintraDB())
            {
                return await Task.FromResult(db.GetTable<Schedule>().LoadWith(x => x.data).Where(where).ToList());
            }
        }

        /*public override async Task<long> Create(Schedule entity, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                var result = await base.Create(entity, db);
                
                await Task.FromResult(db.GetTable<SchedulesData>()
                    .Where(x => x.ScheduleId == entity.Id)
                    .Delete());

                foreach (var d in entity.data)
                {
                    await Task.FromResult(db.InsertWithIdentity(d));
                }                
                
                return result;

            }, dbContext);
        }*/

        public override async Task Delete(long id, CintraDB dbContext = null)
        {
            await RunWithinTransaction(async (db) =>
            {                
                await Task.FromResult(db.GetTable<SchedulesData>()
                    .Where(x => x.ScheduleId == id)
                    .Delete());

                await base.Delete(id, db);

                return null;

            }, dbContext);
        }
    }
}
