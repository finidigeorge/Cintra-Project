using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Common.DtoMapping;
using Shared.Dto;

namespace Client.Security
{
    public class UserIdentity: IIdentity
    {
        public UserIdentity(string name, IList<UserRoleDtoUi> roles)
        {
            Roles = roles ?? new List<UserRoleDtoUi>();
            Name = name;            
        }

        public string Name { get; set; }
        public string AuthenticationType { get => "Custom auth"; }
        public bool IsAuthenticated => !string.IsNullOrEmpty(Name);

        public IList<UserRoleDtoUi> Roles { get; }

    }
}
