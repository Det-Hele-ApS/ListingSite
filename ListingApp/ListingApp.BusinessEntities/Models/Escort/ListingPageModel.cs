using System.Collections.Generic;

namespace ListingApp.BusinessEntities.Models.Escort
{
	public class ListingPageModel
    {
		public int CurrentPage { get; set; }

		public int TotalPages { get; set; }

		public IList<ListingEscortModel> Escorts { get; set; }
    }
}
