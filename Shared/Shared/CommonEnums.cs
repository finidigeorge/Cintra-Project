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
}
