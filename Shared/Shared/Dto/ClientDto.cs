using Shared.Attributes;
using Shared.Dto.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dto
{
    public class ClientDto: IUniqueDto
    {
        public virtual long Id { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual string Name { get; set; }
        public virtual string Email { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual string Phone { get; set; }
        public virtual string Age { get; set; }
        public virtual string Weight { get; set; }
        public virtual string Height { get; set; }
        public virtual string ContactDetails { get; set; }
    }
}
