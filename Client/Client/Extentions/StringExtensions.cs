using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Extentions
{
    public static class StringExtensions
    {
        public static string Append(this string obj, string addValue, string separator = ", ")
        {
            if (string.IsNullOrEmpty(obj))
                return addValue;

            return obj + (!string.IsNullOrEmpty(addValue) ? separator : "");
        }
    }
}
