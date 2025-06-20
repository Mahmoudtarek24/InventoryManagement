using Application.Interfaces;
using Domain.Interface;
using Infrastructure.Context;
using Infrastructure.Models;
using Infrastructure.Services;
using Infrastructure.settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
			service.AddIdentity<ApplicationUser, IdentityRole>(options => { })
				.AddEntityFrameworkStores<InventoryManagementDbContext>()
				.AddDefaultTokenProviders();

			service.Configure<JWTSetting>(options => configuration.GetSection("JWTSetting").Bind(options));
			service.AddScoped<IUserService, UserService>();
			service.AddScoped<IAuthService, AuthService>();
			service.AddScoped<IRoleService, RoleService>();
			return service;
		}
	}
}
