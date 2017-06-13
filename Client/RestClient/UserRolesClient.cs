using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestClient;
using RestSharp;
using Shared.Dto;
using Shared.Interfaces;

namespace RestApi
{
    public class UserRolesClient: BaseRestApiClient, IUserRolesController
    {
        public async Task<IList<UserRoleDto>> Get(string login)
        {
            return await SendRequest<List<UserRoleDto>>("api/userRoles");
        }
    }
}
