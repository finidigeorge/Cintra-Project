using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Db = DataModels;

namespace Controllers
{
    public class ObjectMapper
    {
        private static IMapper _mapper = null;

        static ObjectMapper()
        {

            Mapper.Initialize(conf =>
                {
                    conf.CreateMissingTypeMaps = true;
                    conf.ReplaceMemberName("_", "");                                   
                }
            );

            _mapper = Mapper.Instance;
        }

        public static TDestination Map<TDestination>(object source)
        {
            return _mapper.Map<TDestination>(source);
        }

        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            return _mapper.Map<TSource, TDestination>(source);
        }
    }
}
