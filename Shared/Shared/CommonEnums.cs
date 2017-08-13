using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
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

    public static class enUserRoles
    {
        public const string Administrator = "Administrator";
        public const string User = "User";
    }

    public static class EnumTools
    {
        public static T ToEnumValue<T>(string value) where T : struct
        {
            T result;
            var isSuccess = Enum.TryParse(value, out result);

            if (isSuccess)
                return result;

            throw new Exception($"Can't parse string value {value} as a enum {typeof(T).Name}");
        }
    }



    public static class enKnownControllers
    {
        public const string AuthController = "auth";
        public const string CoachesController = "coaches";
        public const string HorsesController = "horses";
        public const string SchedulesController = "schedules";
        public const string SchedulesDataController = "schedulesData";        
        public const string ServicesController = "services";
        public const string UserRolesController = "userRoles";
        public const string UsersController = "users";
    }
}
