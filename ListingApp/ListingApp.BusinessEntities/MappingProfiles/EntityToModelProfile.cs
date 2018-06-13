using AutoMapper;
using ListingApp.BusinessEntities.Models;
using ListingApp.DataAccess.Entities;

namespace ListingApp.BusinessEntities.MappingProfiles
{
	public class EntityToModelProfile: Profile
    {
		public EntityToModelProfile()
		{
			this.CreateMap<Service, ServiceModel>();
			this.CreateMap<Region, RegionModel>();
			this.CreateMap<City, CityModel>();
		}
    }
}
