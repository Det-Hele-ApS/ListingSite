using System;
using System.ComponentModel.DataAnnotations;

namespace ListingApp.DataAccess.Entities
{
	public class Image
    {
		[Key]
		public Guid Id { get; set; }

		public bool IsPrimary { get; set; }

		[Required]
		public string Path { get; set; }

		public string SmallPath { get; set; }

		[Required]
		public string ExternalLink { get; set; }

		public int SortOrder { get; set; }

		public Guid EscortId { get; set; }

		public Escort Escort { get; set; }
    }
}
