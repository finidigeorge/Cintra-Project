using System;
using System.Collections.Generic;
using System.Text;
using Shared.Dto.Interfaces;

namespace Shared.Dto
{
    public class UserRoleDto: IUniqueDto
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
    }
}
