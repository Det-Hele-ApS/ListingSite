using AutoMapper;
using ListingApp.BusinessContracts.Services;
using ListingApp.BusinessEntities.Models;
using ListingApp.BusinessEntities.Models.Escort;
using ListingApp.DataAccess;
using ListingApp.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ListingApp.BusinessComponents.Services
{
	public class EscortService : IEscortService
	{
		private static readonly Expression<Func<Escort, ListingEscortModel>> ModelSelector =
			e =>
			 new ListingEscortModel
			 {
				 ExternalId = e.ExternalId.ToString(),
				 EscortType = e.EscortType.ExternalName,
				 Name = e.Name,
				 Slug = e.Slug,
				 MainImage = new ImageModel
				 {
					 Path = e.Images.FirstOrDefault(i => i.IsPrimary || i.SortOrder == e.Images.Min(im => im.SortOrder)).Path,
					 SmallPath = e.Images.FirstOrDefault(i => i.IsPrimary || i.SortOrder == e.Images.Min(im => im.SortOrder)).SmallPath
				 },
				 Services = e.EscortServices.Select(s => new ListingServiceModel
				 {
					 Name = s.Service.Name,
					 Slug = s.Service.Slug,
					 Description = s.Service.Description
				 })
			 };

		private static readonly Expression<Func<Escort, EscortModel>> ProfileSelector =
			e =>
			new EscortModel
			{
				Id = e.Id,
				Name = e.Name,
				Type = e.EscortType.Slug,
				Description = e.Description,
				PhoneNumber = e.Phone,
				Features = e.EscortFeatures.ToDictionary(ef => ef.FeatureName, ef => ef.FeatureValue),
				Images = e.Images.Select(i => new ImageModel
				{
					Path = i.Path,
					SmallPath = i.SmallPath,
					SortOrder = i.SortOrder
				}).OrderBy(i => i.SortOrder).ToList(),
				Services = e.EscortServices.Select(es => new ServiceModel
				{
					Id = es.Service.Id,
					Name = es.Service.Name,
					Description = es.Service.Description,
					Slug = es.Service.Slug
				}).ToList(),
				Calendar = e.Calendar.Select(c => new CalendarModel
				{
					Date = c.Date,
					City = new CityModel
					{
						Name = c.City.Name,
						Slug = c.City.Slug,
						Region = new RegionModel
						{
							Name = c.City.Region.Name,
							Slug = c.City.Region.Slug
						}
					}
				}).OrderBy(c => c.Date).ToList()
			};


		private readonly AppDbContext db;

		private readonly IMapper mapper;

		public EscortService(AppDbContext dbContext, IMapper mapper)
		{
			this.db = dbContext;
			this.mapper = mapper;
		}

		public async Task<IList<ListingEscortModel>> GetAll()
		{
			return await this.db.Escorts
				.Select(ModelSelector)
				.ToListAsync();
		}

		public async Task<EscortModel> GetById(int id)
		{
			return await this.db.Escorts
				.Where(e => e.ExternalId == id)
				.Select(ProfileSelector)
				.FirstOrDefaultAsync();
		}

		public async Task<EscortModel> GetBySlug(string slug)
		{
			return await this.db.Escorts
				.Where(e => string.Compare(e.Slug, slug, StringComparison.OrdinalIgnoreCase) == 0)
				.Select(ProfileSelector)
				.FirstOrDefaultAsync();
		}

		public async Task<IList<ListingEscortModel>> GetByAllFilters(string escortType, string serviceName, string city)
		{
			var today = DateTime.Now.Date;
			return await this.db.Escorts
				.Where(e =>
					string.Compare(e.EscortType.Slug, escortType, StringComparison.OrdinalIgnoreCase) == 0
					&& e.EscortServices.Any(es =>
						string.Compare(es.Service.Slug, serviceName, StringComparison.OrdinalIgnoreCase) == 0)
					&& e.Calendar.Any(c =>
						string.Compare(c.City.Slug, city, StringComparison.OrdinalIgnoreCase) == 0
						&& c.Date == today))
				.Select(ModelSelector)
				.ToListAsync();
		}

		public async Task<IList<ListingEscortModel>> GetByCity(string city)
		{
			var today = DateTime.Now.Date;
			return await this.db.Calendar
				.Where(c => 
					string.Compare(c.City.Slug, city, StringComparison.OrdinalIgnoreCase) == 0
					&& c.Date == today)
				.Select(c => c.Escort)
				.Select(ModelSelector)
				.ToListAsync();
		}

		public async Task<IList<ListingEscortModel>> GetByEscortType(string escortType)
		{
			return await this.db.Escorts
				.Where(e => e.EscortType.Slug == escortType)
				.Select(ModelSelector)
				.ToListAsync();
		}

		public async Task<IList<ListingEscortModel>> GetByEscortTypeAndCity(string escortType, string city)
		{
			var today = DateTime.Now.Date;
			return await this.db.Calendar
				.Where(c =>
					string.Compare(c.City.Slug, city, StringComparison.OrdinalIgnoreCase) == 0
					&& c.Date == today)
				.Select(c => c.Escort)
				.Where(e => string.Compare(e.EscortType.Slug, escortType, StringComparison.OrdinalIgnoreCase) == 0)
				.Select(ModelSelector)
				.ToListAsync();
		}

		public async Task<IList<ListingEscortModel>> GetByEscortTypeAndService(string escortType, string serviceName)
		{
			return await this.db.Escorts
				.Where(e => e.EscortServices.Any(es => 
					string.Compare(es.Service.Slug, serviceName, StringComparison.OrdinalIgnoreCase) == 0 
					&& e.EscortType.Slug == escortType))
				.Select(ModelSelector)
				.ToListAsync();
		}

		public async Task<IList<ListingEscortModel>> GetByServiceAndCity(string serviceName, string city)
		{
			var today = DateTime.Now.Date;
			return await this.db.Escorts
				.Where(e => 
					e.EscortServices.Any(es =>
						string.Compare(es.Service.Slug, serviceName, StringComparison.OrdinalIgnoreCase) == 0)
					&& e.Calendar.Any(c => 
						string.Compare(c.City.Slug, city, StringComparison.OrdinalIgnoreCase) == 0
						&& c.Date == today))
				.Select(ModelSelector)
				.ToListAsync();
		}

		public async Task<IList<ListingEscortModel>> GetByServiceName(string serviceName)
		{
			return await this.db.Escorts
				.Where(e => e.EscortServices.Any(es => 
					string.Compare(es.Service.Slug, serviceName, StringComparison.OrdinalIgnoreCase) == 0))
				.Select(ModelSelector)
				.ToListAsync();
		}
	}
}
