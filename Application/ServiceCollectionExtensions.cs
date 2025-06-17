using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddApplication(this IServiceCollection service, IConfiguration configuration)
		{
			return service;
		}
	}
}
