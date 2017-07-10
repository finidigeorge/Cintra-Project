﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModels;
using LinqToDB;
using Repositories.Interfaces;
using Shared;
using Shared.Attributes;

namespace Repositories
{
    [PerScope]
    public class UserRepository : GenericRepository<User>, IUsersRepository
    {
        public async Task<IList<User>> GetByParamsWithRoles(Func<User, bool> where)
        {            
            using (var db = new CintraDB())
            {                
                return await Task.FromResult(
                    db.Users.LoadWith(x => x.user_roles).Where(where).ToList()
                );
            }
        }

        public async Task<bool> HasAdminAccess(string login)
        {
            var user = (await GetByParamsWithRoles(x => x.Login == login)).FirstOrDefault();
            return user?.user_roles.Name == enUserRoles.Administrator;
        }
    }
}
