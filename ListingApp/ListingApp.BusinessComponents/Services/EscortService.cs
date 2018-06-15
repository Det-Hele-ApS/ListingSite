﻿using AutoMapper;
using ListingApp.BusinessContracts.Services;
using ListingApp.BusinessEntities.Models.Escort;
using ListingApp.DataAccess;
using ListingApp.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ListingApp.BusinessComponents.Services
{
	public class EscortService : IEscortService
	{
		private static readonly Expression<Func<Escort, ListingEscortModel>> ModelSelector =
			e =>
			 new ListingEscortModel
			 {
				 ExternalId = e.ExternalId,
				 EscortType = e.EscortType.ExternalName,
				 Name = e.Name,
				 Services = e.EscortServices.Select(s => new ListingServiceModel
				 {
					 Name = s.Service.Name,
					 Slug = s.Service.Slug,
					 Description = s.Service.Description
				 })
			 };

		private readonly AppDbContext db;

		private readonly IMapper mapper;

		public EscortService(AppDbContext dbContext, IMapper mapper)
		{
			this.db = dbContext;
			this.mapper = mapper;
		}

		public async Task<IList<ListingEscortModel>> GetAll()
		{
			return await this.db.Escorts
				.Select(ModelSelector)
				.ToListAsync();
		}

		public async Task<IList<ListingEscortModel>> GetByAllFilters(string escortType, string serviceName, string city)
		{
			throw new NotImplementedException();
		}

		public async Task<IList<ListingEscortModel>> GetByCity(string city)
		{
			throw new NotImplementedException();
		}

		public async Task<IList<ListingEscortModel>> GetByEscortType(string escortType)
		{
			return await this.db.Escorts
				.Where(e => e.EscortType.Slug == escortType)
				.Select(ModelSelector)
				.ToListAsync();
		}

		public async Task<IList<ListingEscortModel>> GetByEscortTypeAndCity(string escortType, string city)
		{
			throw new NotImplementedException();
		}

		public async Task<IList<ListingEscortModel>> GetByEscortTypeAndService(string escortType, string serviceName)
		{
			return await this.db.Escorts
				.Where(e => e.EscortServices.Any(es => es.Service.Slug == serviceName || e.EscortType.Slug == escortType))
				.Select(ModelSelector)
				.ToListAsync();
		}

		public async Task<IList<ListingEscortModel>> GetByServiceAndCity(string serviceName, string city)
		{
			throw new NotImplementedException();
		}

		public async Task<IList<ListingEscortModel>> GetByServiceName(string serviceName)
		{
			return await this.db.Escorts
				.Where(e => e.EscortServices.Any(es => es.Service.Slug == serviceName))
				.Select(ModelSelector)
				.ToListAsync();
		}
	}
}
