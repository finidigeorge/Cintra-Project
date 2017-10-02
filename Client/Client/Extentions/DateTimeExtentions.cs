using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Extentions
{
    public static class DateTimeExtentions
    {
        public static DateTime TruncateToYearStart(this DateTime dt)
        {
            return new DateTime(dt.Year, 1, 1);
        }
        public static DateTime TruncateToMonthStart(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }
        public static DateTime TruncateToDayStart(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day);
        }
        public static DateTime TruncateToHourStart(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
        }
        public static DateTime TruncateToMinuteStart(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
        }
        public static DateTime TruncateToSecondStart(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        }
    }
}
