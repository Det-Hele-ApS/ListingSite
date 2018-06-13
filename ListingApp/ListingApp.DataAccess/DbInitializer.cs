using ListingApp.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ListingApp.DataAccess
{
	public static class DbInitializer
	{
		public static async Task Initialize(IServiceProvider services)
		{
			using (var dbContext = services.GetRequiredService<AppDbContext>())
			{
				await dbContext.Database.MigrateAsync();
				await SeedCitiesAndRegions(dbContext);
				await SeedServices(dbContext);
				await SeedEscortTypes(dbContext);
			}
		}

		private static async Task SeedEscortTypes(AppDbContext dbContext)
		{
			if (!await dbContext.EscortTypes.AnyAsync())
			{
				await dbContext.EscortTypes.AddRangeAsync(new[]
				{
					new EscortType { Slug = "jenter", Name = "Jenter" },
					new EscortType { Slug = "transe", Name = "Transe" }
				});

				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedCitiesAndRegions(AppDbContext dbContext)
		{
			if (!await dbContext.Cities.AnyAsync() && !await dbContext.Regions.AnyAsync())
			{
				var frognerRegion = new Region
				{
					Name = "Frogner",
					Slug = "frogner"
				};

				var midhordlandRegion = new Region
				{
					Name = "Midhordland",
					Slug = "midhordland"
				};

				await dbContext.Cities.AddRangeAsync(new[]
				{
					new City
					{
						Slug = "oslo",
						Name = "Oslo",
						Region = frognerRegion
					},
					new City
					{
						Slug = "bergen",
						Name = "Bergen",
						Region = midhordlandRegion
					}
				});

				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedServices(AppDbContext dbContext)
		{
			if (!await dbContext.Services.AnyAsync())
			{
				await dbContext.Services.AddRangeAsync(new[]
				{
					new Service{Name = "Blowjob", Slug = "blowjob"},
					new Service{Name = "69", Slug = "69"},
					new Service{Name = "Anal sex", Slug = "anal"}
				});

				await dbContext.SaveChangesAsync();
			}
		}
	}
}
