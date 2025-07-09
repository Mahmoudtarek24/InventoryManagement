using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Application.validations.Category;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddApplication(this IServiceCollection service, IConfiguration configuration)
		{
		//	service.AddValidatorsFromAssemblyContaining<CreateCategoryDtoValidator>();


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
