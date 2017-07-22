using System.Collections.Generic;
using System.Linq;
using Mapping;

namespace Common
{
    public static class Extensions
    {
        public static IList<T1> ToList<T, T1>(this IEnumerable<T> value)
        {
            return value.Select(x => ObjectMapper.Map<T1>(x)).ToList();
        }

        public static IList<T1> ToList<T, T1>(this IList<T> value)
        {
            return value.Select(x => ObjectMapper.Map<T1>(x)).ToList();
        }       
    }
}
