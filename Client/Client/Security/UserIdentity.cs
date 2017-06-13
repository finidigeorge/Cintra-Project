using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Shared.Dto;

namespace Client.Security
{
    public class UserIdentity: IIdentity
    {
        public UserIdentity(string name, IList<UserRoleDto> roles, JwtTokenDto token)
        {
            Roles = roles ?? new List<UserRoleDto>();
            Name = name;
            Token = token;
        }

        public string Name { get; set; }
        public string AuthenticationType { get => "Custom auth"; }
        public bool IsAuthenticated => !string.IsNullOrEmpty(Name);

        public IList<UserRoleDto> Roles { get; }

        public JwtTokenDto Token { get; }
    }
}
