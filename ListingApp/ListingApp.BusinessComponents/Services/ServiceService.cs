using AutoMapper;
using ListingApp.BusinessContracts.Services;
using ListingApp.BusinessEntities.Models;
using ListingApp.DataAccess;
using ListingApp.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ListingApp.BusinessComponents.Services
{
	public class ServiceService: IServiceService
    {
		private readonly AppDbContext dbContext;

		private readonly IMapper mapper;

		public ServiceService(AppDbContext db, IMapper mapper)
		{
			this.dbContext = db;
			this.mapper = mapper;
		}

		public async Task<IList<ServiceModel>> GetAll()
		{
			return await this.dbContext.Services
				.Select(s => this.mapper.Map<Service, ServiceModel>(s))
				.OrderBy(s => s.Name)
				.ToListAsync();
		}

		public async Task<bool> IsSlugExists(string slug)
		{
			return await this.dbContext.Services.AnyAsync(s => string.Compare(s.Slug, slug, true) == 0);
		}

		public async Task<ServiceModel> GetBySlug(string slug)
		{
			var service = await this.dbContext.Services
				.FirstOrDefaultAsync(s => string.Compare(s.Slug, slug, true) == 0);

			if (service == null)
			{
				return null;
			}

			return this.mapper.Map<Service, ServiceModel>(service);
		}
	}
}
