using ListingApp.Crawling.Core.Parsers;

namespace ListingApp.Crawling.Core
{
	public class Crawler
    {
		public Crawler(Config.Config config)
		{
			var parser = new RealEscortParser(config);
			parser.Parse().GetAwaiter().GetResult();
		}
    }
}
