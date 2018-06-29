using ListingApp.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ListingApp.DataAccess
{
	public class AppDbContext: DbContext
    {
		public AppDbContext(DbContextOptions options): base(options)
		{
		}

		public DbSet<EscortType> EscortTypes { get; set; }

		public DbSet<Escort> Escorts { get; set; }

		public DbSet<EscortFeature> EscortFeatures { get; set; }

		public DbSet<Service> Services { get; set; }

		public DbSet<Image> Images { get; set; }

		public DbSet<Region> Regions { get; set; }

		public DbSet<City> Cities { get; set; }

		public DbSet<Calendar> Calendar { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<EscortFeature>()
				.HasKey(ef => new { ef.FeatureName, ef.EscortId });

			modelBuilder.Entity<EscortService>()
				.HasKey(es => new { es.EscortId, es.ServiceId });

			modelBuilder.Entity<EscortService>()
				.HasOne(es => es.Escort)
				.WithMany(e => e.EscortServices)
				.HasForeignKey(es => es.EscortId);

			modelBuilder.Entity<EscortService>()
				.HasOne(es => es.Service)
				.WithMany(s => s.EscortServices)
				.HasForeignKey(es => es.ServiceId);

			modelBuilder.Entity<Service>().HasIndex(s => s.Slug).IsUnique(true);
			modelBuilder.Entity<City>().HasIndex(c => c.Slug).IsUnique(true);
			modelBuilder.Entity<Escort>().HasIndex(e => e.ExternalId).IsUnique(true);
			// modelBuilder.Entity<Image>().HasIndex(i => i.EscortId).IsUnique(false);

			// modelBuilder.Entity<Calendar>().HasIndex(c => new { c.Date, c.CityId, c.EscortId }).IsUnique(true);
			// modelBuilder.Entity<Calendar>().HasIndex(c => c.CityId).IsUnique(false);
			// modelBuilder.Entity<Calendar>().HasIndex(c => c.EscortId).IsUnique(false);
		}
    }
}
