using System;

namespace ListingApp.DataAccess.Entities
{
	public class EscortService
    {
		public Guid EscortId { get; set; }

		public Escort Escort { get; set; }

		public Guid ServiceId { get; set; }

		public Service Service { get; set; }
    }
}
