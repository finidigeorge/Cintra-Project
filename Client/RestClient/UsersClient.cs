using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Dto;

namespace RestClient
{
    public class UsersClient: BaseRestApiClient<UserDto>
    {
        public UsersClient() : base("users")
        {
        }
    }
}
