using Application.validations.Category;
using InventoryManagement.Filters;
using InventoryManagement.Settings;
using System.Text.Json.Serialization;

namespace InventoryManagement
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddAPIServices(this IServiceCollection service, IConfiguration configuration)
		{
			service.AddScoped<RequireVerifiedSupplierAttribute>();
			service.AddControllers().AddJsonOptions(options =>
			{		
				options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
			});

			service.Configure<ImageSettings>(configuration.GetSection("ImageSettings"));
			service.AddScoped<ValidateImageAttribute>();
			return service;
		}
	}
}
