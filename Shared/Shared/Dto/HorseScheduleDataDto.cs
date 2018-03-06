using System;
using Shared.Dto.Interfaces;
using Shared.Attributes;

namespace Shared.Dto
{
    public class HorseScheduleDataDto : IUniqueDto
    {
        public virtual long Id { get; set; }
        public virtual long HorseId { get; set; }
        public virtual HorsesUnavailabilityEnum UnavailabilityType { get; set; }

        public virtual long? DayOfWeek { get; set; }

        public virtual DateTime? StartDate { get; set; }        
        public virtual DateTime? EndDate { get; set; }
    }
}
