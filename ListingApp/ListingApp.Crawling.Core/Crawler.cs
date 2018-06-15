using ListingApp.Crawling.Core.Parsers;

namespace ListingApp.Crawling.Core
{
	public class Crawler
    {
		public Crawler(Config.Config config)
		{
			var parser = new RealEscortParser(config.DeathByCaptchaConfig, config.ConnectionString);
			parser.Parse().GetAwaiter().GetResult();
		}
    }
}
