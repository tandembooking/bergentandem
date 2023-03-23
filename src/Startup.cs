using Fujiy.ApplicationInsights.AspNetCore.SqlTrack;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Globalization;
using TandemBooking.Models;
using TandemBooking.Services;
using TandemBooking.Attributes;

namespace TandemBooking
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<TandemBookingContext>(options =>
                {
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]);
                });

            services.AddDataProtection();

            services.AddTandemBookingAuthentication();
            services.AddTandemBookingAuthorization();
            services.AddMvc();
            services.AddTransient<ContentService>();
            // Add configuration services.
            services.AddTransient(provider => new BookingCoordinatorSettings
            {
                Name = Configuration["BookingCoordinator:Name"],
                PhoneNumber = Configuration["BookingCoordinator:PhoneNumber"],
                Email = Configuration["BookingCoordinator:Email"],
                DefaultPassengerFee = int.Parse(Configuration["BookingCoordinator:DefaultPassengerFee"])
            });

            services.AddTransient(provider => new NexmoSettings
            {
                Enable = Configuration["Nexmo:Enable"] == "True",
                ApiKey = Configuration["Nexmo:ApiKey"],
                ApiSecret = Configuration["Nexmo:ApiSecret"]
            });

            services.AddTransient(provider => new MailSettings
            {
                Enable = Configuration["Mail:Enable"] == "True",
                SmtpUser = Configuration["Mail:SmtpUser"],
                SmtpPassword = Configuration["Mail:SmtpPassword"],
                SmtpServer = Configuration["Mail:SmtpServer"],
                SmtpPort = int.Parse(Configuration["Mail:SmtpPort"]),
                FromName = Configuration["Mail:FromName"],
                FromAddress = Configuration["Mail:FromAddress"]
            });

            //add implementations of mail and nexmo services, which does communication with the outside world
            //they are interfaced because we want to provide different implementations for testing
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<INexmoService, NexmoService>();

            services.AddBookingServices();
     
            
            services.AddScoped<LocalizationAttribute>();

            //services.AddMvc(options =>
            //{
            //    options.Filters.Add(new LocalizationAttribute());
            //});

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IApplicationLifetime appLifetime,
            TelemetryClient telemetryClient
        )
        {
            //Force en-US culture to avoid date formatting issues in requests
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");



            //Add EF Core Application Insights
            loggerFactory.AddProvider(new AiEfCoreLoggerProvider(telemetryClient));

            //Flush log when application stops
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

            //Error handling
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Migrate Database
            try
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                    .CreateScope())
                {
                    serviceScope.ServiceProvider.GetService<TandemBookingContext>()
                        .Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Unable to migrate database");
            }

            //Force domain name
            if (!string.IsNullOrEmpty(Configuration["Server:Host"]))
            {
                var host = Configuration["Server:Host"];
                app.Use(async (context, next) =>
                {
                    if (context.Request.Host.Value == host)
                    {
                        await next();
                    }
                    else
                    {
                        var url = context.Request.Scheme + "://" + host + context.Request.Path +
                                  context.Request.QueryString;
                        context.Response.Redirect(url);
                    }
                });
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        "default",
            //        "{lang}/{controller=Home}/{action=Index}/{id?}");
            //});

            

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                        "default",
                        "{lang=no}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                        "old",
                        "{controller=Home}/{action=Index}/{id?}");
            });

            

        }
    }
}