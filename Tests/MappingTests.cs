using DataModels;
using Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Dto;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class MappingTests
    {

        private void TestMapping<T, T1>() where T: new()
        {
            var dto = new T();
            var result = ObjectMapper.Map<T1>(dto);
            dto = ObjectMapper.Map<T>(result);
        }

        [TestMethod]
        public void BookingsMapping()
        {
            var dto = new BookingDto()
            {
                BookingPayment = new BookingPaymentDto(),
                Client = new ClientDto(),
                Coach = new CoachDto() { Schedules = new List<ScheduleDto>() },
                Horse = new HorseDto(),
                Service = new ServiceDto()
            };

            var result = ObjectMapper.Map<Booking>(dto);
            result.BookingPayments = new List<BookingPayments>() { new BookingPayments() };
            dto = ObjectMapper.Map<BookingDto>(result);
        }

        [TestMethod]
        public void BookingPaymentMapping()
        {
            TestMapping<BookingPaymentDto, BookingPayments>();
        }

        [TestMethod]
        public void ClientMapping()
        {
            TestMapping<ClientDto, Client>();            
        }

        [TestMethod]
        public void CoachMapping()
        {
            var dto = new CoachDto() { Schedules = new List<ScheduleDto>() };
            var result = ObjectMapper.Map<Coach>(dto);
            dto = ObjectMapper.Map<CoachDto>(result);
        }

        [TestMethod]
        public void HorsesScheduleDataMapping()
        {
            TestMapping<HorseScheduleDataDto, HorsesScheduleData>();
        }

        [TestMethod]
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

        [TestMethod]
        public void PaymentTypeMapping()
        {
            TestMapping<PaymentTypeDto, PaymentTypes>();
        }

        [TestMethod]
        public void ScheduleMapping()
        {
            TestMapping<ScheduleDto, Schedule>();
        }

        [TestMethod]
        public void ScheduleDataMapping()
        {
            TestMapping<ScheduleDataDto, SchedulesData>();
        }

        [TestMethod]
        public void ServiceMapping()
        {
            TestMapping<ServiceDto, Service>();
        }

        [TestMethod]
        public void UserMapping()
        {            
            var dto = new UserDto() { UserRole = new UserRoleDto() };            
            var result = ObjectMapper.Map<User>(dto);
            dto = ObjectMapper.Map<UserDto>(result);
        }

        [TestMethod]
        public void UserRoleMapping()
        {
            TestMapping<UserRoleDto, UserRoles>();            
        }
        
    }
}
