using Application.DTO_s;
using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Application.Settings;
using Application.validations.Category;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddApplication(this IServiceCollection service, IConfiguration configuration)
		{
			service.Configure<QueryParameterSetting>(options => configuration.GetSection("QueryParameterSetting"));
			service.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
			service.AddScoped<IValidator<CreateCategoryDto>, CreateCategoryDtoValidator>();
			service.AddScoped<IValidator<UpdateCategoryDto>, UpdateCategoryDtoValidator>();


			service.AddScoped<ICategoryServices, CategoryServices>();
			service.AddHttpContextAccessor();
			service.AddScoped<IProductServices, ProductServices>();
			service.AddScoped<ISupplierServices, SupplierServices>();
			service.AddScoped<RoleBasedSupplierMapper>();
			service.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
			service.AddScoped<RoleBasedPurchaseOrderMapper>();
			service.AddScoped<IInventoryService, InventoryService>();
			service.AddScoped<IWarehouseService, WarehouseService>();
			service.AddScoped<IStockMovementServices, StockMovementServices>();

			return service;
		}
	}
}
