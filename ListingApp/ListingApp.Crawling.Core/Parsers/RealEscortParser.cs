using CsQuery;
using ListingApp.BusinessComponents.Services;
using ListingApp.BusinessContracts.Services;
using ListingApp.Crawling.Core.CaptchaSolvers;
using ListingApp.Crawling.Core.Config;
using ListingApp.DataAccess;
using ListingApp.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ListingApp.Crawling.Core.Parsers
{
	public class RealEscortParser
    {
		// Time in seconds we wait between requests.
		private const int RequestCooldown = 5000;

		private const string Host = "www.realescort.eu";

		private const string BaseUrl = "http://www.realescort.eu";

		// {url}/api/a/regions
		private const string ApiUrl = "http://www.realescort.eu/api/a/regions";

		// {url}/ads/category/{categoryName}/{regionId}/{region_slug}
		private const string RegionTemplate = "{0}/ads/category/{1}/{2}/{3}";

		private const string ProxyData = "http://giufhna-j5jtb:u9hROcAgme@216.10.3.74:3199";

		private const string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=finneskorteprodstorage;AccountKey=7netpM+zWW6/6TnHL45p2uTHHm9+fQbVgIiTn7XaeJhUVieLZZ8a1PI0mrabaCJXBKhruXo+/0rw1S6MIL5ZCQ==;EndpointSuffix=core.windows.net";

		private readonly ReCaptchaSolver captchaSolver;

		private readonly IAzureUploadService uploadService;

		private readonly AppDbContext dbContext;

		private readonly HttpClientHandler handler;

		private readonly HttpClient client;

		public RealEscortParser(DeathByCaptchaConfig config, string connectionString)
		{
			this.captchaSolver = new ReCaptchaSolver(config);
			this.uploadService = new AzureUploadService();

			var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
			optionsBuilder.UseSqlServer(connectionString);
			this.dbContext = new AppDbContext(optionsBuilder.Options);

			this.handler = new HttpClientHandler
			{
				AllowAutoRedirect = true,
				UseCookies = true,
				CookieContainer = new CookieContainer(),
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
				UseProxy = true,
				Proxy = new WebProxy
				{
					Address = new Uri("http://216.10.3.74:3199"),
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential("giufhna-j5jtb", "u9hROcAgme")					
				}
			};

			//if(File.Exists("cookie.txt"))
			//{
			//	var cookieValue = File.ReadAllText("cookie.txt");
			//	handler.CookieContainer.Add(new Cookie("cf_clearance", cookieValue, "/", ".realescort.eu"));
			//}

			this.client = new HttpClient(this.handler);

			Console.WriteLine(JsonConvert.SerializeObject(new { str = "Checking packages..." }).ToString());
		}

		public async Task Parse()
		{
			Console.WriteLine("Sending initial request to the home page.");
			await this.SendRequest(BaseUrl, HttpMethod.Get, BaseUrl);

			var categories = await this.dbContext.EscortTypes
				.Select(et => new
				{
					Slug = et.ExternalName,
					et.ExternalId
				}).ToListAsync();

			var regions = await this.dbContext.Regions
				.Select(r => new
				{
					r.Slug,
					r.ExternalId
				}).ToListAsync();

			foreach(var category in categories)
			{
				foreach(var region in regions)
				{
					var pageUrl = string.Format(RegionTemplate, BaseUrl, category.Slug, region.ExternalId, region.Slug);
					var pageNumber = 1;

					Console.WriteLine("Parsing: {0}", pageUrl);

					while (true)
					{
						Console.WriteLine("Page {0}", pageNumber);

						var content = this.GetJson(category.ExternalId, region.ExternalId, pageNumber);
						var response = await this.SendRequest(ApiUrl, HttpMethod.Post, pageUrl, content);
						var html = await response.Content.ReadAsStringAsync();

						if (response.StatusCode == HttpStatusCode.Forbidden)
						{
							Thread.Sleep(RequestCooldown);
							await this.Solve(html, ApiUrl);
						}
						else if(response.StatusCode == HttpStatusCode.NotFound)
						{
							break;
						}
						else
						{
							pageNumber++;
							var cq = CQ.Create(html);
							var links = cq.Find("a.img");
							foreach(var link in links)
							{
								var relUrl = link.GetAttribute("href");
								var girlUrl = $"{BaseUrl}/{relUrl}";

								Thread.Sleep(RequestCooldown);

								var girlResponse = await this.SendRequest(girlUrl, HttpMethod.Get, pageUrl);
								if(girlResponse.StatusCode == HttpStatusCode.OK)
								{
									try
									{
										Console.WriteLine("Parsing girl page {0}", girlUrl);
										var girlPage = await girlResponse.Content.ReadAsStringAsync();
										await this.HandleGirlPage(girlPage, girlUrl, category.ExternalId, region.ExternalId);
									}
									catch
									{
										Console.WriteLine("Error parsing girl: {0}", girlUrl);
									}
								}
							}
						}

						Thread.Sleep(RequestCooldown);
					}
				}
			}
		}

		private string GetJson(int categoryId, int regionId, int page)
		{
			return JsonConvert.SerializeObject(new
			{
				categoryId = categoryId,
				regionId = regionId,
				page = page,
				reload = false
			}).ToString();
		}

		private async Task<HttpResponseMessage> SendRequest(string url, HttpMethod method, string referer, string bodyContent = null, bool solve = true)
		{
			var request = new HttpRequestMessage
			{
				Method = method,
				RequestUri = new Uri(url)
			};

			this.AddHeaders(request, referer);

			if(!string.IsNullOrEmpty(bodyContent))
			{
				request.Content = new StringContent(bodyContent, Encoding.UTF8, "application/json");
			}

			var response = await this.client.SendAsync(request);

			Console.WriteLine("Response from {0} - Status Code = {1}", url, response.StatusCode);

			if(solve && response.StatusCode == HttpStatusCode.Forbidden)
			{
				Console.WriteLine("Forbidden.");
				Thread.Sleep(RequestCooldown);
				await this.Solve(await response.Content.ReadAsStringAsync(), url);
				Console.WriteLine("Solved. Resending.");
				Thread.Sleep(RequestCooldown);
				return await this.SendRequest(url, method, referer, bodyContent);
			}

			if(!solve && response.IsSuccessStatusCode)
			{
				
			}

			return response;
		}

		private void AddHeaders(HttpRequestMessage request, string refererUrl)
		{
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("image/webp"));
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("image/apng"));
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8));

			request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
			request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

			request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
			request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en", 0.9));

			request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
			request.Headers.Connection.Add("keep-alive");
			request.Headers.Host = Host;
			request.Headers.Add("Referer", refererUrl);
			request.Headers.Add("Upgrade-Insecure-Requests", "1");

			request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
			request.Headers.UserAgent.Add(new ProductInfoHeaderValue("AppleWebKit", "537.36"));
			request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Chrome", "64.0.3282.186"));
			request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Safari", "537.36"));
			request.Headers.Add("Upgrade-Insecure-Requests", "1");
		}

		private async Task Solve(string html, string url)
		{
			var cq = CQ.Create(html);
			var form = cq.Find("form#challenge-form").First();
			var postUrl = BaseUrl + form.Attr("action");

			string googleKey = string.Empty;

			var scripts = cq.Find("script");
			foreach (var script in scripts)
			{
				if (script.HasAttribute("data-sitekey"))
				{
					googleKey = script.GetAttribute("data-sitekey");
					var id = script.GetAttribute("data-ray");
					postUrl += "?id=" + id + "&g-recaptcha-response=";
					break;
				}
			}

			Console.WriteLine("Solving {0} - {1}", googleKey, url);
			var captchaAnswer = this.captchaSolver.Solve(googleKey, url, ProxyData);
			postUrl += captchaAnswer;

			Console.WriteLine("post url: {0}", postUrl);

			await this.SendRequest(postUrl, HttpMethod.Get, BaseUrl, solve: false);
		}

		private async Task HandleGirlPage(string html, string url, int categoryId, int regionId)
		{
			var cq = CQ.Create(html);
			var infoBlock = cq.Find("div.info").First();

			var urlParts = url.Split('/');
			var escortExternalId = int.Parse(urlParts[urlParts.Length - 2]);

			var escortTypeId = await this.dbContext.EscortTypes
				.Where(et => et.ExternalId == categoryId)
				.Select(et => et.Id)
				.FirstOrDefaultAsync();
			var name = infoBlock.Find("h2").First().Text();
			var description = cq.Find("div.description div.content p").First().Text();

			var isNewEscort = false;
			var escort = await this.dbContext.Escorts
				.Where(e => e.ExternalId == escortExternalId)
				.FirstOrDefaultAsync();

			if(escort == null)
			{
				escort = new Escort();
				isNewEscort = true;
			}

			escort.Name = name;
			escort.Description = description;
			escort.ExternalId = escortExternalId;
			escort.EscortTypeId = escortTypeId;

			if (isNewEscort)
			{
				await this.dbContext.Escorts.AddAsync(escort);
			}

			await this.dbContext.SaveChangesAsync();

			// Images
			try
			{
				var images = new List<Image>();
				var imagesDiv = cq.Find("div.images");
				var primaryImageLink = imagesDiv.Find("div.img a");
				var otherImageLinks = imagesDiv.Find("div.other div.xs-25 a");

				var imageOrder = 0;
				images.Add(new Image
				{
					IsPrimary = true,
					ExternalLink = primaryImageLink.Attr("href"),
					SmallPath = primaryImageLink.Find("img").Attr("src"),
					SortOrder = imageOrder++
				});

				foreach (var other in otherImageLinks)
				{
					images.Add(new Image
					{
						IsPrimary = false,
						ExternalLink = other.GetAttribute("href"),
						SmallPath = other.FirstChild.GetAttribute("src"),
						SortOrder = imageOrder++
					});
				}

				foreach (var image in images)
				{
					if (await this.dbContext.Images.AnyAsync(i => i.ExternalLink == image.ExternalLink))
					{
						continue;
					}

					var imageName = image.ExternalLink
						.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
						.Last();
					var smallImageName = image.SmallPath
						.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
						.Last();

					var imageUrl = $"{BaseUrl}/{image.ExternalLink}";
					var smallImageUrl = $"{BaseUrl}/{image.SmallPath}";

					var imageResponse = await this.SendRequest(imageUrl, HttpMethod.Get, url);
					var smallImageResponse = await this.SendRequest(smallImageUrl, HttpMethod.Get, url);

					var imageData = await imageResponse.Content.ReadAsByteArrayAsync();
					var smallImageData = await smallImageResponse.Content.ReadAsByteArrayAsync();

					var imageUri = await this.uploadService.Upload(StorageConnectionString, imageName, imageData);
					var smallImageUri = await this.uploadService.Upload(StorageConnectionString, smallImageName, smallImageData);

					// File.WriteAllBytes(imageName, await imageData.Content.ReadAsByteArrayAsync());
					// File.WriteAllBytes(smallImageName, await smallImageData.Content.ReadAsByteArrayAsync());

					await this.dbContext.Images.AddAsync(new Image
					{
						EscortId = escort.Id,
						ExternalLink = image.ExternalLink,
						IsPrimary = image.IsPrimary,
						Path = imageUri,
						SmallPath = smallImageUri,
						SortOrder = image.SortOrder
					});
				}

				await this.dbContext.SaveChangesAsync();
			}
			catch
			{
				Console.WriteLine("Error occured during parsing images.");
			}

			// Features
			var features = new List<EscortFeature>();
			var list = infoBlock.Find("div.list div.row");
			var featureOrder = 0;
			foreach(var row in list)
			{
				try
				{
					var title = row.FirstChild?.InnerText;
					var value = row.FirstChild?.NextElementSibling?.InnerText;

					if (!string.IsNullOrEmpty(value)
						&& !string.IsNullOrEmpty(title)
						&& !features.Any(f => f.FeatureName == title)
						&& !await this.dbContext.EscortFeatures.AnyAsync(f => f.FeatureName == title && f.EscortId == escort.Id))
					{
						features.Add(new EscortFeature
						{
							FeatureName = title,
							FeatureValue = value,
							EscortId = escort.Id,
							Order = featureOrder++
						});
					}
				}
				catch
				{
					continue;
				}
			}

			await this.dbContext.EscortFeatures.AddRangeAsync(features);

			// Services
			var services = new List<Service>();
			var servicesList = cq.Find("div.services.gives div.content div.dash-list span a");
			foreach(var serviceLink in servicesList)
			{
				try
				{
					var serviceName = serviceLink.InnerText;
					var externalLink = serviceLink.GetAttribute("href");
					var serviceDescription = serviceLink.GetAttribute("title");
					var linkParts = externalLink.Split('/');
					var slug = linkParts[linkParts.Length - 1];
					var externalId = int.Parse(linkParts[linkParts.Length - 2]);

					var service = new Service
					{
						ExternalId = externalId,
						Name = serviceName,
						Description = serviceDescription,
						Slug = slug
					};

					if (!await this.dbContext.Services.AnyAsync(s => s.ExternalId == externalId))
					{
						await this.dbContext.Services.AddAsync(service);
					}

					services.Add(service);
				}
				catch { }
			}

			foreach(var service in services)
			{
				var serv = await this.dbContext.Services
					.Include(s => s.EscortServices)
					.Where(s => s.ExternalId == service.ExternalId)
					.FirstOrDefaultAsync();

				if (!serv.EscortServices.Any(es => es.EscortId == escort.Id && es.ServiceId == serv.Id))
				{
					serv.EscortServices.Add(new DataAccess.Entities.EscortService
					{
						EscortId = escort.Id,
						ServiceId = serv.Id
					});
				}

				await this.dbContext.SaveChangesAsync();
			}

			await this.dbContext.SaveChangesAsync();
		}
    }
}
