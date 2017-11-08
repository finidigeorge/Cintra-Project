using System;
using Client.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.DtoMapping;
using RestApi;
using RestClient;

namespace Tests
{
    [TestClass]
    public class NotifyPropertyChangesTest
    {
        [TestMethod]
        public void FodyCommonNotifyPropertyChanges_Test()
        {
            var dto = new ScheduleDataDtoUi();
            var hasCalled = false;
            dto.PropertyChanged += (sender, args) => { hasCalled = true; };

            dto.DateOn = new DateTime(2000);            

            Assert.IsTrue(hasCalled);
        }

        [TestMethod]
        public void FodyClientNotifyPropertyChanges_Test()
        {
            var dto = new AuthVm(null, null);
            var hasCalled = false;
            dto.PropertyChanged += (sender, args) => { hasCalled = true; };

            dto.Username = "Test !";

            Assert.IsTrue(hasCalled);
        }
    }
}
