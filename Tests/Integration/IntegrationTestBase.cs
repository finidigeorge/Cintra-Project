using Cintra;
using DataModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Repositories;
using Repositories.Interfaces;
using System;

namespace Tests.Integration
{
    public abstract class IntegrationTestBase
    {
        protected ServiceProvider ServiceProvider;

        [SetUp]
        public void IntegrationTestBaseSetUp()
        {            
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.test.json")
                .Build();

            var services = new ServiceCollection();
            services.AddTransient<IGenericRepository<Hors>, HorsesRepository>();

            // Setup options with DI
            services.AddOptions();

            // configure linq2db
            LinqToDB.Data.DataConnection.DefaultSettings = new DbSettings(configuration.GetSection("LinqToDb"));
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
            LinqToDB.Common.Configuration.Linq.IgnoreEmptyUpdate = true;

            ServiceProvider = services.BuildServiceProvider();            
        }

        protected Hors GetHorse()
        {
            return new Hors() { Nickname = "TestHorse", MaxWorkingHours = 4 };
        }

    }
}
