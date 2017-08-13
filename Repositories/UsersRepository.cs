using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModels;
using LinqToDB;
using Repositories.Interfaces;
using Shared;
using Shared.Attributes;
using Shared.Dto;

namespace Repositories
{
    [PerScope]
    public class UserRepository : GenericRepository<User>, IUsersRepository
    {
        public override async Task<List<User>> GetByParams(Func<User, bool> where)
        {            
            using (var db = new CintraDB())
            {                
                return await Task.FromResult(
                    db.Users.LoadWith(x => x.user_roles).Where(where).ToList()
                );
            }
        }
        
        public async Task<User> GetByLogin(string login)
        {
            using (var db = new CintraDB())
            {
                return await db.Users.FirstOrDefaultAsync(x => x.Login == login);                
            }
        }

        public async Task<bool> HasAdminAccess(string login)
        {
            var user = (await GetByParams(x => x.Login == login)).FirstOrDefault();
            return user?.user_roles.Name == nameof(UserRolesEnum.Administrator);
        }
    }
}
