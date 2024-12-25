using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Naspinski.FoodTruck.AdminWeb.Data;
using Naspinski.FoodTruck.AdminWeb.Models;
using Naspinski.FoodTruck.AdminWeb.Services;
using Naspinski.FoodTruck.AdminWeb.SignalR;
using Naspinski.FoodTruck.Data;
using System;

namespace Naspinski.FoodTruck.AdminWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration.GetSection("AzureSettings").Get<AzureSettings>());

            var elmah = Configuration.GetSection("ElmahSettings").Get<ElmahSettings>();
            services.AddElmahIo(o => { o.ApiKey = elmah.ApiKey; o.LogId = elmah.LogId; });

            // Add services to the container.
            var connectionString = Configuration.GetConnectionString("FoodTruckDb") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            services.AddDbContext<FoodTruckContext>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("FoodTruckDb"),
                    sqlServerOptionsAction: opts =>
                    {
                        opts.EnableRetryOnFailure(
                            maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorNumbersToAdd: null);
                    });
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, AdminEmailSender>();
            services.AddMvc();
            services.AddRazorPages().AddRazorRuntimeCompilation();

            services.AddTransient<Initializer>();

            services.AddSignalR();

            var origins = Configuration.GetSection("AzureSettings").GetValue<string>("CorsAddresses");
            services.AddCors(options =>
            {
                options.AddPolicy("SignalRPolicy",
                    builder => builder
                    .WithOrigins(origins.Split(',', System.StringSplitOptions.RemoveEmptyEntries))
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
            services.AddMvc(option => option.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Initializer initializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Settings/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<SignalHub>("/hub");
            });

            initializer.Seed().Wait();
        }
    }
}
