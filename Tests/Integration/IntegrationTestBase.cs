using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.Integration
{
    public abstract class IntegrationTestBase
    {
        [SetUp]
        public void IntegrationTestBaseSetUp()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionStringsAppSettings = new ConnectionStringsAppSettings();
            configuration.GetSection("ConnectionStrings").Bind(connectionStringsAppSettings);            
        }

    }
}
