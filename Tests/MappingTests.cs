using DataModels;
using Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Dto;

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
        public void ClientMapping()
        {
            TestMapping<ClientDto, Client>();            
        }

        [TestMethod]
        public void CoachMapping()
        {
            TestMapping<CoachDto, Coach>();            
        }

        [TestMethod]
        public void HorseMapping()
        {
            TestMapping<HorseDto, Hors>();
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
