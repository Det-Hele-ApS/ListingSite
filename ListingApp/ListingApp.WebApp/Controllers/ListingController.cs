﻿using ListingApp.BusinessContracts;
using ListingApp.BusinessContracts.Services;
using ListingApp.BusinessEntities.Models.Escort;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ListingApp.WebApp.Controllers
{
	public class ListingController : Controller
    {
		private const int PageSize = 20;

		private readonly IEscortTypeService escortTypeService;

		private readonly IServiceService serviceService;

		private readonly IEscortService escortService;

		private readonly IRegionService regionService;

		private readonly ICityService cityService;

		public ListingController(IEscortTypeService escortTypeService,
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

		public async Task<IActionResult> Index(string first, string second, string third, [FromQuery]int page = 1)
		{
			this.ViewBag.Types = await this.escortTypeService.GetAll();
			this.ViewBag.Services = await this.serviceService.GetAll();
			this.ViewBag.Regions = await this.regionService.GetAll();
			this.ViewBag.Cities = await this.cityService.GetAll();

			var type = await this.GetRouteParam(first, second, third, RouteParams.Type);
			var service = await this.GetRouteParam(first, second, third, RouteParams.Service);
			var city = await this.GetRouteParam(first, second, third, RouteParams.City);
			this.ViewBag.City = city;
			this.ViewBag.Type = type;
			this.ViewBag.Service = service;

			IList<ListingEscortModel> model;

			if(string.IsNullOrEmpty(type) && string.IsNullOrEmpty(service) && string.IsNullOrEmpty(city))
			{
				model = await this.escortService.GetAll();
			}
			else if(string.IsNullOrEmpty(type) && string.IsNullOrEmpty(city))
			{
				model = await this.escortService.GetByServiceName(service);
			}
			else if(string.IsNullOrEmpty(service) && string.IsNullOrEmpty(city))
			{
				model = await this.escortService.GetByEscortType(type);
			}
			else if(string.IsNullOrEmpty(type) && string.IsNullOrEmpty(service))
			{
				model = await this.escortService.GetByCity(city);
			}
			else if(string.IsNullOrEmpty(city))
			{
				model = await this.escortService.GetByEscortTypeAndService(type, service);
			}
			else if(string.IsNullOrEmpty(service))
			{
				model = await this.escortService.GetByEscortTypeAndCity(type, city);
			}
			else if(string.IsNullOrEmpty(type))
			{
				model = await this.escortService.GetByServiceAndCity(service, city);
			}
			else
			{
				model = await this.escortService.GetByAllFilters(type, service, city);
			}

			var pageModel = new ListingPageModel
			{
				CurrentPage = page,
				TotalPages = model.Count / PageSize + (model.Count % PageSize > 0 ? 1 : 0),
				Escorts = model.Skip((page - 1) * PageSize).Take(PageSize).ToList()
			};

			return this.View(pageModel);
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