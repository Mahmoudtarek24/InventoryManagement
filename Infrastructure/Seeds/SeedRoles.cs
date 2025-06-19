using Infrastructure.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Seeds
{
	public static class SeedRoles
	{
		public static async Task AddRolesAsync(IServiceProvider serviceProvider)
		{
			var scope = serviceProvider.CreateAsyncScope();
			var roleManger = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

			if (!roleManger.Roles.Any())
			{     
				await roleManger.CreateAsync(new IdentityRole($"{AppRoles.Admin}"));
				await roleManger.CreateAsync(new IdentityRole($"{AppRoles.InventoryManager}"));
				await roleManger.CreateAsync(new IdentityRole($"{AppRoles.SalesViewer}"));
			}
		}
	}
}
