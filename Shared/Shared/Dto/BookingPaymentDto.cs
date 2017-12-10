using Shared.Attributes;
using Shared.Dto.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dto
{
    public class BookingPaymentDto : IUniqueDto
    {
        public virtual long Id { get; set; }
        public virtual long BookingId { get; set; }
        [VmMeta(IsNullable = false)]
        public virtual PaymentTypeDto PaymentType { get; set; }
        public virtual bool IsPaid { get; set; }
        public virtual string PaymentOptions { get; set; }

    }
}
