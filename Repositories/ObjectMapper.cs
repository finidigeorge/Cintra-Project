using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using DataModels;
using Shared;
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

                    conf.CreateMap<Booking, BookingDto>()                    
                    .AfterMap((db, vm) =>
                    {
                        vm.Client = _mapper.Map<ClientDto>(db.client);
                        vm.Coach = _mapper.Map<CoachDto>(db.coach);
                        vm.Horse = _mapper.Map<HorseDto>(db.hors);
                        vm.Service = _mapper.Map<ServiceDto>(db.service);
                        var payment = db.bookingpayments?.FirstOrDefault();
                        vm.BookingPayment = _mapper.Map<BookingPaymentDto>(db.bookingpayments?.FirstOrDefault());                        
                    });
                    conf.CreateMap<BookingDto, Booking>()
                        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())                        
                        .AfterMap((vm, db) =>
                        {
                            db.ClientId = vm.Client.Id;
                            db.CoachId = vm.Coach.Id;
                            db.HorseId = vm.Horse.Id;
                            db.ServiceId = vm.Service.Id;

                        });


                    conf.CreateMap<BookingPaymentDto, BookingPayments>()
                        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());


                    conf.CreateMap<User, UserDto>()                    
                    .AfterMap((db, vm) =>
                    {
                        vm.UserRole = _mapper.Map<UserRoleDto>(db.user_roles);
                    });
                    conf.CreateMap<UserDto, User>()
                        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                        .AfterMap((vm, db) =>
                        {
                            db.RoleId = vm.UserRole.Id;                        
                        });

                    conf.CreateMap<UserRoleDto, UserRoles>()
                        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

                    conf.CreateMap<SchedulesData, ScheduleDataDto>()
                        .ForMember(x => x.IntervalId, opt => opt.Ignore())
                        .AfterMap((db, vm) =>
                        {
                            vm.IntervalId = (ScheduleIntervalEnum)db.IntervalId;
                        });
                    conf.CreateMap<ScheduleDataDto, SchedulesData>()
                        .ForMember(x => x.IntervalId, opt => opt.Ignore())
                        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                        .AfterMap((vm, db) =>
                        {
                            db.IntervalId = (int)vm.IntervalId;
                        });

                    conf.CreateMap<Schedule, ScheduleDto>().AfterMap((db, vm) =>
                    {
                        vm.ScheduleData = _mapper.Map<List<ScheduleDataDto>>(db.data);                        
                    });
                    conf.CreateMap<ScheduleDto, Schedule>()
                        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                        .AfterMap((vm, db) =>
                        {
                            db.data = _mapper.Map<List<SchedulesData>>(vm.ScheduleData);                        
                        });

                    

                    conf.CreateMap<Coach, CoachDto>().AfterMap((db, vm) =>
                    {
                        vm.Schedules = _mapper.Map<List<ScheduleDto>>(db.schedules);
                    });
                    conf.CreateMap<CoachDto, Coach>()
                        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                        .AfterMap((vm, db) =>
                        {
                            db.schedules = _mapper.Map<List<Schedule>>(vm.Schedules);
                        });

                    conf.CreateMap<HorseDto, Hors>()
                        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

                    conf.CreateMap<ServiceDto, Service>()
                        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

                    conf.CreateMap<ClientDto, Client>()
                        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
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
