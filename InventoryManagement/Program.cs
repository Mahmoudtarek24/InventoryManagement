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
				//options.SwaggerDoc("v1", new OpenApiInfo
				//{
				//	Version = "v1",
				//	Title = "Inventory Management",
				//	Contact = new OpenApiContact
				//	{
				//		Name = "Mahmoud Tarek",
				//		Email = "mahmoudtark556@gmail.com"
				//	},
				//	Description = "Inventory System API - Learning Project",
				//	License = new OpenApiLicense
				//	{
				//		Name = "View on GitHub",
				//		Url = new Uri("https://github.com/Mahmoudtarek24/InventoryManagement")
				//	}
				//});
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
				options.Events = new JwtBearerEvents
				{
					OnMessageReceived = context =>
					{
						var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
						if (!string.IsNullOrEmpty(token))
						{
							Console.WriteLine($"🔍 Token received: {token[..Math.Min(token.Length, 50)]}...");
						}
						else
						{
							Console.WriteLine("❌ No token found in Authorization header");
						}
						return Task.CompletedTask;
					},
					OnAuthenticationFailed = context =>
					{
						Console.WriteLine($"❌ JWT Authentication failed: {context.Exception.Message}");
						Console.WriteLine($"Exception type: {context.Exception.GetType().Name}");
						return Task.CompletedTask;
					},
					OnTokenValidated = context =>
					{
						Console.WriteLine($"✅ JWT Token validated successfully!");
						Console.WriteLine($"User: {context.Principal.Identity.Name}");
						Console.WriteLine($"Claims count: {context.Principal.Claims.Count()}");
						return Task.CompletedTask;
					},
					OnChallenge = context =>
					{
						Console.WriteLine($"🔒 JWT Challenge: {context.Error}, {context.ErrorDescription}");
						return Task.CompletedTask;
					}
				};
			});
			builder.Services.AddAuthorization();

			builder.Services.AddInfrastructure(builder.Configuration)
							.AddApplication(builder.Configuration);

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
			app.Use(async (context, next) =>
			{
				Console.WriteLine($"🔍 Request: {context.Request.Method} {context.Request.Path}");
				var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
				Console.WriteLine($"Auth Header: {authHeader ?? "None"}");
				await next();
			});
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseMiddleware<UserContextMiddleware>();

			app.UseStaticFiles();
			app.MapControllers();

			app.Run();
		}
	}
}
