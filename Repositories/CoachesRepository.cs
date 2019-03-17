using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using LinqToDB;
using Repositories.Interfaces;
using Shared.Attributes;
using System.Linq;
using System.Linq.Expressions;
using LinqToDB.Data;

namespace Repositories
{
    [PerScope]
    public class CoachesRepository : GenericPreservableRepository<Coach>
    {
        private IEnumerable<Schedule> LoadSchedules(long coachId, DataConnection db)
        {
            return db.GetTable<Schedule>().LoadWith(x => x.SchedulesData).Where(x => x.CoachId == coachId && x.IsDeleted == false);
        }

        public override async Task<long> Create(Coach entity, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                bool isNew = false;
                if (entity.Id == 0)
                {
                    isNew = true;
                }

                var coachId = await base.Create(entity, db);

                //create default schedule
                if (isNew) {
                    var scheduleRepository = new SchedulesRepository();
                    await scheduleRepository.Create(
                        new Schedule() { CoachId = coachId, Name = "Default Schedule", IsActive = true},
                        db);
                }

                if (isNew && entity.CoachRoleId == (int)Shared.CoachRolesEnum.StaffMember)
                {
                    var serviceToCoachesLinks = db.GetTable<CoachRolesToServicesLink>()
                        .Where(x => x.CoachRoleId == (int)Shared.CoachRolesEnum.StaffMember)
                        .Select(x => new ServiceToCoachesLink() { CoachId = coachId, ServiceId = x.ServiceId }).ToList();

                    foreach (var c in serviceToCoachesLinks)
                        await db.InsertWithIdentityAsync(c);
                }

                //assign all coach services to the new staff member
                if (isNew && entity.CoachRoleId == (int)Shared.CoachRolesEnum.Coach)
                {
                    var ids = db.GetTable<CoachRolesToServicesLink>().Where(x => x.CoachRoleId == (int)Shared.CoachRolesEnum.StaffMember).Select(x => x.ServiceId).ToList();
                    var hashSet = new HashSet<long>(ids);

                    var links = db.GetTable<Service>().Where(x => !hashSet.Contains(x.Id)).Select(x => new ServiceToCoachesLink() { CoachId = coachId, ServiceId = x.Id }).ToList();
                    foreach (var c in links)
                        await db.InsertWithIdentityAsync(c);
                }

                return coachId;

            },
            dbContext
            );
        }


        public override async Task<List<Coach>> GetByParams(Expression<Func<Coach, bool>> where, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                return (await
                    db.GetTable<Coach>()
                    .LoadWith(x => x.ServiceToCoachesLinks)                    
                    .Where(where)
                    .Where(x => x.IsDeleted == false)
                    .OrderBy(x => x.Name)
                    .ToListAsync()
                    )
                    .Select(x =>
                    {
                        var res = x;
                        res.Schedules = LoadSchedules(x.Id, db).ToList();
                        return res;
                    })
                    .ToList();
                
            }, dbContext);
        }
    }
}
