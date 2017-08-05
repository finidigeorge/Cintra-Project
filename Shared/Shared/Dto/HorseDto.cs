using System;
using System.Collections.Generic;
using System.Text;
using Shared.Attributes;
using Shared.Dto.Interfaces;

namespace Shared.Dto
{
    public class HorseDto: IUniqueDto
    {
        public long Id { get; set; }

        [VmMeta(IsNullable = false)]
        public string NickName { get; set; }
    }
}
