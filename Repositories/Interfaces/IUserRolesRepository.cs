using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataModels;

namespace Repositories.Interfaces
{
    public interface IUserRolesRepository: IGenericRepository<UserRoles>
    {
        Task<IList<UserRoles>> GetByParamsWithUsers(Func<UserRoles, bool> where);
    }
}