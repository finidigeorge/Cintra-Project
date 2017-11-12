using System;
using System.Collections.Generic;
using System.Text;
using Shared.Dto.Interfaces;

namespace Shared.Dto
{
    public class ScheduleDto : IUniqueDto
    {
        public virtual long Id { get; set; }        
        public virtual string Name { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual long CoachId { get; set; }
        public virtual List<ScheduleDataDto> ScheduleData { get; set; }

    }
}
