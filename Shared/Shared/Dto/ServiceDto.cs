using System;
using System.Collections.Generic;
using System.Text;
using Shared.Dto.Interfaces;
using Shared.Attributes;

namespace Shared.Dto
{
    public class ServiceDto: IUniqueDto
    {
        public virtual long Id { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual string Name { get; set; }        
    }
}
