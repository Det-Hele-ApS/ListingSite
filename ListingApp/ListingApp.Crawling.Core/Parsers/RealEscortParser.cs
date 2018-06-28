using CsQuery;
using ListingApp.BusinessComponents.Services;
using ListingApp.BusinessContracts.Services;
using ListingApp.Crawling.Core.CaptchaSolvers;
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
		private const string Host = "www.realescort.eu";

		private const string BaseUrl = "http://www.realescort.eu";

		// {url}/api/a/regions
		private const string ApiUrl = "http://www.realescort.eu/api/a/regions";

		// {url}/ads/category/{categoryName}/{regionId}/{region_slug}
		private const string RegionTemplate = "{0}/ads/category/{1}/{2}/{3}";

		// Time in milliseconds we wait between requests.
		private readonly int requestCooldown;

		private readonly bool skipCityParsing;

		private readonly bool skipExisting;

		private readonly bool skipPhoneCaptchaSolving;

		private readonly string proxyData;

		private readonly string storageConnectionString;

		//private readonly ReCaptchaSolver captchaSolver;

		private readonly TwoCaptchaSolver captchaSolver;

		private readonly IAzureUploadService uploadService;

		private readonly AppDbContext dbContext;

		private readonly HttpClientHandler handler;

		private readonly HttpClient client;

		private readonly Random random;

		private string phoneCaptchaAnswer = string.Empty;

		public RealEscortParser(Config.Config config)
		{
			//this.captchaSolver = new ReCaptchaSolver(config.DeathByCaptchaConfig);
			this.captchaSolver = new TwoCaptchaSolver(config.DeathByCaptchaConfig);
			this.uploadService = new AzureUploadService();

			var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
			optionsBuilder.UseSqlServer(config.ConnectionString);
			this.dbContext = new AppDbContext(optionsBuilder.Options);

			this.random = new Random(this.GetCurrentUnixTime());
			var proxyIndex = random.Next(0, config.Proxies.Length - 1);
			var proxy = config.Proxies[proxyIndex];

			this.skipCityParsing = config.SkipCityParsing;
			this.skipExisting = config.SkipExisting;
			this.skipPhoneCaptchaSolving = config.SkipPhoneCaptchaSolving;
			this.requestCooldown = config.RequestCooldown; // random.Next(1000, this.requestCooldown);
			this.proxyData = $"{proxy.Schema}://{proxy.Username}:{proxy.Password}@{proxy.Host}:{proxy.Port}";
			this.storageConnectionString = config.StorageConnectionString;

			this.handler = new HttpClientHandler
			{
				AllowAutoRedirect = true,
				UseCookies = true,
				CookieContainer = new CookieContainer(),
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
				UseProxy = true,
				Proxy = new WebProxy
				{
					Address = new Uri($"{proxy.Schema}://{proxy.Host}:{proxy.Port}"),
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential(proxy.Username, proxy.Password)
				}
			};

			this.client = new HttpClient(this.handler);
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

			var regions = await this.dbContext.Regions.ToListAsync();

			if (!this.skipCityParsing)
			{
				await this.ParseCities(regions);
			}

			foreach (var category in categories)
			{
				foreach (var region in regions)
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
							this.WaitCooldown();
							await this.Solve(html, ApiUrl);
						}
						else if (response.StatusCode == HttpStatusCode.NotFound)
						{
							break;
						}
						else
						{
							pageNumber++;
							var cq = CQ.Create(html);
							var links = cq.Find("a.img");
							foreach (var link in links)
							{
								var relUrl = link.GetAttribute("href");
								var girlUrl = $"{BaseUrl}/{relUrl}";

								this.WaitCooldown();

								var girlResponse = await this.SendRequest(girlUrl, HttpMethod.Get, pageUrl);
								if (girlResponse.StatusCode == HttpStatusCode.OK)
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

		private int GetCurrentUnixTime()
		{
			var start = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return (DateTime.Now - start).Milliseconds;
		}

		private async Task<HttpResponseMessage> SendRequest(string url, HttpMethod method, string referer, string bodyContent = null, bool solve = true)
		{
			var request = new HttpRequestMessage
			{
				Method = method,
				RequestUri = new Uri(url)
			};

			this.AddHeaders(request, referer);

			if (!string.IsNullOrEmpty(bodyContent))
			{
				request.Content = new StringContent(bodyContent, Encoding.UTF8, "application/json");
			}

			var response = await this.client.SendAsync(request);

			Console.WriteLine("Response from {0} - Status Code = {1}", url, response.StatusCode);

			if (solve && response.StatusCode == HttpStatusCode.Forbidden)
			{
				Console.WriteLine("Forbidden.");
				this.WaitCooldown();
				await this.Solve(await response.Content.ReadAsStringAsync(), url);
				Console.WriteLine("Solved. Resending.");
				this.WaitCooldown();
				return await this.SendRequest(url, method, referer, bodyContent);
			}

			if (!solve && response.IsSuccessStatusCode)
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
			var captchaAnswer = await this.captchaSolver.Solve(googleKey, url, this.proxyData);
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
			var escortExternalSlug = urlParts.Last();

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

			if (escort == null)
			{
				escort = new Escort();
				isNewEscort = true;
			}

			if(this.skipExisting && !isNewEscort)
			{
				Console.WriteLine("Already exists. Skipping...");
				return;
			}

			if (isNewEscort || string.IsNullOrEmpty(escort.Phone) || escort.Phone.Contains("*"))
			{
				var phoneH3 = cq.Find("div.contact div.content div.row div.phone h3");
				var phoneNumber = phoneH3.Text().Substring(5, 13);

				var phoneLink = phoneH3.Find("a");
				var phoneUrl = phoneLink.Attr("href");

				if (!this.skipPhoneCaptchaSolving && phoneLink != null && !string.IsNullOrEmpty(phoneUrl))
				{
					var phoneModalRespone = await this.SendRequest($"{BaseUrl}/{phoneUrl}", HttpMethod.Get, url);
					var phoneModal = await phoneModalRespone.Content.ReadAsStringAsync();

					var div = CQ.Create(phoneModal).Find("div.verify div").FirstElement();
					var captchaKey = div.GetAttribute("recaptcha");

					var captchaUrl = $"{BaseUrl}/{phoneUrl}";
					Console.WriteLine("Solving captcha: {0} - {1} - {2}", captchaKey, captchaUrl, url);
					var captchaAnswer = await this.captchaSolver.Solve(captchaKey, url, this.proxyData);

					var response = await this.SendRequest($"{BaseUrl}/{phoneUrl}/data", HttpMethod.Post, url, JsonConvert.SerializeObject(new
					{
						recaptcha = captchaAnswer
					}).ToString());

					var json = await response.Content.ReadAsStringAsync();
					var phoneData = JsonConvert.DeserializeObject<PhoneData>(json);

					if (!string.IsNullOrEmpty(phoneData.Phone1))
					{
						phoneNumber = phoneData.Phone1;

						Console.WriteLine("Hidden phone number: {0}", phoneNumber);
					}
					else
					{
						Console.WriteLine("Invalid captcha.");
						if (!string.IsNullOrEmpty(phoneData.Error))
						{
							this.phoneCaptchaAnswer = string.Empty;
						}
					}
				}

				escort.Phone = phoneNumber;
			}

			escort.Name = name;
			escort.Description = description;
			escort.ExternalId = escortExternalId;
			escort.EscortTypeId = escortTypeId;
			escort.Slug = $"{escortExternalSlug}-{escortExternalId}";

			if (isNewEscort)
			{
				await this.dbContext.Escorts.AddAsync(escort);
			}

			await this.dbContext.SaveChangesAsync();

			await this.DownloadImages(cq, escort.Id, url);
			await this.ParseFeatures(infoBlock, escort.Id);
			await this.ParseServices(cq, escort.Id);
			await this.ParseCalendar(cq.Find("div.travelplan div.day div.location.dash-list"), escort.Id);

			await this.dbContext.SaveChangesAsync();
		}

		private async Task ParseCities(List<Region> regions)
		{
			foreach (var region in regions)
			{
				var citiesUrl = $"{BaseUrl}/modals/cities/{region.ExternalId}";
				var citiesResponse = await this.SendRequest(citiesUrl, HttpMethod.Get, BaseUrl);
				var citiesHtml = await citiesResponse.Content.ReadAsStringAsync();
				var lis = CQ.Create(citiesHtml).Find("div.content ul li");
				foreach (var li in lis)
				{
					var city = li.InnerText;
					if (!await this.dbContext.Cities.AnyAsync(c => c.Name == city && c.RegionId == region.Id))
					{
						await this.dbContext.Cities.AddAsync(new City
						{
							Name = city,
							RegionId = region.Id,
							Slug = city
								.ToLower()
								.Replace(' ', '-')
								.Replace("(", string.Empty)
								.Replace(")", string.Empty)
								.Replace("&#197;", "a")
								.Replace("&#216;", "o")
								.Replace("&#229;", "a")
								.Replace("&#230;", "a")
								.Replace("&#248;", "o")
						});
					}
				}
			}

			await this.dbContext.SaveChangesAsync();
		}

		private async Task DownloadImages(CQ cq, Guid escortId, string escortUrl)
		{
			try
			{
				var existingImages = await this.dbContext.Images.Where(i => i.EscortId == escortId).ToListAsync();

				var images = new List<Image>();
				var imagesDiv = cq.Find("div.images");
				var primaryImageLink = imagesDiv.Find("div.img a");
				var otherImageLinks = imagesDiv.Find("div.other div.xs-25 a");

				var imageOrder = 0;
				var primaryExternalLink = primaryImageLink.Attr("href");

				if (!existingImages.Any(ei => ei.ExternalLink == primaryExternalLink))
				{
					images.Add(new Image
					{
						IsPrimary = true,
						ExternalLink = primaryExternalLink,
						SmallPath = primaryImageLink.Find("img").Attr("src"),
						SortOrder = imageOrder++
					});
				}

				foreach (var other in otherImageLinks)
				{
					var externalLink = other.GetAttribute("href");

					if (!existingImages.Any(ei => ei.ExternalLink == externalLink))
					{
						images.Add(new Image
						{
							IsPrimary = false,
							ExternalLink = externalLink,
							SmallPath = other.FirstChild.GetAttribute("src"),
							SortOrder = imageOrder++
						});
					}
				}

				foreach (var image in images)
				{
					var imageName = image.ExternalLink
						.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
						.Last();
					var smallImageName = image.SmallPath
						.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
						.Last();

					var imageUrl = $"{BaseUrl}/{image.ExternalLink}";
					var smallImageUrl = $"{BaseUrl}/{image.SmallPath}";

					var imageResponse = await this.SendRequest(imageUrl, HttpMethod.Get, escortUrl);
					var smallImageResponse = await this.SendRequest(smallImageUrl, HttpMethod.Get, escortUrl);

					var imageData = await imageResponse.Content.ReadAsByteArrayAsync();
					var smallImageData = await smallImageResponse.Content.ReadAsByteArrayAsync();

					var imageUri = await this.uploadService.Upload(this.storageConnectionString, imageName, imageData);
					var smallImageUri = await this.uploadService.Upload(this.storageConnectionString, smallImageName, smallImageData);

					await this.dbContext.Images.AddAsync(new Image
					{
						EscortId = escortId,
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
		}

		private async Task ParseFeatures(CQ infoBlock, Guid escortId)
		{
			var existingFeatures = await this.dbContext.EscortFeatures.Where(f => f.EscortId == escortId).ToListAsync();

			var features = new List<EscortFeature>();
			var list = infoBlock.Find("div.list div.row");
			var featureOrder = 0;
			foreach (var row in list)
			{
				try
				{
					var title = row.FirstChild?.InnerText;
					var value = row.FirstChild?.NextElementSibling?.InnerText;

					if (!string.IsNullOrEmpty(value)
						&& !string.IsNullOrEmpty(title)
						&& !features.Any(f => f.FeatureName == title)
						&& !existingFeatures.Any(f => f.FeatureName == title))
					{
						features.Add(new EscortFeature
						{
							FeatureName = title,
							FeatureValue = value,
							EscortId = escortId,
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
		}

		private async Task ParseServices(CQ cq, Guid escortId)
		{
			var existingServices = await this.dbContext.Services.ToListAsync();

			var services = new List<Service>();
			var servicesList = cq.Find("div.services.gives div.content div.dash-list span a");
			foreach (var serviceLink in servicesList)
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

					if (!existingServices.Any(s => s.ExternalId == externalId))
					{
						await this.dbContext.Services.AddAsync(service);
					}

					services.Add(service);
				}
				catch
				{
					continue;
				}
			}

			foreach (var service in services)
			{
				var serv = await this.dbContext.Services
					.Include(s => s.EscortServices)
					.Where(s => s.ExternalId == service.ExternalId)
					.FirstOrDefaultAsync();

				if (!serv.EscortServices.Any(es => es.EscortId == escortId && es.ServiceId == serv.Id))
				{
					serv.EscortServices.Add(new DataAccess.Entities.EscortService
					{
						EscortId = escortId,
						ServiceId = serv.Id
					});
				}

				await this.dbContext.SaveChangesAsync();
			}
		}

		private async Task ParseCalendar(CQ days, Guid escortId)
		{
			var existingCalendar = await this.dbContext.Calendar.Where(c => c.EscortId == escortId).ToListAsync();

			var index = 0;
			var firstDate = DateTime.Now;

			foreach (var day in days)
			{
				var date = firstDate.AddDays(index++);
				foreach (var span in day.ChildNodes)
				{
					if (span.NodeName == "SPAN")
					{
						var regionId = 0;
						var regionName = string.Empty;
						var cities = string.Empty;
						var wholeRegion = false;

						foreach (var spanContent in span.ChildNodes)
						{
							if (spanContent.NodeName == "#text" && string.IsNullOrWhiteSpace(spanContent.NodeValue))
							{
								continue;
							}

							if (spanContent.NodeName == "#text")
							{
								if (!wholeRegion && !string.IsNullOrEmpty(regionName))
								{
									cities = spanContent.NodeValue.Trim();
								}
							}

							if (spanContent.NodeName == "A")
							{
								var href = spanContent.GetAttribute("href");
								if (href.StartsWith("/modals/cities"))
								{
									wholeRegion = true;
									regionId = int.Parse(href.Split('/').Last());
								}
								else
								{
									regionName = spanContent.InnerText.Trim(':');
								}
							}
						}

						IQueryable<Calendar> calendarRows;
						if (wholeRegion)
						{
							calendarRows = this.dbContext.Cities
								.Where(c => c.Region.ExternalId == regionId)
								.Select(c => new Calendar
								{
									CityId = c.Id,
									EscortId = escortId,
									Date = date
								});
						}
						else
						{
							var cityNames = cities.Split(',').Select(c => c.Trim());
							calendarRows = this.dbContext.Cities
								.Where(c => cityNames.Contains(c.Name))
								.Select(c => new Calendar
								{
									CityId = c.Id,
									EscortId = escortId,
									Date = date
								});
						}

						foreach (var calendar in await calendarRows.ToListAsync())
						{
							if (!existingCalendar.Any(c =>
								 c.CityId == calendar.CityId
								 && c.Date == calendar.Date))
							{
								await this.dbContext.AddAsync(calendar);
							}
						}
					}
				}
			}

			await this.dbContext.SaveChangesAsync();
		}

		private void WaitCooldown()
		{
			var cooldown = this.random.Next(1000, this.requestCooldown);
			Console.WriteLine("Cooldown: {0}", cooldown);
			Thread.Sleep(cooldown);
		}

		private class PhoneData
		{
			public string Phone1 { get; set; }

			public string Error { get; set; }
		}
	}
}
