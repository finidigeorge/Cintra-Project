using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Security
{
    public class AnonymousIdentity: UserIdentity
    {
        public AnonymousIdentity(): base(null, null)
        {
        }
    }
}
