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
                        vm.Client = _mapper.Map<ClientDto>(db.Client);
                        vm.Coach = _mapper.Map<CoachDto>(db.Coach);
                        vm.Horse = _mapper.Map<HorseDto>(db.Hor);
                        vm.Service = _mapper.Map<ServiceDto>(db.Service);
                        var payment = db.BookingPayments?.FirstOrDefault();
                        vm.BookingPayment = _mapper.Map<BookingPaymentDto>(db.BookingPayments?.FirstOrDefault());                        
                    });
                    conf.CreateMap<BookingDto, Booking>()
                        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())                        
                        .AfterMap((vm, db) =>
                        {
                            db.ClientId = vm.Client?.Id ?? 0;
                            db.Client = vm.Client != null ? _mapper.Map<Client>(vm.Client) : null;
                            db.CoachId = vm.Coach?.Id ?? 0;
                            db.HorseId = vm.Horse?.Id ?? 0;
                            db.Hor = vm.Horse != null ? _mapper.Map<Hors>(vm.Horse) : null;
                            db.ServiceId = vm.Service?.Id ?? 0;
                            db.Service = vm.Service != null ? _mapper.Map<Service>(vm.Horse) : null;

                            //need for checks only
                            vm.Coach = _mapper.Map<CoachDto>(db.Coach);
                            vm.Horse = _mapper.Map<HorseDto>(db.Hor);                            

                        });

                    conf.CreateMap<BookingPayments, BookingPaymentDto>()
                    .AfterMap((db, vm) =>
                    {
                        vm.PaymentType = _mapper.Map<PaymentTypeDto>(db.PaymentType);
                    });

                    conf.CreateMap<BookingPaymentDto, BookingPayments>()
                    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                    .AfterMap((db, vm) =>
                    {
                        db.BookingId = vm.BookingId;
                    });


                    conf.CreateMap<User, UserDto>()                    
                    .AfterMap((db, vm) =>
                    {
                        vm.UserRole = _mapper.Map<UserRoleDto>(db.UserRole);
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
                        vm.ScheduleData = _mapper.Map<List<ScheduleDataDto>>(db.SchedulesData);                        
                    });
                    conf.CreateMap<ScheduleDto, Schedule>()
                        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                        .AfterMap((vm, db) =>
                        {
                            db.SchedulesData = _mapper.Map<List<SchedulesData>>(vm.ScheduleData);                        
                        });
                   
                    conf.CreateMap<Coach, CoachDto>().AfterMap((db, vm) =>
                    {
                        vm.Schedules = _mapper.Map<List<ScheduleDto>>(db.Schedules);
                    });
                    conf.CreateMap<CoachDto, Coach>()
                        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                        .AfterMap((vm, db) =>
                        {
                            db.Schedules = _mapper.Map<List<Schedule>>(vm.Schedules);
                        });


                    conf.CreateMap<HorsesScheduleData, HorseScheduleDataDto>().AfterMap((db, vm) =>
                    {
                        vm.UnavailabilityType = (HorsesUnavailabilityEnum)db.UnavailabilityTypeId;
                    });

                    conf.CreateMap<HorseScheduleDataDto, HorsesScheduleData>().AfterMap((vm, db) =>
                    {
                        db.UnavailabilityTypeId = (long)vm.UnavailabilityType;
                    });


                    conf.CreateMap<Hors, HorseDto>()
                        .AfterMap((db, vm) => 
                        {
                            vm.HorseScheduleData = db.HorsesScheduleData?.Select(x => _mapper.Map<HorseScheduleDataDto>(x)).ToList() ?? new List<HorseScheduleDataDto>();
                        });

                    conf.CreateMap<HorseDto, Hors>()
                        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                        .AfterMap((vm, db) => 
                        {
                            db.HorsesScheduleData = vm.HorseScheduleData.Select(x => _mapper.Map<HorsesScheduleData>(x)).ToList() ?? new List<HorsesScheduleData>();
                        });

                    conf.CreateMap<Service, ServiceDto>().AfterMap((db, vm) =>
                    {                       
                        vm.Coaches = _mapper.Map<List<CoachDto>>(db.ServiceToCoachesLinks.Select(p => p.Coach));
                        vm.Horses= _mapper.Map<List<HorseDto>>(db.ServiceToHorsesLinks.Select(p => p.Hor));
                    });
                    conf.CreateMap<ServiceDto, Service>()
                        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                        .AfterMap((vm, db) =>
                        {
                            db.ServiceToCoachesLinks = vm.Coaches.Select(x => 
                                new ServiceToCoachesLink()
                                {                                    
                                    CoachId = x.Id,
                                    ServiceId = vm.Id
                                });

                            db.ServiceToHorsesLinks = vm.Horses.Select(x =>
                                new ServiceToHorsesLink()
                                {
                                    HorseId = x.Id,
                                    ServiceId = vm.Id
                                }
                                );
                        });

                    conf.CreateMap<ClientDto, Client>()
                        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

                    conf.CreateMap<PaymentTypeDto, PaymentTypes>()
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
