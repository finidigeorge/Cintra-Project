using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using LinqToDB;
using LinqToDB.Data;
using Shared.Attributes;
using Shared.Dto.Interfaces;

namespace Repositories
{
    [PerScope]
    public class SchedulesRepository : GenericPreservableRepository<Schedule>
    {
        private readonly SchedulesDataRepository _dataRepository = new SchedulesDataRepository();

        public override async Task<List<Schedule>> GetByParams(Expression<Func<Schedule, bool>> where, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                return await db.GetTable<Schedule>().LoadWith(x => x.SchedulesData).Where(where).Where(x => x.IsDeleted == false).ToListAsync();
            }, dbContext);
        }

        public override async Task Delete(long id, DataConnection dbContext = null)
        {
            await RunWithinTransaction(async (db) =>
            {
                var schedulesData = await _dataRepository.GetByParams(x => x.ScheduleId == id, db);
                schedulesData.ForEach(async x => await _dataRepository.Delete(x.Id, db));

                await base.Delete(id, db);

                return null;

            }, dbContext);
        }
    }
}
