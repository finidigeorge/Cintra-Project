﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using LinqToDB;
using Repositories.Interfaces;
using Shared.Attributes;

namespace Repositories
{
    [PerScope]
    public class UserRolesRepository: GenericRepository<UserRoles>, IUserRolesRepository
    {
        public async Task<IList<UserRoles>> GetByParamsWithUsers(Func<UserRoles, bool> where)
        {
            using (var db = new CintraDB())
            {
                return await Task.FromResult(db.UserRoles.LoadWith(x => x.users).Where(where).ToList());
            }
        }
    }
}
