using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using LinqToDB;
using Repositories.Interfaces;
using Shared.Attributes;
using System.Linq;
using DbLayer.Extentions;
using System.Linq.Expressions;

namespace Repositories
{
    [PerScope]
    public class CoachesRepository : GenericPreservableRepository<Coach>
    {
        private IEnumerable<Schedule> LoadSchedules(long coachId, CintraDB db)
        {
            return db.Schedules.LoadWith(x => x.SchedulesData).Where(x => x.CoachId == coachId && x.IsDeleted == false);
        }

        public override async Task<long> Create(Coach entity, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                bool isNew = false;
                if (entity.Id == 0)
                {
                    isNew = true;
                }

                var coachId = await base.Create(entity, db);

                if (isNew && entity.CoachRoleId == (int)Shared.CoachRolesEnum.StaffMember)
                {
                    var serviceToCoachesLinks = db.CoachRolesToServicesLink.Where(x => x.CoachRoleId == (int)Shared.CoachRolesEnum.StaffMember).Select(x => new ServiceToCoachesLink() { CoachId = coachId, ServiceId = x.ServiceId }).ToList();
                    foreach (var c in serviceToCoachesLinks)
                        await db.InsertWithIdentityAsyncWithLock(c);
                }

                //assign all coach services to the new staff member
                if (isNew && entity.CoachRoleId == (int)Shared.CoachRolesEnum.Coach)
                {
                    var ids = db.CoachRolesToServicesLink.Where(x => x.CoachRoleId == (int)Shared.CoachRolesEnum.StaffMember).Select(x => x.ServiceId).ToList();
                    var hashSet = new HashSet<long>(ids);

                    var links = db.Services.Where(x => !hashSet.Contains(x.Id)).Select(x => new ServiceToCoachesLink() { CoachId = coachId, ServiceId = x.Id }).ToList();
                    foreach (var c in links)
                        await db.InsertWithIdentityAsyncWithLock(c);
                }

                return coachId;

            },
            dbContext
            );
        }


        public override async Task<List<Coach>> GetByParams(Expression<Func<Coach, bool>> where, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                return await Task.FromResult(
                    db.Coaches
                    .LoadWith(x => x.ServiceToCoachesLinks)
                    .Where(where)
                    .Where(x => x.IsDeleted == false)                    
                    .OrderBy(x => x.Name)
                    .ToList()
                    .Select(x =>
                    {
                        var res = x;
                        res.Schedules = LoadSchedules(x.Id, db).ToList();
                        return res;
                    })
                );
            }, dbContext);
        }
    }
}
