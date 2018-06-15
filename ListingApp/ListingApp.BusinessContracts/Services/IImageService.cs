using ListingApp.BusinessEntities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ListingApp.BusinessContracts.Services
{
	public interface IImageService
    {
		Task<ImageModel> GetPrimaryImageByEscortId(Guid escortId);

		Task<IList<ImageModel>> GetImagesByEscortId(Guid escortId, bool includePrimary = false);
    }
}
