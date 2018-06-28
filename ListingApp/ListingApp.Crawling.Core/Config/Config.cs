namespace ListingApp.Crawling.Core.Config
{
    public class Config
    {
		public AntiCaptchaConfig DeathByCaptchaConfig { get; set; }

		public string ConnectionString { get; set; }

		public string StorageConnectionString { get; set; }

		public int RequestCooldown { get; set; }

		public bool SkipCityParsing { get; set; }

		public bool SkipExisting { get; set; }

		public bool SkipPhoneCaptchaSolving { get; set; }

		public ProxyConfig[] Proxies { get; set; }
    }
}
