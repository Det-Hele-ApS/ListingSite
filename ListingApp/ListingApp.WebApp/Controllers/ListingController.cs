using ListingApp.BusinessContracts;
using ListingApp.BusinessContracts.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ListingApp.WebApp.Controllers
{
	public class ListingController : Controller
    {
		private readonly IEscortTypeService escortTypeService;

		private readonly IServiceService serviceService;

		private readonly IEscortService escortService;
		
		private readonly ICityService cityService;

		public ListingController(IEscortTypeService escortTypeService,
			IServiceService serviceService,
			IEscortService escortService,
			ICityService cityService)
		{
			this.escortTypeService = escortTypeService;
			this.serviceService = serviceService;
			this.escortService = escortService;
			this.cityService = cityService;
		}

		public async Task<IActionResult> Index(string first, string second, string third)
		{
			this.ViewBag.Types = await this.escortTypeService.GetAll();
			this.ViewBag.Services = await this.serviceService.GetAll();
			this.ViewBag.Cities = await this.cityService.GetAll();

			this.ViewBag.Type = await this.GetRouteParam(first, second, third, RouteParams.Type);
			this.ViewBag.Service = await this.GetRouteParam(first, second, third, RouteParams.Service);
			this.ViewBag.City = await this.GetRouteParam(first, second, third, RouteParams.City);

			return this.View();
		}

		private async Task<string> GetRouteParam(string first, string second, string third, RouteParams paramType)
		{
			ISlugService service = paramType == RouteParams.City
				? this.cityService
				: paramType == RouteParams.Type
					? (ISlugService)this.escortTypeService
					: paramType == RouteParams.Service
						? this.serviceService
						: null;

			if(service == null)
			{
				throw new ArgumentException($"Unknown {nameof(paramType)}: {paramType}");
			}

			if (await service.IsSlugExists(first)) return first;
			if (await service.IsSlugExists(second)) return second;
			if (await service.IsSlugExists(third)) return third;

			return string.Empty;
			// throw new ArgumentException($"Unkown route: {first}/{second}/{third}");
		}

		private enum RouteParams
		{
			Type,
			Service,
			City
		}
	}
}