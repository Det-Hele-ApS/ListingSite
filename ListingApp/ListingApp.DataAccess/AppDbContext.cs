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

		public DbSet<Service> Services { get; set; }

		public DbSet<Region> Regions { get; set; }

		public DbSet<City> Cities { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
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
		}
    }
}
