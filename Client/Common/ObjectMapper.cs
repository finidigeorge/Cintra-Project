using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Shared.Dto;
using System.Linq;
using Common.DtoMapping;

namespace Mapping
{
    public static class MapperExtensions
    {
        private static void IgnoreUnmappedProperties(TypeMap map, IMappingExpression expr)
        {
            foreach (string propName in map.GetUnmappedPropertyNames())
            {
                if (map.SourceType.GetProperty(propName) != null)
                {
                    expr.ForSourceMember(propName, opt => opt.Ignore());
                }
                if (map.DestinationType.GetProperty(propName) != null)
                {
                    expr.ForMember(propName, opt => opt.Ignore());
                }
            }
        }

        public static void IgnoreUnmapped(this IProfileExpression profile)
        {
            profile.ForAllMaps(IgnoreUnmappedProperties);
        }

        public static void IgnoreUnmapped(this IProfileExpression profile, Func<TypeMap, bool> filter)
        {
            profile.ForAllMaps((map, expr) =>
            {
                if (filter(map))
                {
                    IgnoreUnmappedProperties(map, expr);
                }
            });
        }

        public static void IgnoreUnmapped(this IProfileExpression profile, Type src, Type dest)
        {
            profile.IgnoreUnmapped((TypeMap map) => map.SourceType == src && map.DestinationType == dest);
        }

        public static void IgnoreUnmapped<TSrc, TDest>(this IProfileExpression profile)
        {
            profile.IgnoreUnmapped(typeof(TSrc), typeof(TDest));
        }
    }

    public class ObjectMapper
    {
        private static readonly IMapper _mapper;

        static ObjectMapper()
        {

            Mapper.Initialize(conf =>
                {
                    conf.CreateMissingTypeMaps = true;
                    conf.ReplaceMemberName("_", "");
                    
                    conf.CreateMap<CoachDto, CoachDtoUi>();
                    conf.CreateMap<HorseDto, HorseDtoUi>();
                    conf.CreateMap<ScheduleDataDto, ScheduleDataDtoUi>();
                    conf.CreateMap<ScheduleDto, ScheduleDtoUi>();
                    conf.CreateMap<ServiceDto, ServiceDtoUi>();
                    conf.CreateMap<UserDto, UserDtoUi>();
                    conf.CreateMap<UserRoleDto, UserRoleDtoUi>();
                    conf.IgnoreUnmapped();
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
