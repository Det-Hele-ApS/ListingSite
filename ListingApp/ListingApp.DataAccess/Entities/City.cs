using System;
using System.ComponentModel.DataAnnotations;

namespace ListingApp.DataAccess.Entities
{
	public class City
    {
		[Key]
		public Guid Id { get; set; }

		[Required]
		public string Slug { get; set; }

		[Required]
		public string Name { get; set; }

		public Guid RegionId { get; set; }

		public Region Region { get; set; }
	}
}
