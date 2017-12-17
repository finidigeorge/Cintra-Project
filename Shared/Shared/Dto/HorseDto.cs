using System;
using System.Collections.Generic;
using System.Text;
using Shared.Attributes;
using Shared.Dto.Interfaces;

namespace Shared.Dto
{
    public class HorseDto: IUniqueDto
    {
        public virtual long Id { get; set; }

        [VmMeta(IsNullable = false)]
        public virtual string NickName { get; set; }        

        public virtual List<HorseScheduleDataDto> HorseScheduleData { get; set; }
}
}
