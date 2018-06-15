using ListingApp.Crawling.Core.Config;
using System;

namespace ListingApp.Crawling.Crawler
{
	class Program
	{
		static void Main(string[] args)
		{
			var crawler = new Core.Crawler(new Config
			{
				ConnectionString = "Data Source=.;Initial Catalog=ListingDB;User ID=sa;Password=123456;MultipleActiveResultSets=True;Trusted_Connection=False;Persist Security Info=True",
				DeathByCaptchaConfig = new DeathByCaptchaConfig
				{
					InvalidAttemptsCount = 8,
					Username = "jlejlbka",
					Password = "Mikhalkou93"
				}
			});

			Console.WriteLine("Done.");
			Console.ReadKey(true);
		}
	}
}
