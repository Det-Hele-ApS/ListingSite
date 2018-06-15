using AutoMapper;
using ListingApp.BusinessEntities.Models;
using ListingApp.BusinessEntities.Models.Escort;
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
			this.CreateMap<EscortType, EscortTypeModel>();
			//this.CreateMap<Escort, ListingEscortModel>();
			//this.CreateMap<NewEscortModel, Escort>();
			this.CreateMap<Image, ImageModel>();
		}
    }
}
