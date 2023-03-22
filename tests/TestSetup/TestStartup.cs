using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
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

        public TestStartup(IHostingEnvironment env, IHostingEnvironment appEnv)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Warning()
                .WriteTo.LiterateConsole()
                .CreateLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string databaseName = $"TandemBooking_{Guid.NewGuid().ToString().Replace("-", "_")}";
            _connectionString = Task.Run(async () =>
            {
                if (await LocalDbTools.CheckLocalDbExistsAsync(databaseName))
                {
                    await LocalDbTools.DestroyLocalDbDatabase(databaseName);
                }
                var connectionString = await LocalDbTools.CreateLocalDbDatabaseAsync(databaseName);
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
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
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                    .CreateScope())
                {
                    var connectionString = serviceScope.ServiceProvider.GetService<DbContextOptions>()
                        .GetExtension<SqlServerOptionsExtension>()
                        .ConnectionString;

                    LocalDbTools.DestroyLocalDbDatabase(connectionString).Wait();
                }
            });

            app.UseDeveloperExceptionPage();
            app.UseDatabaseErrorPage();

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
