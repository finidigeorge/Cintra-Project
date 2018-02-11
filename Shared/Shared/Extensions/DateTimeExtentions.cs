using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Extentions
{
    public static class DateTimeExtentions
    {
        public static DateTime RoundUp(this DateTime dt, TimeSpan d)
        {
            return new DateTime(((dt.Ticks + d.Ticks / 2) / d.Ticks) * d.Ticks);
        }

        public static DateTime RoundDown(this DateTime dt, TimeSpan d)
        {
            return new DateTime((dt.Ticks / d.Ticks) * d.Ticks);
        }

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

        public static bool IsBetween(this DateTime dt, DateTime startDate, DateTime endDate)
        {
            return (dt >= startDate && dt <= endDate.AddSeconds(-1));
        }

        public static bool IsOverlap(DateTime startDate1, DateTime endDate1, DateTime startDate2, DateTime endDate2)
        {
            return !((startDate1 > endDate2.AddSeconds(-1)) || (startDate2 > endDate1.AddSeconds(-1)));
        }

        public static DateTime TruncateToCurrentHourStart(this DateTime dt)
        {
            var hour = DateTime.Now.Hour + 1;
            if (hour < Constants.BeginHour)
                hour = Constants.BeginHour;

            if (hour > 23)
                hour = 23;

            return new DateTime(dt.Year, dt.Month, dt.Day, hour, 0, 0);
        }

        public static DateTime TruncateToCurrentHourEnd(this DateTime dt)
        {
            var hour = DateTime.Now.Hour + 2;

            if (hour > 23)
                hour = 23;

            return new DateTime(dt.Year, dt.Month, dt.Day, hour, 0, 0);
        }

        public static DateTime TruncateToNextWeekday(this DateTime dt, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)dt.DayOfWeek + 7) % 7;
            return dt.AddDays(daysToAdd).TruncateToDayStart();
        }

        //return day of week numered from 0 to 6 started from MONDAY
        public static int ToEuropeanDayNumber(this DateTime dt)
        {
            return ((int)dt.DayOfWeek) - 1 >= 0 ? ((int)dt.DayOfWeek) - 1 : 6;
        }
    }
}
