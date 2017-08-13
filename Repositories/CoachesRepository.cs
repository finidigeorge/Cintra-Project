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
    public class CoachesRepository: GenericRepository<Coach>
    {
        private IEnumerable<Schedule> LoadSchedules(long coachId, CintraDB db)
        {
            return db.Schedules.LoadWith(x => x.data).Where(x => x.CoachId == coachId);            
        }


        public override async Task<List<Coach>> GetByParams(Func<Coach, bool> where)
        {
            using (var db = new CintraDB())
            {
                return await Task.FromResult(
                    db.Coaches.LoadWith(x => LoadSchedules(x.Id, db)).Where(where).ToList()
                );
            }
        }
    }
}