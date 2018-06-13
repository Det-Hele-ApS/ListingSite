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
	public class CityService: ICityService
    {
		private readonly AppDbContext dbContext;

		private readonly IMapper mapper;

		public CityService(AppDbContext db, IMapper mapper)
		{
			this.dbContext = db;
			this.mapper = mapper;
		}

		public async Task<IList<CityModel>> GetAll()
		{
			return await this.dbContext.Cities
				.Include(c => c.Region)
				.Select(c => this.mapper.Map<City, CityModel>(c))
				.ToListAsync();
		}

		public async Task<bool> IsSlugExists(string slug)
		{
			return await this.dbContext.Cities.AnyAsync(c => string.Compare(c.Slug, slug, true) == 0);
		}
	}
}
