using ListingApp.BusinessEntities.Models.Escort;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ListingApp.BusinessContracts.Services
{
	public interface IEscortService
    {
		Task<IList<ListingEscortModel>> GetAll();

		Task<IList<ListingEscortModel>> GetByEscortType(string escortType);

		Task<IList<ListingEscortModel>> GetByServiceName(string serviceName);

		Task<IList<ListingEscortModel>> GetByCity(string city);

		Task<IList<ListingEscortModel>> GetByServiceAndCity(string serviceName, string city);

		Task<IList<ListingEscortModel>> GetByEscortTypeAndService(string escortType, string serviceName);

		Task<IList<ListingEscortModel>> GetByEscortTypeAndCity(string escortType, string city);

		Task<IList<ListingEscortModel>> GetByAllFilters(string escortType, string serviceName, string city);

		// Task<Guid> Add(NewEscortModel escort); 
	}
}
