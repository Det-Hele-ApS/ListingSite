using ListingApp.BusinessEntities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ListingApp.BusinessContracts.Services
{
	public interface IEscortTypeService: ISlugService
    {
		Task<IList<EscortTypeModel>> GetAll();
	}
}
