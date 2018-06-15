using System;

namespace ListingApp.BusinessEntities.Models
{
	public class ServiceModel
    {
		public Guid Id { get; set; }

		public string Slug { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }
	}
}
