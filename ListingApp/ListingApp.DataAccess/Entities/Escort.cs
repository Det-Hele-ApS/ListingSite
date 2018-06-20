using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ListingApp.DataAccess.Entities
{
	public class Escort
    {
		[Key]
		public Guid Id { get; set; }

		[Required]
		public Guid EscortTypeId { get; set; }

		public EscortType EscortType { get; set; }

		[Required]
		public string Name { get; set; }

		public string Phone { get; set; }

		public string Email { get; set; }

		public ICollection<EscortFeature> EscortFeatures { get; set; }

		public ICollection<EscortService> EscortServices { get; set; }

		public ICollection<Image> Images { get; set; }

		public ICollection<Calendar> Calendar { get; set; }

		public string Description { get; set; }

		public int ExternalId { get; set; }
	}
}
