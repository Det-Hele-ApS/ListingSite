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
	public class EscortTypeService: IEscortTypeService
	{
		private readonly AppDbContext dbContext;

		private readonly IMapper mapper;

		public EscortTypeService(AppDbContext db, IMapper mapper)
		{
			this.dbContext = db;
			this.mapper = mapper;
		}

		public async Task<IList<EscortTypeModel>> GetAll()
		{
			return await this.dbContext.EscortTypes
				.Select(es => this.mapper.Map<EscortType, EscortTypeModel>(es))
				.ToListAsync();
		}

		public async Task<bool> IsSlugExists(string slug)
		{
			return await this.dbContext.EscortTypes.AnyAsync(es => string.Compare(es.Slug, slug, true) == 0);
		}
	}
}
