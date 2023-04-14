using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
// using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using TandemBooking.Controllers;
using TandemBooking.Models;
using TandemBooking.Services;

namespace TandemBooking.Tests.TestSetup
{
    public class TestStartup
    {
        private string _connectionString;
        private string _db_name;


        public TestStartup(IWebHostEnvironment env, IWebHostEnvironment appEnv)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Warning()
                .WriteTo.Console()
                .CreateLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _db_name = $"TandemBooking_{Guid.NewGuid().ToString().Replace("-", "_")}";

            _connectionString = Task.Run(async () =>
            {
                if (await LocalDbTools.CheckLocalDbExistsAsync(_db_name))
                {
                    await LocalDbTools.DestroyLocalDbDatabase(_db_name);
                }
                var connectionString = await LocalDbTools.CreateLocalDbDatabaseAsync(_db_name);
                return connectionString;
            }).Result;

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<TandemBookingContext>(options =>
                {
                    options.UseSqlServer(_connectionString);
                });
            services.AddMvc(opts =>
            {
            });
            services.AddDataProtection();
            services.AddDatabaseDeveloperPageExceptionFilter();


            services.AddTandemBookingAuthentication();
            services.AddTandemBookingAuthorization();
            services.AddBookingServices();

            services.AddScoped<INexmoService, MockNexmoService>();
            services.AddScoped<IMailService, MockMailService>();

            services.AddTransient(_ => new BookingCoordinatorSettings
            {
                Name = "Tore",
                Email = "tore.birkeland@gmail.com",
                PhoneNumber = "4798463072"
            });

            //Add all -Controller types
            foreach (var type in typeof(BookingController).GetTypeInfo().Assembly.GetTypes())
            {
                var typeInfo = type.GetTypeInfo();
                if (typeInfo.IsClass && !typeInfo.IsAbstract && type.Name.EndsWith("Controller"))
                {
                    services.AddTransient(type);
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IHostApplicationLifetime appLifetime)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            loggerFactory.AddSerilog();

            // Migrate Database
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                //make sure database is migrated
                var context = serviceScope.ServiceProvider.GetService<TandemBookingContext>();
                context.Database.Migrate();
            }

            // Destroy database on exit
            appLifetime.ApplicationStopped.Register(() =>
            {
                LocalDbTools.DestroyLocalDbDatabase(_db_name).Wait();
            });

            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();

            app.UseStaticFiles();
            app.UseAuthentication();

        //     app.UseMvc(routes =>
        //     {
        //         routes.MapRoute(
        //             name: "default",
        //             template: "{controller=Home}/{action=Index}/{id?}");
        //     });
        }
    }
}
