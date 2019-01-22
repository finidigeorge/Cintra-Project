using DataModels;
using NUnit.Framework;
using Repositories;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Tests.Integration
{
    [TestFixture]
    public class HorseRepositoryTest: IntegrationTestBase
    {
        HorsesRepository _respository;

        [SetUp]
        public void HorseRepositoryTestSetUp()
        {
            _respository = ServiceProvider.GetService<IGenericRepository<Hors>>() as HorsesRepository;
        }

        [Test]
        public async Task HorseRepositoryCreate()
        {
            Assert.IsNotNull(_respository);
            var horse = new Hors() { Nickname = "TestHorse", MaxWorkingHours = 4 };

            var id = await _respository.Create(horse);
            Assert.AreNotEqual(0, id);
        }

        [Test]
        public async Task HorseRepositoryCreateMultiThreading()
        {
            var tasks = Enumerable.Range(0, 1000).Select(async x =>
            {
                var horse = new Hors() { Nickname = $"TestHorse{x}", MaxWorkingHours = 4 };
                var respository = ServiceProvider.GetService<IGenericRepository<Hors>>() as HorsesRepository;
                var id = await respository.Create(horse);
                Assert.AreNotEqual(0, id);
            });

            await Task.WhenAll(tasks);
        }
    }
}
