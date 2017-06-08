using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using Shared.Dto;
using Shared.Interfaces;

namespace RestApi
{
    public class UserRolesClient: BaseRestApiClient, IUserRolesController
    {
        public async Task<IEnumerable<UserRoleDto>> Get()
        {
            return await SendRequest<List<UserRoleDto>>("api/userRoles");
        }
    }
}
