using AutoMapper;
using ListingApp.BusinessContracts.Services;
using ListingApp.BusinessEntities.Models;
using ListingApp.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListingApp.BusinessComponents.Services
{
	public class ImageService : IImageService
	{
		private readonly AppDbContext db;

		private readonly IMapper mapper;

		public ImageService(AppDbContext dbContext, IMapper mapper)
		{
			this.db = dbContext;
			this.mapper = mapper;
		}

		public async Task<IList<ImageModel>> GetImagesByEscortId(Guid escortId, bool includePrimary = false)
		{
			return await this.db.Images
				.Where(i => i.EscortId == escortId
					&& (includePrimary
						|| (!includePrimary && !i.IsPrimary)))
				.OrderBy(i => i.SortOrder)
				.Select(i => this.mapper.Map<ImageModel>(i))
				.ToListAsync();
		}

		public async Task<ImageModel> GetPrimaryImageByEscortId(Guid escortId)
		{
			return await this.db.Images
				.Where(i => i.EscortId == escortId && i.IsPrimary)
				.Select(i => this.mapper.Map<ImageModel>(i))
				.FirstOrDefaultAsync();
		}
	}
}
