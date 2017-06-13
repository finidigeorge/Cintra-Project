using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Client.Security
{
    public class UserPrincipal: IPrincipal
    {
        public bool IsInRole(string role)
        {
           return _identity?.Roles != null &&_identity.Roles.Any(x => x.Name == role);
        }

        private UserIdentity _identity;

        public UserIdentity Identity
        {
            get => _identity ?? new AnonymousIdentity();
            set => _identity = value;
        }

        IIdentity IPrincipal.Identity => Identity;
    }
}
