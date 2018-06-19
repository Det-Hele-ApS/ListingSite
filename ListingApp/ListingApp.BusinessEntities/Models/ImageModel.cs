namespace ListingApp.BusinessEntities.Models
{
	public class ImageModel
    {
		public bool IsPrimary { get; set; }

		public string Path { get; set; }

		public string SmallPath { get; set; }

		public int SortOrder { get; set; }
	}
}
