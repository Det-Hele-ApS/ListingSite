using System.Collections.Generic;

namespace ListingApp.BusinessEntities.Models.Escort
{
	public class ListingEscortModel
    {
		public int ExternalId { get; set; }

		public string Name { get; set; }

		public string Slug { get; set; }

		public string EscortType { get; set; }

		public ImageModel MainImage { get; set; }

		public IEnumerable<ListingServiceModel> Services { get; set; }
	}
}
