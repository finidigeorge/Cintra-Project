using System;
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
                var query =
                    from r in db.UserRoles
                    join u in db.Users on r.Id equals u.RoleId into users
                    select new UserRoles() {Id = r.Id, Name = r.Name, users = users};

                return await Task.FromResult(
                    query.Where(where).ToList()
                );

                //could be done in the way like this, but the current implementation of linq2db does 2 queries to db
                //return await Task.FromResult(db.UserRoles.LoadWith(x => x.users).Where(where).ToList());

            }
        }
    }
}
