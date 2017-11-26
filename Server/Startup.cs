using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Controllers.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Server.Middlewares;
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

        public IConfigurationRoot Configuration { get; }
        private SymmetricSecurityKey SigningKey => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("Jwt").GetValue<string>("SecretKey")));

        private void _configureServices(IServiceCollection services)
        {
            services.AddCors(c => c.AddPolicy("*", b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().Build()));
            
            // Setup options with DI
            services.AddOptions();

            // configure lin2db
            LinqToDB.Data.DataConnection.DefaultSettings = new DbSettings(Configuration.GetSection("LinqToDb"));
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
            LinqToDB.Common.Configuration.Linq.IgnoreEmptyUpdate = true;            

            var assemblyNames = new[] {                
                "Repositories",
                "Controllers"
            };

            //Set up DI container
            Assemblies = assemblyNames.Select(s => new AssemblyName(s)).Select(s => Assembly.Load(s)).ToList();

            var instantiateOnStartup = new HashSet<Type>();
            var isolatedScope = new HashSet<Type>();

            //load assemblies
            InitAssemblies(services, instantiateOnStartup, isolatedScope);            
            var serviceProvider = services.BuildServiceProvider();

            //run singletons on startup
            foreach (var singleton in instantiateOnStartup)
            {
                Console.WriteLine($"Instantiating {singleton}");

                IStartupHandler startupHandler;
                if (isolatedScope.Contains(singleton))                
                    startupHandler = serviceProvider.CreateScope().ServiceProvider.GetRequiredService(singleton) as IStartupHandler;                
                else                
                    startupHandler = serviceProvider.GetRequiredService(singleton) as IStartupHandler;
                
                startupHandler?.Run();
            }

            // Configure JwtTokenOptions
            var jwtAppSettingOptions = Configuration.GetSection("Jwt");            
            services.Configure<JwtTokenOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtTokenOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtTokenOptions.Audience)];
                options.ValidFor = TimeSpan.FromMinutes(Convert.ToInt32(jwtAppSettingOptions[nameof(JwtTokenOptions.ValidFor)]));
                options.SigningCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256);
            });

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.ObjectCreationHandling = ObjectCreationHandling.Replace;
                });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            /*services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireClaim("Admin"));
            });*/
        }

        private static void InitAssemblies(IServiceCollection services, HashSet<Type> instantiateOnStartup, HashSet<Type> isolatedScope)
        {
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
                    var interfaces = ti.ImplementedInterfaces
                        .Where(i => !i.Namespace.StartsWith("System") && i != typeof(IStartupHandler)).ToList();
                    interfaces = interfaces.Except(interfaces.SelectMany(i => i.GetTypeInfo().ImplementedInterfaces)).ToList();

                    if (ti.GetCustomAttributes<PerScope>().Any())
                    {
                        foreach (var i in interfaces)
                        {
                            services.AddScoped(i, type);
                        }
                    }

                    if (ti.GetCustomAttributes<Chained>().Any())
                    {
                        foreach (var i in interfaces)
                        {
                            services.AddTransient(i, type);
                        }
                    }

                    if (ti.GetCustomAttributes<Singleton>().Any())
                    {
                        var attr = ti.GetCustomAttributes<Singleton>().First();

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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddSingleton<IConfiguration>(Configuration);

            _configureServices(services);
        }

        private TokenValidationParameters GetJwtTokenValidationParameters()
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = SigningKey,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = Configuration.GetSection("Jwt").GetValue<string>("Issuer"),

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = Configuration.GetSection("Jwt").GetValue<string>("Audience"),

                // Validate the token expiry
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };
            return tokenValidationParameters;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddFile(Configuration.GetSection("Logging")["LogFile"]);
            loggerFactory.AddDebug();

            // configure port
            var a = app.ServerFeatures.Get<IServerAddressesFeature>();
            a.Addresses.Clear();
            a.Addresses.Add($"http://*:{Configuration.GetValue<int>("Port")}");
            
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = GetJwtTokenValidationParameters()
            });

            app.UseMiddleware(typeof(ExceptionHandlerMiddleware));
            app.UseMvc();

        }                       
    }
}
