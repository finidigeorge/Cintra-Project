using Shared.Dto.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dto
{
    public class ClientDto: IUniqueDto
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual int Age { get; set; }
        public virtual string Weight { get; set; }
        public virtual string Height { get; set; }
        public virtual string ContactDetails { get; set; }
    }
}
