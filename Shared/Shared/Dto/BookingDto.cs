﻿using Shared.Attributes;
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

        [VmMeta(IsNullable = false)]
        public virtual HorseDto Horse { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual ClientDto Client { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual CoachDto Coach { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual ServiceDto Service { get; set; }
        public virtual BookingPaymentDto BookingPayment { get; set; }

    }
}
