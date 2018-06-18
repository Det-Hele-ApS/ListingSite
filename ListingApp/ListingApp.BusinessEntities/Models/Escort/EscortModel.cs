using System;
using System.Collections.Generic;

namespace ListingApp.BusinessEntities.Models.Escort
{
	public class EscortModel
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string Type { get; set; }

		public string Description { get; set; }

		public IList<ImageModel> Images { get; set; }

		public Dictionary<string, string> Features { get; set; }

		public IList<ServiceModel> Services { get; set; }
    }
}
