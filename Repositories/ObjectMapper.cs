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


                    conf.CreateMap<Booking, BookingTemplates>().AfterMap((s, d) =>
                    {
                        d.BookingsTemplateMetadata = s.BookingsTemplateMetadata;
                        d.Id = 0;
                        d.BookingTemplatesToCoachesLinks = s.BookingsToCoachesLinks?.Select(x => new BookingTemplatesToCoachesLink() { CoachId = x.CoachId, Coach = x.Coach }).ToList();
                        d.BookingTemplatesToClientsLinks = s.BookingsToClientsLinks?.Select(x => new BookingTemplatesToClientsLink() { ClientId = x.ClientId, Client = x.Client }).ToList();
                        d.BookingTemplatesToHorsesLinks = s.BookingsToHorsesLinks?.Select(x => new BookingTemplatesToHorsesLink() { HorseId = x.HorseId, Hor = x.Hor }).ToList();
                    });

                    conf.CreateMap<BookingTemplates, Booking>().AfterMap((s, d) =>
                    {                        
                        d.BookingsTemplateMetadata = s.BookingsTemplateMetadata;
                        d.Id = 0;
                        d.BookingsToCoachesLinks = s.BookingTemplatesToCoachesLinks?.Select(x => new BookingsToCoachesLink() { CoachId = x.CoachId, Coach = x.Coach }).ToList();
                        d.BookingsToClientsLinks = s.BookingTemplatesToClientsLinks?.Select(x => new BookingsToClientsLink() { ClientId = x.ClientId, Client = x.Client }).ToList();
                        d.BookingsToHorsesLinks = s.BookingTemplatesToHorsesLinks?.Select(x => new BookingsToHorsesLink() { HorseId = x.HorseId, Hor = x.Hor }).ToList();
                    });
                    
                    conf.CreateMap<BookingTemplateMetadataDto, BookingsTemplateMetadata>()
                        .AfterMap((vm, db) =>
                        {
                            var list1 = vm.BookingTemplates.Select(x => Map<Booking>(x)).ToList();
                            var list2 = list1.Select(x => Map<BookingTemplates>(x)).ToList();
                            db.BookingTemplates = list2;
                        });
                    conf.CreateMap<BookingsTemplateMetadata, BookingTemplateMetadataDto>()
                        .AfterMap((db, vm) =>
                        {
                            vm.BookingTemplates = db.BookingTemplates.Select(x => Map<BookingDto>(Map<BookingTemplates>(x))).ToList();
                        });

                    conf.CreateMap<Booking, BookingDto>()                    
                    .AfterMap((db, vm) =>
                    {
                        vm.Clients = db.BookingsToClientsLinks.Select(x => _mapper.Map<ClientDto>(x.Client)).ToList(); 
                        vm.Coaches = db.BookingsToCoachesLinks.Select(x => { var c = _mapper.Map<CoachDto>(x.Coach); c.ShowOnlyAssignedCoaches = x.ShowOnlyAssiignedCoaches;  return c; }).ToList();
                        vm.Horses = db.BookingsToHorsesLinks.Select(x => _mapper.Map<HorseDto>(x.Hor)).ToList(); 
                        vm.Service = _mapper.Map<ServiceDto>(db.Service);
                        var payment = db.BookingPayments?.FirstOrDefault();
                        vm.BookingPayment = _mapper.Map<BookingPaymentDto>(payment) ?? new BookingPaymentDto();  
                        vm.BookingTemplateMetadata = _mapper.Map<BookingTemplateMetadataDto>(db.BookingsTemplateMetadata);
                    });
                    conf.CreateMap<BookingDto, Booking>()
                        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())                        
                        .AfterMap((vm, db) =>
                        {                            
                            db.ServiceId = vm.Service?.Id ?? 0;
                            db.Service = vm.Service != null ? _mapper.Map<Service>(vm.Service) : null;
                            db.BookingPayments = vm.BookingPayment == null ? new List<BookingPayments>() : new List<BookingPayments>() { _mapper.Map<BookingPayments>(vm.BookingPayment) };

                            db.TemplateMetadataId = vm.BookingTemplateMetadata?.Id;
                            db.BookingsTemplateMetadata = vm.BookingTemplateMetadata != null ? _mapper.Map<BookingsTemplateMetadata>(vm.BookingTemplateMetadata) : null;

                            db.BookingsToCoachesLinks = vm.Coaches?.Select(x => new BookingsToCoachesLink() { CoachId = x.Id, BookingId = db.Id, Coach = _mapper.Map<Coach>(x), Booking = db, ShowOnlyAssiignedCoaches = x.ShowOnlyAssignedCoaches });
                            db.BookingsToClientsLinks = vm.Clients?.Select(x => new BookingsToClientsLink() { ClientId = x.Id, BookingId = db.Id, Client = _mapper.Map<Client>(x), Booking = db });
                            db.BookingsToHorsesLinks = vm.Horses?.Select(x => new BookingsToHorsesLink() { HorseId = x.Id, BookingId = db.Id, Hor = _mapper.Map<Hors>(x), Booking = db });

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
                        vm.CoachRole = (CoachRolesEnum?)db.CoachRoleId;
                    });
                    conf.CreateMap<CoachDto, Coach>()
                        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                        .AfterMap((vm, db) =>
                        {
                            db.Schedules = _mapper.Map<List<Schedule>>(vm.Schedules);
                            db.CoachRoleId = (long?)vm.CoachRole;
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
                            db.HorsesScheduleData = vm.HorseScheduleData?.Select(x => _mapper.Map<HorsesScheduleData>(x)).ToList() ?? new List<HorsesScheduleData>();
                        });

                    conf.CreateMap<Service, ServiceDto>().AfterMap((db, vm) =>
                    {                       
                        vm.Coaches = _mapper.Map<List<CoachDto>>(db.ServiceToCoachesLinks.Select(p => p.Coach).ToList());
                        vm.Horses= _mapper.Map<List<HorseDto>>(db.ServiceToHorsesLinks.Select(p => p.Hor).ToList());
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
