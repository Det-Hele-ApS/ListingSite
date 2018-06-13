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
	public class RegionService: IRegionService
    {
		private readonly AppDbContext dbContext;

		private readonly IMapper mapper;

		public RegionService(AppDbContext db, IMapper mapper)
		{
			this.dbContext = db;
			this.mapper = mapper;
		}

		public async Task<IList<RegionModel>> GetAll()
		{
			return await this.dbContext.Regions
				.Select(r => this.mapper.Map<Region, RegionModel>(r))
				.ToListAsync();
		}
	}
}
