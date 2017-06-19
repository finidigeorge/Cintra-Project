using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Dto;

namespace Shared.Interfaces
{
    public interface IUserRolesController
    {
        Task<IList<UserRoleDto>> GetByUser(string login);
    }
}