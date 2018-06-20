using ListingApp.BusinessContracts.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ListingApp.WebApp.Controllers
{
	public class ProfileController : Controller
    {
		private readonly IEscortTypeService escortTypeService;

		private readonly IServiceService serviceService;

		private readonly IEscortService escortService;

		private readonly IRegionService regionService;

		private readonly ICityService cityService;

		public ProfileController(IEscortTypeService escortTypeService,
			IServiceService serviceService,
			IEscortService escortService,
			IRegionService regionService,
			ICityService cityService)
		{
			this.escortTypeService = escortTypeService;
			this.serviceService = serviceService;
			this.escortService = escortService;
			this.regionService = regionService;
			this.cityService = cityService;
		}

        public async Task<IActionResult> Index(string id)
        {
			this.ViewBag.Types = await this.escortTypeService.GetAll();
			this.ViewBag.Services = await this.serviceService.GetAll();
			this.ViewBag.Regions = await this.regionService.GetAll();
			this.ViewBag.Cities = await this.cityService.GetAll();

			var escort = await this.escortService.GetBySlug(id);

			if(escort == null && int.TryParse(id, out int parsedId))
			{
				escort = await this.escortService.GetById(parsedId);
			}

            return View(escort);
        }
    }
}