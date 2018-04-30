using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dto
{
    public class BookingTemplateMetadataDto
    {
        public long Id { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsFortnightly { get; set; }        
        public List<BookingDto> BookingTemplates { get; set; }
    }
}
