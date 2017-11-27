using Common.DtoMapping;
using Shared.Dto;
using System;
using System.Windows.Media;

namespace Client.Controls.WpfScheduler
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime Start {get; set; }
        public DateTime End { get; set; }
        public bool AllDay { get; set; }
        public Brush Color { get; set; }
        public bool IsSelected { get; set; }

        public Event()
        {
            Id = Guid.NewGuid();
        }

        public void UpdateFromScheduleDtoData(ScheduleDataDto scheduleData) 
        {
            if (scheduleData.EventGuid != Guid.Empty)
                Id = scheduleData.EventGuid;

            Description = scheduleData.AvailabilityDescription;
            Start = scheduleData.BeginTime;
            End = scheduleData.EndTime;
            AllDay = false;
            Subject = scheduleData.AvailabilityDescription;            
        }

        public void MergeToScheduleDtoData(ref ScheduleDataDto scheduleData)
        {
            scheduleData.EventGuid = Id;
            scheduleData.AvailabilityDescription = Description;
            scheduleData.BeginTime = Start;
            scheduleData.EndTime = End;                                            
        }

        public void MergeToScheduleDtoData(ref ScheduleDataDtoUi scheduleData)
        {
            scheduleData.EventGuid = Id;
            scheduleData.AvailabilityDescription = Description;
            scheduleData.BeginTime = Start;
            scheduleData.EndTime = End;
        }


        public void UpdateFromBookingDataUi(BookingDtoUi booking)
        {
            if (booking.EventGuid != Guid.Empty)
                Id = booking.EventGuid;            
            Start = booking.BeginTime;
            End = booking.EndTime;
            AllDay = false;
            string paymentInfo = booking.BookingPayment.IsPaid ? "Paid" : "Not paid";

            Subject = $"Client: {booking.Client.Name}, Coach: {booking.Coach.Name}, Horse: {booking.Horse.NickName}, Service: {booking.Service.Name} \n" + 
                $"{paymentInfo}";
        }

        public void MergeToScheduleDtoData(ref BookingDtoUi booking)
        {
            booking.EventGuid = Id;

            booking.BeginTime = Start;
            booking.EndTime = End;
        }
    }
}
