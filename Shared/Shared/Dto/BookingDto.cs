using Shared.Attributes;
using Shared.Dto.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dto
{
    public class BookingDto : IUniqueDto
    {
        public virtual long Id { get; set; }
        public virtual Guid EventGuid { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual DateTime DateOn { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual DateTime BeginTime { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual DateTime EndTime { get; set; }

        //only for transfer BookingDto -> BookingTemplate
        public virtual int DayOfWeek { get; set; }
        
        public virtual List<HorseDto> Horses { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual List<ClientDto> Clients { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual List<CoachDto> Coaches { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual ServiceDto Service { get; set; }
        public virtual BookingPaymentDto BookingPayment { get; set; }

        public virtual BookingTemplateMetadataDto BookingTemplateMetadata { get; set; }

        public virtual string ValidationErrors { get; set; }
        public virtual string ValidationWarnings { get; set; }

    }
}
