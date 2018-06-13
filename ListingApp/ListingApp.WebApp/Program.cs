using ListingApp.DataAccess;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ListingApp.WebApp
{
	public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;

				try
				{
					DbInitializer.Initialize(services).Wait();
				}
				catch (Exception ex)
				{
					var logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(ex, "An error occurred Initializing the DB.");
				}
			}

			host.Run();
		}

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
				.Build();
    }
}
