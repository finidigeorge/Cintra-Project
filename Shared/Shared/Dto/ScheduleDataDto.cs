using System;
using System.Collections.Generic;
using System.Text;
using PropertyChanged;
using Shared.Dto.Interfaces;

namespace Shared.Dto
{
    public class ScheduleDataDto: IUniqueDto
    {
        public long Id { get; set; }
        public long ScheduleId { get; set; }
        public ScheduleIntervalEnum IntervalId { get; set; }
        public bool IsAvialable { get; set; }
        public string AvailabilityDescription { get; set; }
        public long? DayNumber { get; set; }
        public DateTime? DateOn { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
