using ListingApp.BusinessEntities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ListingApp.BusinessContracts.Services
{
	public interface IServiceService: ISlugService
	{
		Task<IList<ServiceModel>> GetAll();
	}
}
