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
        [VmMeta(IsNullable = false)]
        public virtual DateTime StartDate{ get; set; }
        [VmMeta(IsNullable = false)]
        public virtual DateTime EndDate { get; set; }
    }
}
