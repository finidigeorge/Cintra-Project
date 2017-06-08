using System.Collections.Generic;
using LinqToDB.Configuration;
using Microsoft.Extensions.Configuration;

namespace Cintra
{
    public class ConnectionStringSettings : IConnectionStringSettings
    {
        public string ConnectionString { get; set; }
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public bool IsGlobal => false;
    }

    public class DbSettings : ILinqToDBSettings
    {
        private readonly string connectionString;

        public DbSettings(IConfigurationSection cfg)
        {
            connectionString = cfg.GetValue<string>("ConnectionString");
        }

        public IEnumerable<IDataProviderSettings> DataProviders
        {
            get { yield break; }
        }

        public string DefaultConfiguration => "SqlServer";
        public string DefaultDataProvider => "SqlServer";

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                yield return
                    new ConnectionStringSettings
                    {
                        Name = "DbContext",
                        ProviderName = "SQLite",
                        ConnectionString = connectionString
                    };
            }
        }
    }
}
