using System;
using System.Collections.Generic;
using System.Text;
using PropertyChanged;
using Shared.Dto.Interfaces;
using Shared.Attributes;

namespace Shared.Dto
{
    public class ScheduleDataDto: IUniqueDto
    {
        public virtual long Id { get; set; }
        public virtual long ScheduleId { get; set; }
        public virtual ScheduleIntervalEnum IntervalId { get; set; }
        public virtual Guid EventGuid { get; set; }
        public virtual bool IsAvialable { get; set; }
        public virtual string AvailabilityDescription { get; set; }
        public virtual long? DayNumber { get; set; }
        public virtual DateTime? DateOn { get; set; }

        [VmMeta(IsNullable = false)]
        public virtual DateTime BeginTime { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual DateTime EndTime { get; set; }
    }
}
