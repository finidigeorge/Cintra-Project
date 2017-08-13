using System;
using System.Collections.Generic;
using System.Text;
using Shared.Dto.Interfaces;

namespace Shared.Dto
{
    public class ScheduleDto : IUniqueDto
    {
        public long Id { get; set; }
        public long IntervalId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public List<ScheduleDataDto> ScheduleData { get; set; }

    }
}
