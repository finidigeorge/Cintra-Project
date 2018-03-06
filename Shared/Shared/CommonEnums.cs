using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Shared
{
    public static class Constants
    {
        public static int BeginHour = 6;
        public static int BeginMinute = 0;

        public static int EndHour = 19;
        public static int EndMinute = 30;

        public static TimeSpan HorseBreakTime = TimeSpan.FromMinutes(60);
    }

    //TODO autogenerate from database
    public enum UserRolesEnum {
        Administrator,
        User
    }

    public enum ScheduleIntervalEnum
    {
        Weekly = 10,
        Daily = 20,       
    }

    public enum HorsesUnavailabilityEnum
    {
        OnHoliday,
        Sick,
        DayOff
    }

    public enum CoachRolesEnum
    {
        Coach = 1,
        [Description("Staff member")]
        StaffMember = 2
    }

    public static class enUserRoles
    {
        public const string Administrator = "Administrator";
        public const string User = "User";
    }

    public static class EnumTools
    {
        public static T ToEnumValue<T>(string value) where T : struct
        {
            var isSuccess = Enum.TryParse(value, out T result);

            if (isSuccess)
                return result;

            throw new Exception($"Can't parse string value {value} as a enum {typeof(T).Name}");
        }

        public static IEnumerable<CoachRolesEnum> GetCoachRolesEnumItems => Enum.GetValues(typeof(CoachRolesEnum)).Cast<CoachRolesEnum>();
    }



    public static class enKnownControllers
    {
        public const string AuthController = "auth";
        public const string BookingsController = "bookings";
        public const string CoachesController = "coaches";
        public const string ClientsController = "clients";
        public const string HorsesController = "horses";
        public const string HorsesScheduleDataController = "horsesScheduleData";
        public const string SchedulesController = "schedules";
        public const string PaymentTypesController = "paymentTypes";
        public const string SchedulesDataController = "schedulesData";        
        public const string ServicesController = "services";
        public const string UserRolesController = "userRoles";
        public const string UsersController = "users";
    }
}
