using DataModels;
using NUnit.Framework;
using Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class HorseRepositoryTest
    {
        readonly HorsesRepository _respository = new HorsesRepository();

        [Test]
        public async Task HorseRepositoryCreate()
        {
            var horse = new Hors() { Nickname = "TestHorse", MaxWorkingHours = 4 };

            var id = await _respository.Create(horse);
            Assert.AreNotEqual(0, id);
        }
    }
}
