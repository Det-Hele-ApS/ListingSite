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
				ConnectionString = "Server=tcp:finneskorte-prod.database.windows.net,1433;Initial Catalog=finneskorte-prod;Persist Security Info=False;User ID=Escortuser;Password=495€hj#13h?()€32;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
				StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=finneskorteprodstorage;AccountKey=7netpM+zWW6/6TnHL45p2uTHHm9+fQbVgIiTn7XaeJhUVieLZZ8a1PI0mrabaCJXBKhruXo+/0rw1S6MIL5ZCQ==;EndpointSuffix=core.windows.net",
				RequestCooldown = 7000,
				DeathByCaptchaConfig = new DeathByCaptchaConfig
				{
					InvalidAttemptsCount = 8,
					Username = "jlejlbka",
					Password = "Mikhalkou93"
				},
				Proxies = new[]
				{
					new ProxyConfig
					{
						Schema = "http",
						Host = "216.10.3.74",
						Port = 3199,
						Password = "u9hROcAgme",
						Username = "giufhna-j5jtb"
					},
					new ProxyConfig
					{
						Schema = "http",
						Host = "162.245.0.168",
						Port = 3199,
						Password = "u9hROcAgme",
						Username = "giufhna-j5jtb"
					},
					new ProxyConfig
					{
						Schema = "http",
						Host = "162.245.0.29",
						Port = 3199,
						Password = "u9hROcAgme",
						Username = "giufhna-j5jtb"
					},
					new ProxyConfig
					{
						Schema = "http",
						Host = "144.172.110.233",
						Port = 3199,
						Password = "u9hROcAgme",
						Username = "giufhna-j5jtb"
					},
					new ProxyConfig
					{
						Schema = "http",
						Host = "107.189.22.244",
						Port = 3199,
						Password = "u9hROcAgme",
						Username = "giufhna-j5jtb"
					}
				}
			});

			Console.WriteLine("Done.");
			Console.ReadKey(true);
		}
	}
}
