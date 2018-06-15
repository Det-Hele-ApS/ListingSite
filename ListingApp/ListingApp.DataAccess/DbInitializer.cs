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
					new EscortType { Slug = "jenter", Name = "Jenter", ExternalName = "girls", ExternalId = 5 },
					new EscortType { Slug = "transe", Name = "Transe", ExternalName = "tvts", ExternalId = 7 }
				});

				await dbContext.SaveChangesAsync();
			}
		}

		private static async Task SeedCitiesAndRegions(AppDbContext dbContext)
		{
			if (!await dbContext.Regions.AnyAsync())
			{
				var regions = new[]
				{
					 new Region
					 {
						 Name = "Akershus",
						 Slug = "akershus",
						 ExternalId = 2
					 },
					 new Region
					 {
						 Name = "Aust-Agder",
						 Slug = "aust-agder",
						 ExternalId = 9
					 },
					 new Region
					 {
						 Name = "Buskerud",
						 Slug = "buskerud",
						 ExternalId = 6
					 },
					 new Region
					 {
						 Name = "Finnmark",
						 Slug = "finnmark",
						 ExternalId = 19
					 },
					 new Region
					 {
						 Name = "Hedmark",
						 Slug = "hedmark",
						 ExternalId = 4
					 },
					 new Region
					 {
						 Name = "Hordaland",
						 Slug = "hordaland",
						 ExternalId = 12
					 },
					 new Region
					 {
						 Name = "Møre og Romsdal",
						 Slug = "moere-og-romsdal",
						 ExternalId = 14
					 },
					 new Region
					 {
						 Name = "Nordland",
						 Slug = "nordland",
						 ExternalId = 17
					 },
					 new Region
					 {
						 Name = "Oppland",
						 Slug = "oppland",
						 ExternalId = 5
					 },
					 new Region
					 {
						 Name = "Oslo",
						 Slug = "oslo",
						 ExternalId = 3
					 },
					 new Region
					 {
						 Name = "Rogaland",
						 Slug = "rogaland",
						 ExternalId = 11
					 },
					 new Region
					 {
						 Name = "Sogn og Fjordane",
						 Slug = "sogn-og-fjordane",
						 ExternalId = 13
					 },
					 new Region
					 {
						 Name = "Svalbard",
						 Slug = "svalbard",
						 ExternalId = 361
					 },
					 new Region
					 {
						 Name = "Telemark",
						 Slug = "telemark",
						 ExternalId = 8
					 },
					 new Region
					 {
						 Name = "Troms",
						 Slug = "troms",
						 ExternalId = 18
					 },
					 new Region
					 {
						 Name = "Trøndelag",
						 Slug = "troendelag",
						 ExternalId = 15
					 },
					 new Region
					 {
						 Name = "Vest-Agder",
						 Slug = "vest-agder",
						 ExternalId = 10
					 },
					 new Region
					 {
						 Name = "Vestfold",
						 Slug = "vestfold",
						 ExternalId = 7
					 },
					 new Region
					 {
						 Name = "Østfold",
						 Slug = "oestfold",
						 ExternalId = 1
					 }
				};

				await dbContext.Regions.AddRangeAsync(regions);
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
