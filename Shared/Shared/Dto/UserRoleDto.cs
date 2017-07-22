using System;
using System.Collections.Generic;
using System.Text;
using Shared.Dto.Interfaces;

namespace Shared.Dto
{
    public class UserRoleDto: IUniqueDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
