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
        public virtual DateTime DateOn { get; set; }
        public virtual DateTime BeginTime { get; set; }
        public virtual DateTime EndTime { get; set; }

        public virtual HorseDto Horse { get; set; }
        public virtual ClientDto Client { get; set; }
        public virtual CoachDto Coach { get; set; }
        public virtual ServiceDto Service { get; set; }
        public virtual BookingPaymentDto BookingPayment { get; set; }

    }
}
