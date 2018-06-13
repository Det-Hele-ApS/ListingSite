using Microsoft.AspNetCore.Mvc;
using System;

namespace ListingApp.WebApp.Controllers
{
	public class ProfileController : Controller
    {
        public IActionResult Index(Guid id)
        {
            return View(id);
        }
    }
}