﻿using System;
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

                //assign all staff services to the new staff member
                if (isNew && entity.CoachRoleId == (int)Shared.CoachRolesEnum.StaffMember)
                {                   
                    var serviceToCoachesLinks = db.CoachRolesToServicesLink.Where(x => x.CoachRoleId == (int)Shared.CoachRolesEnum.StaffMember).Select(x => new ServiceToCoachesLink() { CoachId = coachId, ServiceId = x.ServiceId });
                    foreach (var c in serviceToCoachesLinks)
                        await db.InsertWithIdentityAsync(c);
                }                               

                return coachId;
            },
            dbContext
            );
        }


        public override async Task<List<Coach>> GetByParams(Func<Coach, bool> where, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                return await Task.FromResult(
                    db.Coaches
                    .LoadWith(x => x.ServiceToCoachesLinks)
                    .Where(where)                    
                    .Where(x => x.IsDeleted == false).Select(x =>
                    {
                        var res = x;
                        res.Schedules = LoadSchedules(x.Id, db).ToList();
                        return res;
                    }).ToList().OrderBy(x => x.Name)
                );
            }, dbContext);
        }
    }
}
