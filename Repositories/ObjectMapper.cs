using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using DataModels;
using Shared.Dto;


namespace Mapping
{
    public class ObjectMapper
    {
        private static readonly IMapper _mapper;

        static ObjectMapper()
        {

            Mapper.Initialize(conf =>
                {
                    conf.CreateMissingTypeMaps = true;
                    conf.ReplaceMemberName("_", "");
                    conf.CreateMap<User, UserDto>().AfterMap((db, vm) =>
                    {
                        vm.UserRole = _mapper.Map<UserRoleDto>(db.user_roles);
                    });
                    conf.CreateMap<UserDto, User>().AfterMap((vm, db) =>
                    {
                        db.RoleId = vm.UserRole.Id;
                    });

                    conf.CreateMap<Schedule, ScheduleDto>().AfterMap((db, vm) =>
                    {
                        vm.ScheduleData = _mapper.Map<List<ScheduleDataDto>>(db.data);
                    });
                    conf.CreateMap<ScheduleDto, Schedule>().AfterMap((vm, db) =>
                    {
                        db.data = _mapper.Map<List<SchedulesData>>(vm.ScheduleData);
                    });

                    conf.CreateMap<Coach, CoachDto>().AfterMap((db, vm) =>
                    {
                        vm.Schedules = _mapper.Map<List<ScheduleDto>>(db.schedules);
                    });
                    conf.CreateMap<CoachDto, Coach>().AfterMap((vm, db) =>
                    {
                        db.schedules = _mapper.Map<List<Schedule>>(vm.Schedules);
                    });
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
