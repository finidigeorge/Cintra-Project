using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bindables;
using Client.Commands;
using Client.Extentions;

namespace Client.ViewModels
{
    public class ScheduleIntervalEditWindowVm: BaseVm
    {
        public ScheduleIntervalEditWindowVm()
        {
            EditScheduleIntervalCommand = new AsyncCommand<object>(async (x) =>
            {
                try
                {
                    IsEditSuccess = true;
                    
                }
                catch (Exception e)
                {
                    IsEditSuccess = false;
                    ErrorMessage = e.Message;
                }

            }, x => true);
        }

        public DateTime CurrentDate { get; set; } = DateTime.UtcNow.ToLocalTime().TruncateToDayStart();

        public DateTime StartTime { get; set; } = DateTime.UtcNow.ToLocalTime().TruncateToDayStart() + TimeSpan.FromHours(6);
        public DateTime EndTime { get; set; } = DateTime.UtcNow.ToLocalTime().TruncateToDayStart() + TimeSpan.FromHours(21);

        public bool IsEditSuccess { get; set; }

        public string ErrorMessage { get; set; }

        public IAsyncCommand EditScheduleIntervalCommand { get; set; }
    }
}
