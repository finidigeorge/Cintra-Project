using System;
using System.Collections.Generic;
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
