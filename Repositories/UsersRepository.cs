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
    public class UserRepository : GenericPreservableRepository<User>, IUsersRepository
    {
        public override async Task<List<User>> GetByParams(Func<User, bool> where)
        {            
            using (var db = new CintraDB())
            {                
                return await Task.FromResult(
                    db.Users.LoadWith(x => x.user_roles).Where(where).Where(x => x.IsDeleted == false).ToList()
                );
            }
        }
        
        public async Task<User> GetByLogin(string login)
        {
            using (var db = new CintraDB())
            {
                return (await GetByParams(x => x.Login == login)).FirstOrDefault();
            }
        }

        public async Task<bool> HasAdminAccess(string login)
        {
            var user = await GetByLogin(login);
            return user?.user_roles.Name == nameof(UserRolesEnum.Administrator);
        }
    }
}
