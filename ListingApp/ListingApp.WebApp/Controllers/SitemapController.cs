using Microsoft.AspNetCore.Mvc;

namespace ListingApp.WebApp.Controllers
{
	public class SitemapController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}