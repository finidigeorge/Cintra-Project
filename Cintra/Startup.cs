using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shared;
using Shared.Attributes;

namespace Cintra
{
    public interface IStartupHandler
    {
        void Run();
    }

    public class Startup
    {        
        public static IEnumerable<Assembly> Assemblies;

        private static void _configureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(c => c.AddPolicy("*", b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().Build()));

            
            // Setup options with DI
            services.AddOptions();

            // configure lin2db
            LinqToDB.Data.DataConnection.DefaultSettings = new DbSettings(configuration.GetSection("LinqToDb"));

            var assemblyNames = new[] {                
                "Repositories",
                "Controllers",
            };

            Assemblies = assemblyNames.Select(s => new AssemblyName(s)).Select(s => Assembly.Load(s)).ToList();

            var instantiateOnStartup = new HashSet<Type>();
            var isolatedScope = new HashSet<Type>();

            foreach (var assembly in Assemblies)
            {
                foreach (var type in assembly.ExportedTypes)
                {
                    if (typeof(Controller).IsAssignableFrom(type))
                    {
                        services.AddTransient(type);
                        continue;
                    }

                    var ti = type.GetTypeInfo();
                    var interfaces = ti.ImplementedInterfaces.Where(i => !i.Namespace.StartsWith("System") && i != typeof(IStartupHandler));
                    interfaces = interfaces.Except(interfaces.SelectMany(i => i.GetTypeInfo().ImplementedInterfaces));

                    if (ti.GetCustomAttributes<PerScope>().Any())
                    {
                        if (interfaces.Count() > 1)
                        {
                            Console.WriteLine($"Multiple interface implementations found: {type} - [{string.Join(",", interfaces.Select(i => i.Name))}]");
                        }

                        foreach (var i in interfaces)
                        {
                            services.AddScoped(i, type);
                        }
                    }

                    if (ti.GetCustomAttributes<Chained>().Any())
                    {
                        if (interfaces.Count() > 1)
                        {
                            Console.WriteLine($"Multiple interface implementations found: {type} - [{string.Join(",", interfaces.Select(i => i.Name))}]");
                        }

                        foreach (var i in interfaces)
                        {
                            services.AddTransient(i, type);
                        }
                    }

                    if (ti.GetCustomAttributes<Singleton>().Any())
                    {
                        var attr = ti.GetCustomAttributes<Singleton>().First();

                        if (interfaces.Count() > 1)
                        {
                            Console.WriteLine($"Multiple interface implementations found: {type} - [{string.Join(",", interfaces.Select(i => i.Name))}]");
                        }

                        foreach (var i in interfaces)
                        {
                            services.AddSingleton(i, type);
                        }

                        if (attr.InstantiateOnStartup)
                        {
                            foreach (var i in interfaces)
                            {
                                instantiateOnStartup.Add(i);

                                if (attr.IsolateScope)
                                {
                                    isolatedScope.Add(i);
                                }
                            }
                        }
                    }

                }
            }

            // configure automapper
            //var allProfiles = Assemblies.SelectMany(a => a.ExportedTypes).Where(t => typeof(Profile).IsAssignableFrom(t));

            /*var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                //add your profiles (either resolve from container or however else you acquire them)
                foreach (var profile in allProfiles)
                {
                    cfg.AddProfile(profile);
                }
            });
            */

            var serviceProvider = services.BuildServiceProvider();
            //services.AddSingleton(mapperConfiguration.CreateMapper(serviceProvider.GetRequiredService));

            foreach (var singleton in instantiateOnStartup)
            {
                System.Console.WriteLine($"Instantiating {singleton}");

                IStartupHandler startupHandler = null;
                if (isolatedScope.Contains(singleton))
                {
                    // TODO: new scope is deliberately left un-disposed, need some kind of app dispose handler to clean it up
                    startupHandler = serviceProvider.CreateScope().ServiceProvider.GetRequiredService(singleton) as IStartupHandler;
                }
                else
                {
                    startupHandler = serviceProvider.GetRequiredService(singleton) as IStartupHandler;
                }

                startupHandler?.Run();
            }

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.TypeNameHandling = TypeNameHandling.Objects;                    
                    options.SerializerSettings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                });
            }
        

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddSingleton<IConfiguration>(Configuration);

            _configureServices(services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // configure port
            var a = app.ServerFeatures.Get<IServerAddressesFeature>();
            a.Addresses.Clear();
            a.Addresses.Add($"http://*:{Configuration.GetValue<int>("Port")}");

            //app.UseMiddleware();
            app.UseMvc();

        }
    }
}
