using ListingApp.BusinessContracts.Services;
using ListingApp.BusinessEntities.Models.Listing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ListingApp.BusinessComponents.Services
{
	public class EscortService : IEscortService
	{
		public Task<IList<ListingEscortModel>> GetAll()
		{
			throw new NotImplementedException();
		}

		public Task<IList<ListingEscortModel>> GetByAllFilters(string escortType, string serviceName, string city)
		{
			throw new NotImplementedException();
		}

		public Task<IList<ListingEscortModel>> GetByCity(string city)
		{
			throw new NotImplementedException();
		}

		public Task<IList<ListingEscortModel>> GetByEscortType(string escortType)
		{
			throw new NotImplementedException();
		}

		public Task<IList<ListingEscortModel>> GetByEscortTypeAndCity(string escortType, string city)
		{
			throw new NotImplementedException();
		}

		public Task<IList<ListingEscortModel>> GetByEscortTypeAndService(string escortType, string serviceName)
		{
			throw new NotImplementedException();
		}

		public Task<IList<ListingEscortModel>> GetByServiceAndCity(string serviceName, string city)
		{
			throw new NotImplementedException();
		}

		public Task<IList<ListingEscortModel>> GetByServiceName(string serviceName)
		{
			throw new NotImplementedException();
		}
	}
}
