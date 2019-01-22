using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using LinqToDB;
using LinqToDB.Data;
using Repositories.Interfaces;
using Shared.Attributes;

namespace Repositories
{
    [PerScope]
    public class UserRolesRepository: GenericPreservableRepository<UserRoles>, IUserRolesRepository
    {
        public async Task<IList<UserRoles>> GetByParamsWithUsers(Func<UserRoles, bool> where, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                return await db.GetTable<UserRoles>().LoadWith(x => x.Users).Where(where).Where(x => x.IsDeleted == false).AsQueryable().ToListAsync();
            }, dbContext);            
        }        
    }
}
