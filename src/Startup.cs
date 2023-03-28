using System;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using Fujiy.ApplicationInsights.AspNetCore.SqlTrack;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
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
            services.AddDbContext<TandemBookingContext>(options =>
                {
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]);
                });

            services.AddDataProtection();

            services.AddTandemBookingAuthentication();
            services.AddTandemBookingAuthorization();
            services.AddMvc();
            services.AddRazorPages();
            services.AddServerSideBlazor();

            // Add configuration services.
            services.AddTransient(provider => Configuration.GetSection("BookingCoordinator").Get<BookingCoordinatorSettings>());
            services.AddTransient(provider => Configuration.GetSection("Nexmo").Get<NexmoSettings>());
            services.AddTransient(provider => Configuration.GetSection("Mail").Get<MailSettings>());
            // services.AddTransient(provider => Configuration.GetSection("IZettle").Get<IZettleSettings>());
            // services.AddTransient(provider => Configuration.GetSection("Vipps").Get<VippsSettings>());

            //add implementations of mail and nexmo services, which does communication with the outside world
            //they are interfaced because we want to provide different implementations for testing
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<INexmoService, NexmoService>();
            // services.AddSingleton<IZettleService>();
            // services.AddSingleton<VippsService>();
            
            services.AddBookingServices();
            services.AddScoped<LocalizationAttribute>();
            services.AddControllersWithViews().AddNewtonsoftJson();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILoggerFactory loggerFactory,
            IHostApplicationLifetime appLifetime,
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
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapRazorPages();
                endpoints.MapBlazorHub();
                endpoints.MapControllerRoute("default", "{lang=no}/{controller=Home}/{action=Index}/{id?}");
                //endpoints.MapFallbackToController("Index", "Controller");
                endpoints.MapFallbackToPage("admin/{*path}", "/_Host");
            });
        }
    }
}