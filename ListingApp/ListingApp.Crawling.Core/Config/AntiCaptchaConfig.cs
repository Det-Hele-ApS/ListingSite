namespace ListingApp.Crawling.Core.Config
{
	public class AntiCaptchaConfig
    {
		public string Username { get; set; }

		public string Password { get; set; }

		public string ApiKey { get; set; }

		public int InvalidAttemptsCount { get; set; }
	}
}
