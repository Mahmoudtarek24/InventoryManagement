using Application.validations.Category;
using System.Text.Json.Serialization;

namespace InventoryManagement
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddAPIServices(this IServiceCollection service, IConfiguration configuration)
		{
			service.AddControllers().AddJsonOptions(options =>
					{
						options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
					});

			return service;
		}
	}
}
