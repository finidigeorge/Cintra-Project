using System;
using System.Collections.Generic;
using System.Text;
using Shared.Attributes;
using Shared.Dto.Interfaces;

namespace Shared.Dto
{
    public class UserDto: IUniqueDto
    {
        public virtual long Id { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual string Login { get; set; }
        public virtual string Password { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual string Name { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual Boolean NewPasswordOnLogin { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual UserRoleDto UserRole { get; set; }
        public virtual Boolean IsLocked { get; set; }

    }
}
