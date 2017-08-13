using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DataModels;

namespace Repositories.Interfaces
{
    public interface IUsersRepository : IGenericRepository<User>
    {
        Task<User> GetByLogin(string login);

        Task<bool> HasAdminAccess(string login);
    }
}    
