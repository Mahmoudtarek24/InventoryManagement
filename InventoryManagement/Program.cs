using Application;
using Infrastructure;
using Infrastructure.Seeds;
using InventoryManagement.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Threading.Tasks;
using System.Security.Claims;

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
			builder.Services.AddSwaggerGen(options =>
			{
				options.EnableAnnotations();	
				options.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "Inventory Management",
					Contact = new OpenApiContact
					{
						Name = "Mahmoud Tarek",
						Email = "mahmoudtark556@gmail.com"
					},
					Description = "Inventory System API - Learning Project",
					License = new OpenApiLicense
					{
						Name = "View on GitHub",
						Url = new Uri("https://github.com/Mahmoudtarek24/InventoryManagement")
					}
				});
				//options.OperationFilter<FileUploadOperation>();
				options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,

				});
				options.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference=new OpenApiReference
							{
								Type=ReferenceType.SecurityScheme,
								Id="Bearer"
							},
							Name="Bearer",
							In=ParameterLocation.Header
						},
						new List<string>(){}
					}
				});

			});

			builder.Services.AddAuthentication(opthios =>
			{
				opthios.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				opthios.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				opthios.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.SaveToken = true;
				options.RequireHttpsMetadata = false;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateIssuerSigningKey = true,
					ValidateLifetime = true,
					ValidIssuer = builder.Configuration["JWTSetting:Issuer"],
					ValidAudience = builder.Configuration["JWTSetting:Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSetting:Key"])),
					ClockSkew = TimeSpan.Zero,
					RoleClaimType = ClaimTypes.Role
				};
			});
			builder.Services.AddAuthorization();

			builder.Services.AddInfrastructure(builder.Configuration)
							.AddApplication(builder.Configuration)
							.AddAPIServices(builder.Configuration);

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			//if (app.Environment.IsDevelopment())
			//{
			app.UseSwagger();
			app.UseSwaggerUI();
			//	}
			app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

			using (var scope = app.Services.CreateScope()) /////explain this code 
			{
				var service = scope.ServiceProvider;
				await SeedRoles.AddRolesAsync(service);
				await AddAdmin.SeedAdminAsync(service);
			}
			app.UseHttpsRedirection();

		
			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();
			app.UseMiddleware<UserContextMiddleware>();

			app.UseStaticFiles();
			app.MapControllers();

			app.Run();
		}
	}
}
