using System;
using System.Collections.Generic;
using System.Text;
using Shared.Dto.Interfaces;
using Shared.Attributes;

namespace Shared.Dto
{
    public class ScheduleDto : IUniqueDto
    {
        public virtual long Id { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual string Name { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual long CoachId { get; set; }
        public virtual List<ScheduleDataDto> ScheduleData { get; set; }

    }
}
