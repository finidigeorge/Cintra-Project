    using System;
using System.Collections.Generic;
using System.Text;
using Shared.Dto.Interfaces;
using Shared.Attributes;

namespace Shared.Dto
{
    public class ServiceDto: IUniqueDto
    {
        public virtual long Id { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual string Name { get; set; }

        public virtual long? LengthMinutes { get; set; }
        public virtual bool NoHorseRequired { get; set; }
        public virtual int MaxClients { get; set; }
        public virtual int MaxHorses { get; set; }
        public virtual int MaxCoaches { get; set; }
        public virtual DateTime? BeginTime { get; set; }
        public virtual DateTime? EndTime { get; set; }

        public virtual List<CoachDto> Coaches { get; set; } = new List<CoachDto>();
        public virtual List<HorseDto> Horses { get; set; } = new List<HorseDto>();
    }
}
