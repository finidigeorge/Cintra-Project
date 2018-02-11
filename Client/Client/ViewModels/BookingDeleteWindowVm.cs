using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using Shared.Extentions;

namespace Client.ViewModels
{
    public class BookingDeleteWindowVm : BaseVm
    {
        public bool DeleteSelectedBooking { get; set; } = true;
        public bool DeleteRecurringBookings { get; set; }

        [DependsOn("DeleteSelectedBooking", "DeleteRecurringBookings")]
        public bool CanDelete => DeleteSelectedBooking || DeleteRecurringBookings;

        public DateTime RecurringStartDate { get; set; } = DateTime.Now.TruncateToDayStart().AddDays(1);
    }
}
