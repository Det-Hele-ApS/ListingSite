using ListingApp.BusinessEntities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ListingApp.BusinessContracts.Services
{
	public interface ICityService: ISlugService
	{
		Task<IList<CityModel>> GetAll();

		Task<CityModel> GetBySlug(string slug);
	}
}
