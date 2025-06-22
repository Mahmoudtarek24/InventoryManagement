using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddApplication(this IServiceCollection service, IConfiguration configuration)
		{
			service.AddScoped<ICategoryServices, CategoryServices>();
			service.AddHttpContextAccessor();
			service.AddScoped<IProductServices, ProductServices>();
			service.AddScoped<ISupplierServices, SupplierServices>();
			service.AddScoped<RoleBasedSupplierMapper>();
			return service;
		}
	}
}
