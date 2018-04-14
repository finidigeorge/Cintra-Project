using System;
using System.Collections.Generic;
using System.Text;
using Shared.Dto.Interfaces;
using Shared.Attributes;

namespace Shared.Dto
{
    public class CoachDto: IUniqueDto
    {        
        public virtual long Id { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual string Name { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }

        public virtual CoachRolesEnum? CoachRole { get; set; }

        public virtual List<ScheduleDto> Schedules { get; set; } = new List<ScheduleDto>();
    }
}
