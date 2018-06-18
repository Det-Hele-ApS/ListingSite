using ListingApp.BusinessContracts.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ListingApp.WebApp.Controllers
{
	public class ProfileController : Controller
    {
		private readonly IServiceService serviceService;

		private readonly IEscortService escortService;

		private readonly IRegionService regionService;

		private readonly ICityService cityService;

		public ProfileController(IServiceService serviceService,
			IEscortService escortService,
			IRegionService regionService,
			ICityService cityService)
		{
			this.serviceService = serviceService;
			this.escortService = escortService;
			this.regionService = regionService;
			this.cityService = cityService;
		}

        public async Task<IActionResult> Index(int id)
        {
			this.ViewBag.Services = await this.serviceService.GetAll();
			this.ViewBag.Regions = await this.regionService.GetAll();
			this.ViewBag.Cities = await this.cityService.GetAll();

			var escort = await this.escortService.GetById(id);
            return View(escort);
        }
    }
}