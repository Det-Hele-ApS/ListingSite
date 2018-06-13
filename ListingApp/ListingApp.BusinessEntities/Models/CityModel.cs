using System;

namespace ListingApp.BusinessEntities.Models
{
	public class CityModel
    {
		public Guid Id { get; set; }

		public string Slug { get; set; }

		public string Name { get; set; }

		public RegionModel Region { get; set; }
    }
}
