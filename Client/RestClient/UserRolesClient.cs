using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using RestClient;
using RestSharp;
using Shared.Dto;
using Shared.Interfaces;

namespace RestApi
{
    public class UserRolesClient: BaseRestApiClient<UserRoleDto>, IUserRolesController
    {
        public UserRolesClient() : base("userRoles")
        {
        }

        public async Task<IList<UserRoleDto>> GetByUser(string login)
        {
            return await SendRequest<List<UserRoleDto>>($"api/userRoles/getByUser/{login}");
        }
    }
}
