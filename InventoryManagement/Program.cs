using Application;
using Infrastructure;
using Infrastructure.Seeds;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddAuthentication(opthios =>
			{
				opthios.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				opthios.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;	
				opthios.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.SaveToken = true;
				options.RequireHttpsMetadata = true;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateIssuerSigningKey = true,
					ValidateLifetime = true,
					ValidIssuer = builder.Configuration["JWTSetting:Issuer"],
					ValidAudience = builder.Configuration["JWTSetting:Audience"],
					IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSetting:Key"])),
					ClockSkew=TimeSpan.Zero, //thats me the token be valid only for her time withot any secode valid after expired
				};
			});

			builder.Services.AddInfrastructure(builder.Configuration)
							.AddApplication(builder.Configuration);

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}
			using (var scope = app.Services.CreateScope()) /////explain this code 
			{
				var service = scope.ServiceProvider;
				await SeedRoles.AddRolesAsync(service);
				await AddAdmin.SeedAdminAsync(service);
			}

			app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
