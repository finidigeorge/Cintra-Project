using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using LinqToDB;
using Repositories.Interfaces;
using Shared.Attributes;
using System.Linq;

namespace Repositories
{
    [PerScope]
    public class CoachesRepository: GenericPreservableRepository<Coach>
    {
        private IEnumerable<Schedule> LoadSchedules(long coachId, CintraDB db)
        {
            return db.Schedules.LoadWith(x => x.SchedulesData).Where(x => x.CoachId == coachId && x.IsDeleted == false);            
        }


        public override async Task<List<Coach>> GetByParams(Func<Coach, bool> where)
        {
            using (var db = new CintraDB())
            {
                return await Task.FromResult(
                    db.Coaches.Where(where).Where(x => x.IsDeleted == false).Select(x =>
                    {
                        var res = x;
                        res.Schedules = LoadSchedules(x.Id, db).ToList();
                        return res;
                    }).ToList()
                );
            }
        }
    }
}
