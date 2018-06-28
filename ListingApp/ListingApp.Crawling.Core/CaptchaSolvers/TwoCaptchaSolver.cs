using ListingApp.Crawling.Core.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ListingApp.Crawling.Core.CaptchaSolvers
{
    public class TwoCaptchaSolver
    {
		private const string ApiUrl = "http://2captcha.com";

		private readonly HttpClient client;

		private readonly string key;

		private readonly int invalidAttemptsCount;

		public TwoCaptchaSolver(AntiCaptchaConfig config)
		{
			this.client = new HttpClient();
			this.key = config.Username;
			this.invalidAttemptsCount = config.InvalidAttemptsCount;


		}

		public async Task<string> Solve(string googleKey, string url, string proxy)
		{
			var apiUrl = $"{ApiUrl}/in.php?key=${this.key}&method=userrecaptcha&googlekey=${googleKey}&pageurl=${url}&proxy={proxy}";
			var requestContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("key", this.key),
				new KeyValuePair<string, string>("method", "userrecaptcha"),
				new KeyValuePair<string, string>("googlekey", googleKey),
				new KeyValuePair<string, string>("pageurl", url),
				new KeyValuePair<string, string>("proxy", proxy),
			});

			var request = await this.client.PostAsync(apiUrl, requestContent);
			var response = await request.Content.ReadAsStringAsync();

			var id = response.Split('|').Last();

			var result = "CAPCHA_NOT_READY";

			while (result == "CAPCHA_NOT_READY")
			{
				Thread.Sleep(20000);
				var resultRequestContent = new FormUrlEncodedContent(new[]
				{
					new KeyValuePair<string, string>("key", this.key),
					new KeyValuePair<string, string>("action", "get"),
					new KeyValuePair<string, string>("id", id)
				});

				var resultRequest = await this.client.PostAsync($"{ApiUrl}/res.php?", resultRequestContent);
				result = await resultRequest.Content.ReadAsStringAsync();
				Console.WriteLine(result);
			}

			return result.StartsWith("OK") ? result.Substring(3) : null;
		}
	}
}
