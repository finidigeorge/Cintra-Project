using Shared.Attributes;
using Shared.Dto.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dto
{
    public class PaymentTypeDto: IUniqueDto
    {
        public virtual long Id { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual string Name { get; set; }
    }
}
