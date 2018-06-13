using System;
using System.ComponentModel.DataAnnotations;

namespace ListingApp.DataAccess.Entities
{
	public class Region
    {
		[Key]
		public Guid Id { get; set; }

		[Required]
		public string Slug { get; set; }

		[Required]
		public string Name { get; set; }

		// public ICollection
    }
}
