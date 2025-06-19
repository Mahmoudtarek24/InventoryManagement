using Infrastructure.Enum;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Seeds
{
	public static class AddAdmin
	{
		public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
		{
			var scope = serviceProvider.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			var admin = new ApplicationUser()
			{
				UserName = "admin",
				Email = "admin@Inventory.com",
			    FullName = "Admin",
				ProfileImage="",
				EmailConfirmed = true
			};

			var user = await userManager.FindByEmailAsync(admin.Email);
			if (user is null)
			{
				await userManager.CreateAsync(admin, "P@ssword123");
				await userManager.AddToRoleAsync(admin,$"{AppRoles.Admin}");
			}
		}
	}
}
