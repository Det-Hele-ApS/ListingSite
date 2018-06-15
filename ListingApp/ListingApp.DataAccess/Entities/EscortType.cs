using System;
using System.ComponentModel.DataAnnotations;

namespace ListingApp.DataAccess.Entities
{
	public class EscortType
    {
		[Key]
		public Guid Id { get; set; }

		[Required]
		public string Slug { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string ExternalName { get; set; }

		public int ExternalId { get; set; }
    }
}
