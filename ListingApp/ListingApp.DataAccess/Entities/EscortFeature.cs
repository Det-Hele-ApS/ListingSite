using System;
using System.ComponentModel.DataAnnotations;

namespace ListingApp.DataAccess.Entities
{
	public class EscortFeature
    {
		[Required]
		public string FeatureName { get; set; }

		[Required]
		public string FeatureValue { get; set; }

		public int Order { get; set; }

		[Required]
		public Guid EscortId { get; set; }

		public Escort Escort { get; set; }
    }
}
