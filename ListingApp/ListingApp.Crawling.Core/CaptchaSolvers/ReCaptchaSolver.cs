using DeathByCaptcha;
using ListingApp.Crawling.Core.Config;
using System;
using System.Collections;

namespace ListingApp.Crawling.Core.CaptchaSolvers
{
	public class ReCaptchaSolver
    {
		private const int CaptchaType = 4;

		private readonly HttpClient client;

		private readonly int invalidAttemptsCount;

		public ReCaptchaSolver(DeathByCaptchaConfig config)
		{
			this.client = new HttpClient(config.Username, config.Password);
			this.invalidAttemptsCount = config.InvalidAttemptsCount;
		}

		public string Solve(string googleKey, string url)
		{
			Captcha result = null;
			var attemptsCount = 0;
			var tokenParams = "{\"googlekey\": \"" + googleKey + "\", \"pageurl\": \"" + url + "\"}";
			var extData = new Hashtable
			{
				{ "type", CaptchaType },
				{ "token_params", tokenParams }
			};

			while(result == null)
			{
				Console.WriteLine("Attempt to solve captcha number {0}", ++attemptsCount);

				result = this.client.Decode(TimeSpan.FromMinutes(5).Seconds, extData);

				if(attemptsCount >= this.invalidAttemptsCount)
				{
					break;
				}
			}

			if(result != null)
			{
				Console.WriteLine("Solved: {0}", result.Text);
			}

			return result?.Text;
		}
    }
}
