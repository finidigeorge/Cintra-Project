using DataModels;
using Mapping;
using NUnit.Framework;
using Shared.Dto;
using System.Collections.Generic;

namespace Tests
{
    [TestFixture]
    public class MappingTests
    {

        private void TestMapping<T, T1>() where T: new()
        {
            var dto = new T();
            var result = ObjectMapper.Map<T1>(dto);
            dto = ObjectMapper.Map<T>(result);
        }

        [Test]
        public void BookingsMapping()
        {
            var dto = new BookingDto()
            {
                BookingTemplateMetadata = new BookingTemplateMetadataDto() { BookingTemplates = new List<BookingDto>() { new BookingDto() { } } },
                BookingPayment = new BookingPaymentDto(),
                Clients = new List<ClientDto>() { new ClientDto() },
                Coaches = new List<CoachDto>() { new CoachDto() { Schedules = new List<ScheduleDto>() } },
                Horses = new List<HorseDto>() { new HorseDto() },
                Service = new ServiceDto()
            };

            var result = ObjectMapper.Map<Booking>(dto);

            Assert.IsNotNull(result.BookingPayments);

            result.BookingPayments = new List<BookingPayments>() { new BookingPayments() };
            dto = ObjectMapper.Map<BookingDto>(result);

            Assert.IsNotNull(dto.BookingPayment);
        }

        [Test]
        public void BookingPaymentMapping()
        {
            TestMapping<BookingPaymentDto, BookingPayments>();
        }

        [Test]
        public void ClientMapping()
        {
            TestMapping<ClientDto, Client>();            
        }

        [Test]
        public void CoachMapping()
        {
            var dto = new CoachDto() { Schedules = new List<ScheduleDto>() };
            var result = ObjectMapper.Map<Coach>(dto);
            dto = ObjectMapper.Map<CoachDto>(result);
        }

        [Test]
        public void HorsesScheduleDataMapping()
        {
            TestMapping<HorseScheduleDataDto, HorsesScheduleData>();
        }

        [Test]
        public void HorseMapping()
        {
            var dto = new HorseDto()
            {
                Id = 0, NickName = "BB",
                HorseScheduleData = new List<HorseScheduleDataDto>() {
                    new HorseScheduleDataDto() { Id = 1, UnavailabilityType = Shared.HorsesUnavailabilityEnum.DayOff}
                }
            };
            var result = ObjectMapper.Map<HorsesScheduleData>(dto);
            dto = ObjectMapper.Map<HorseDto>(result);
        }

        [Test]
        public void PaymentTypeMapping()
        {
            TestMapping<PaymentTypeDto, PaymentTypes>();
        }

        [Test]
        public void ScheduleMapping()
        {
            TestMapping<ScheduleDto, Schedule>();
        }

        [Test]
        public void ScheduleDataMapping()
        {
            TestMapping<ScheduleDataDto, SchedulesData>();
        }

        [Test]
        public void ServiceMapping()
        {
            TestMapping<ServiceDto, Service>();
        }

        [Test]
        public void UserMapping()
        {            
            var dto = new UserDto() { UserRole = new UserRoleDto() };            
            var result = ObjectMapper.Map<User>(dto);
            dto = ObjectMapper.Map<UserDto>(result);
        }

        [Test]
        public void UserRoleMapping()
        {
            TestMapping<UserRoleDto, UserRoles>();            
        }
        
    }
}
