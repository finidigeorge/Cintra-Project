using System;
using System.Collections.Generic;
using System.Text;
using Shared.Dto.Interfaces;

namespace Shared.Dto
{
    public class CoachDto: IUniqueDto
    {        
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public List<ScheduleDto> Schedules { get; set; }
    }
}
