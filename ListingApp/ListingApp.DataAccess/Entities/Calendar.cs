using System;
using System.ComponentModel.DataAnnotations;

namespace ListingApp.DataAccess.Entities
{
	public class Calendar
    {
		[Key]
		public Guid Id { get; set; }

		public Guid EscortId { get; set; }

		public Escort Escort { get; set; }

		public Guid CityId { get; set; }

		public City City { get; set; }

		[Required]
		[DataType(DataType.Date)]
		public DateTime Date { get; set; }
    }
}
