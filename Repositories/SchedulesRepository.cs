using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using LinqToDB;
using Shared.Attributes;
using Shared.Dto.Interfaces;

namespace Repositories
{
    [PerScope]
    public class SchedulesRepository: GenericPreservableRepository<Schedule>
    {
        private readonly SchedulesDataRepository _dataRepository = new SchedulesDataRepository();

        public override async Task<List<Schedule>> GetByParams(Func<Schedule, bool> where, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                return await Task.FromResult(db.GetTable<Schedule>().LoadWith(x => x.SchedulesData).Where(where).Where(x => x.IsDeleted == false).ToList());
            }, dbContext);
        }        

        public override async Task Delete(long id, CintraDB dbContext = null)
        {            
            await RunWithinTransaction(async (db) =>
            {
                var schedulesData = await  _dataRepository.GetByParams(x => x.ScheduleId == id);
                schedulesData.ForEach(async x => await _dataRepository.Delete(x.Id));                

                await base.Delete(id, db);

                return null;

            }, dbContext);
        }
    }
}
