using AutoMapper;
using ListingApp.BusinessComponents.Services;
using ListingApp.BusinessContracts.Services;
using ListingApp.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;

namespace ListingApp.WebApp
{
	public class Startup
    {
		private const string ConnectionStringName = "DefaultConnection";

		private readonly string connectionString;

		public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
			this.connectionString = this.Configuration.GetConnectionString(ConnectionStringName);

			Log.Logger = new LoggerConfiguration()
				.CreateLogger();
		}

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                // options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

			services.AddDbContext<AppDbContext>(options => options.UseSqlServer(this.connectionString));
			services.AddMvc(); //.SetCompatibilityVersion(CompatibilityVersion.Version_2_0);
			services.AddAutoMapper();

			services.AddScoped<IEscortTypeService, EscortTypeService>();
			services.AddScoped<IServiceService, ServiceService>();
			services.AddScoped<IEscortService, EscortService>();
			services.AddScoped<IRegionService, RegionService>();
			services.AddScoped<ICityService, CityService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
			loggerFactory.AddSerilog();
			loggerFactory.AddFile("logs/log_{Date}.txt");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
			app.UseDirectoryBrowser(new DirectoryBrowserOptions
			{
				FileProvider = new PhysicalFileProvider(
					Path.Combine(Directory.GetCurrentDirectory(), "logs")),
				RequestPath = "/logs"
			});

            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
				routes.MapRoute(
					name: "escorts",
					template: "eskorte/{first?}/{second?}/{third?}",
					defaults: new { controller = "Listing", action = "Index", first = "", second = "", third = "" });
				routes.MapRoute(
					name: "profile",
					template: "profile/{id:guid}",
					defaults: new { controller = "Profile", action = "Index" });
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/");
            });
        }
    }
}
