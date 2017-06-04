using DbUp;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Helpers;
using DbUp.SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DbUpdater
{
    class Program
    {
        private static Dictionary<string, string> Parse(string parameters)
        {
            var paramDict = parameters.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            return
                paramDict.ToDictionary(
                    keySelector: param => param.Substring(0, param.IndexOf("=", StringComparison.Ordinal)),
                    elementSelector: param => param.Substring(param.IndexOf("=", StringComparison.Ordinal) + 1)
                );
        }

        static void Main(string[] args)
        {
            var scriptSource = args.Any(arg => arg.StartsWith("/scripts:")) ?
                args.First(arg => arg.StartsWith("/scripts:")).Replace("/scripts:", string.Empty).TrimEnd() :
                Path.Combine(Directory.GetCurrentDirectory(), "SqlScripts");

            var connectionString = args.FirstOrDefault(arg => arg.StartsWith("/cnString:"));

            if (connectionString == null)
            {
                throw new ArgumentNullException("connection string");
            }

            if (string.IsNullOrEmpty(scriptSource))
            {
                throw new ArgumentNullException("path");
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connection string");
            }

            if (Directory.Exists(scriptSource) == false)
            {
                Console.WriteLine($"Folder not found: {scriptSource}");
                return;
            }

            connectionString = connectionString.Replace("/cnString:", string.Empty).TrimEnd();

            var parameters = args.FirstOrDefault(arg => arg.StartsWith("/params:"));

            var variables = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(parameters))
            {
                parameters = parameters.Replace("/params:", string.Empty).TrimEnd();
                variables = Parse(parameters);
            }
            
            
            PerformUpgrade(args, connectionString, variables, scriptSource);
        }

        private static void PerformUpgrade(string[] args, string connectionString, Dictionary<string, string> variables, string scriptSource)
        {
            var sqlFilePaths = new DirectoryInfo(scriptSource).GetFiles("*.sql").OrderBy(f => f.Name);

            var scripts = new List<SqlScript>();

            foreach (var sqlFilePath in sqlFilePaths)
            {
                var scriptContents = File.ReadAllText(sqlFilePath.FullName);
                scripts.Add(new SqlScript(sqlFilePath.Name, scriptContents));
            }

            var upgradeLog = new Func<IUpgradeLog>(() => new ConsoleUpgradeLog());

            var connectionFactory = new Func<IConnectionManager>(() =>
            {
                var r = new SQLiteConnectionManager(connectionString);
                r.OperationStarting(new ConsoleUpgradeLog(), null);
                return r;
            });

            IJournal journal = new SQLiteTableJournal(connectionFactory, upgradeLog, "db_updates_log");

            if (args.Contains("/noJournal"))
            {
                journal = new NullJournal();
            }

            var upgradeBuilder = DeployChanges.To.SQLiteDatabase(connectionString)
                .JournalTo(journal)
                .WithScripts(scripts)
                .LogToConsole()
                .WithVariables(variables);

            upgradeBuilder.Configure((cfg) =>
            {
                cfg.ScriptExecutor.ExecutionTimeoutSeconds = 10000;
                cfg.VariablesEnabled = false;
            });

            var upgrader = upgradeBuilder.Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw result.Error;
            }
        }
    }
}