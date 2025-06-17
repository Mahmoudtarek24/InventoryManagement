using Domain.Interface;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection service, IConfiguration configuration)
		{
			service.AddDbContext<InventoryManagementDbContext>(options =>
			{
				options.UseSqlServer(configuration.GetConnectionString("InventoryDbConnection"));
			});

			service.AddScoped<IUnitOfWork,UnitOfWork.UnitOfWork>();
			return service;
		}
	}
}
